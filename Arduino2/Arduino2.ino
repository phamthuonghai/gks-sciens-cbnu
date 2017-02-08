#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"
#include <SoftwareSerial.h>

//========================================================= Bluetooth ======================================================
SoftwareSerial blueSerial(10, 11);

//================================================== Accel & Gyro system ===================================================
// Arduino Wire library is required if I2Cdev I2CDEV_ARDUINO_WIRE implementation
// is used in I2Cdev.h
#if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
    #include "Wire.h"
#endif

const float RADIANS_TO_DEGREES = 180/3.14159;

// class default I2C address is 0x68
// specific I2C addresses may be passed as a parameter here
// AD0 low = 0x68 (default for InvenSense evaluation board)
// AD0 high = 0x69
MPU6050 mpu;

//===================================================== Preprocessing ======================================================
#define MAX_MEM_ACCELGYRO 20
int16_t _memAccelGyro[MAX_MEM_ACCELGYRO][6];
int16_t _memAccelGyroCurPos = 0;

//------- Applying filter to mem data -------
void applyFilter()
{
  int j;
  float tmp;
  for (int i=0; i<6; ++i)
  {
    tmp = 0;
    for (j=0; j<MAX_MEM_ACCELGYRO; ++j)
      tmp += _memAccelGyro[j][i] * 1.0 / MAX_MEM_ACCELGYRO;

    _memAccelGyro[_memAccelGyroCurPos][i] = (int16_t)tmp;
  }
}

//==================================================== Control system ======================================================
#define GEN_DELAY_TIME 100  // General delay time
#define SWITCH_DELAY_TIME 500
int _lastTime;               // General last time
int _lastSwitchTime;

#define MODE_ROBOT_CONTROL 1
#define MODE_PC_CONTROL 2
#define MODE_3D_SIMULATION 3
#define MODE_FLIGHT_SIMULATION 0
int _currentMode = MODE_ROBOT_CONTROL;

#define PIN_MODE_SWITCH 3
#define PIN_ACTIVATE 5
int mode_switch_state;
int activate_state;

//------- MPU control/status vars -------
bool dmpReady = false;  // set true if DMP init was successful
uint8_t mpuIntStatus;   // holds actual interrupt status byte from MPU
uint8_t devStatus;      // return status after each device operation (0 = success, !0 = error)
uint16_t packetSize;    // expected DMP packet size (default is 42 bytes)
uint16_t fifoCount;     // count of all bytes currently in FIFO
uint8_t fifoBuffer[64]; // FIFO storage buffer

int16_t ax, ay, az;
int16_t gx, gy, gz;

float base_x_gyro = 0;
float base_y_gyro = 0;
float base_z_gyro = 0;
float base_x_accel = 0;
float base_y_accel = 0;
float base_z_accel = 0;

float GYRO_FACTOR;        // how to scale gyroscope data
float ACCEL_FACTOR;       // how to scale acclerometer data

unsigned long last_read_time;
float         last_x_angle;  // These are the filtered angles
float         last_y_angle;
float         last_z_angle;  
float         last_gyro_x_angle;  // Store the gyro angles to compare drift
float         last_gyro_y_angle;
float         last_gyro_z_angle;

void set_last_read_angle_data(unsigned long time, float x, float y, float z, float x_gyro, float y_gyro, float z_gyro) {
  last_read_time = time;
  last_x_angle = x;
  last_y_angle = y;
  last_z_angle = z;
  last_gyro_x_angle = x_gyro;
  last_gyro_y_angle = y_gyro;
  last_gyro_z_angle = z_gyro;
}

inline unsigned long get_last_time() {return last_read_time;}
inline float get_last_x_angle() {return last_x_angle;}
inline float get_last_y_angle() {return last_y_angle;}
inline float get_last_z_angle() {return last_z_angle;}
inline float get_last_gyro_x_angle() {return last_gyro_x_angle;}
inline float get_last_gyro_y_angle() {return last_gyro_y_angle;}
inline float get_last_gyro_z_angle() {return last_gyro_z_angle;}

//------- Interrupt detection -------
volatile bool mpuInterrupt = false;     // indicates whether MPU interrupt pin has gone high
void dmpDataReady() {
    mpuInterrupt = true;
}

//------- Calibrate MPU6050 -------
void calibrate_sensors() {
  int num_readings = 10;
  
  mpu.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);
  
  // Read and average the raw values
  for (int i = 0; i < num_readings; i++) {
    mpu.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);
    base_x_gyro += gx;
    base_y_gyro += gy;
    base_z_gyro += gz;
    base_x_accel += ax;
    base_y_accel += ay;
    base_y_accel += az;
  }
  
  base_x_gyro /= num_readings;
  base_y_gyro /= num_readings;
  base_z_gyro /= num_readings;
  base_x_accel /= num_readings;
  base_y_accel /= num_readings;
  base_z_accel /= num_readings;
}

//------- Getting data from MPU6050 -------
Quaternion q;
VectorFloat gravity;
float euler[3];
float ypr[3];
  
void getData()
{
  unsigned long t_now = millis();

  // wait for MPU interrupt or extra packet(s) available
  while (!mpuInterrupt && fifoCount < packetSize) {
    
    // Keep calculating the values of the complementary filter angles for comparison with DMP here
    // Read the raw accel/gyro values from the MPU-6050
    mpu.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);
          
    // Get time of last raw data read
    unsigned long t_now = millis();
      
    // Remove offsets and scale gyro data  
    float gyro_x = (gx - base_x_gyro)/GYRO_FACTOR;
    float gyro_y = (gy - base_y_gyro)/GYRO_FACTOR;
    float gyro_z = (gz - base_z_gyro)/GYRO_FACTOR;
    float accel_x = ax; // - base_x_accel;
    float accel_y = ay; // - base_y_accel;
    float accel_z = az; // - base_z_accel;
    
          
    float accel_angle_y = atan(-1*accel_x/sqrt(pow(accel_y,2) + pow(accel_z,2)))*RADIANS_TO_DEGREES;
    float accel_angle_x = atan(accel_y/sqrt(pow(accel_x,2) + pow(accel_z,2)))*RADIANS_TO_DEGREES;
    float accel_angle_z = 0;

    // Compute the (filtered) gyro angles
    float dt =(t_now - get_last_time())/1000.0;
    float gyro_angle_x = gyro_x*dt + get_last_x_angle();
    float gyro_angle_y = gyro_y*dt + get_last_y_angle();
    float gyro_angle_z = gyro_z*dt + get_last_z_angle();
    
    // Compute the drifting gyro angles
    float unfiltered_gyro_angle_x = gyro_x*dt + get_last_gyro_x_angle();
    float unfiltered_gyro_angle_y = gyro_y*dt + get_last_gyro_y_angle();
    float unfiltered_gyro_angle_z = gyro_z*dt + get_last_gyro_z_angle();     
    
    // Apply the complementary filter to figure out the change in angle - choice of alpha is
    // estimated now.  Alpha depends on the sampling rate...
    const float alpha = 0.96;
    float angle_x = alpha*gyro_angle_x + (1.0 - alpha)*accel_angle_x;
    float angle_y = alpha*gyro_angle_y + (1.0 - alpha)*accel_angle_y;
    float angle_z = gyro_angle_z;  //Accelerometer doesn't give z-angle
    
    // Update the saved data with the latest values
    set_last_read_angle_data(t_now, angle_x, angle_y, angle_z, unfiltered_gyro_angle_x, unfiltered_gyro_angle_y, unfiltered_gyro_angle_z);       
  }

  // reset interrupt flag and get INT_STATUS byte
  mpuInterrupt = false;
  mpuIntStatus = mpu.getIntStatus();

  // get current FIFO count
  fifoCount = mpu.getFIFOCount();

  // check for overflow (this should never happen unless our code is too inefficient)
  if ((mpuIntStatus & 0x10) || fifoCount == 1024) {
    // reset so we can continue cleanly
    mpu.resetFIFO();
    
  // otherwise, check for DMP data ready interrupt (this should happen frequently)
  } else if (mpuIntStatus & 0x02) {
    // wait for correct available data length, should be a VERY short wait
    while (fifoCount < packetSize) fifoCount = mpu.getFIFOCount();

    // read a packet from FIFO
    mpu.getFIFOBytes(fifoBuffer, packetSize);
    
    // track FIFO count here in case there is > 1 packet available
    // (this lets us immediately read more without waiting for an interrupt)
    fifoCount -= packetSize;
    
    // Obtain Euler angles from buffer
    //mpu.dmpGetQuaternion(&q, fifoBuffer);
    //mpu.dmpGetEuler(euler, &q);
    
    // Obtain YPR angles from buffer
    mpu.dmpGetQuaternion(&q, fifoBuffer);
    mpu.dmpGetGravity(&gravity, &q);
    mpu.dmpGetYawPitchRoll(ypr, &q, &gravity);
  }
}

//========================================================= Setup ==========================================================
void setup() {
  // join I2C bus (I2Cdev library doesn't do this automatically)
  #if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
      Wire.begin();
  #elif I2CDEV_IMPLEMENTATION == I2CDEV_BUILTIN_FASTWIRE
      Fastwire::setup(400, true);
  #endif

  // initialize serial communication
  Serial.begin(38400);
  blueSerial.begin(9600);

  // Set the full scale range of the gyro
  uint8_t FS_SEL = 0;
        
  // initialize device
  mpu.initialize();
  devStatus = mpu.dmpInitialize();
  if (devStatus == 0) {
    mpu.setDMPEnabled(true);

    attachInterrupt(0, dmpDataReady, RISING);
    mpuIntStatus = mpu.getIntStatus();
    dmpReady = true;

    GYRO_FACTOR = 131.0/(FS_SEL + 1);
    
    // get expected DMP packet size for later comparison
    packetSize = mpu.dmpGetFIFOPacketSize();
  }

  calibrate_sensors();
  set_last_read_angle_data(millis(), 0, 0, 0, 0, 0, 0);
  
  _lastTime = millis();
  _lastSwitchTime = _lastTime;

  pinMode(PIN_MODE_SWITCH, INPUT);
  pinMode(PIN_ACTIVATE, INPUT);
  _currentMode = MODE_ROBOT_CONTROL;
}

//========================================================= Main ===========================================================
void loop() {
  int thisTime = millis();

  if (digitalRead(PIN_MODE_SWITCH) == HIGH)
    mode_switch_state = HIGH;
  if (digitalRead(PIN_ACTIVATE) == HIGH)
    activate_state = HIGH;
  
  if (thisTime - _lastSwitchTime > SWITCH_DELAY_TIME)
  {
    if (mode_switch_state == HIGH)
    {
      int nextMode = (_currentMode+1) % 4;
      if (nextMode == MODE_ROBOT_CONTROL)
      {
//        blueSerial.write("AT+CMODE=0\r\n");
//        blueSerial.write("AT+BIND=10:14:07:01:12:07\r\n");
//        blueSerial.write("AT+LINK=10:14:07:01:12:07\r\n");
      }
      else if (_currentMode == MODE_ROBOT_CONTROL)
      {
//        blueSerial.write("AT+CMODE=1\r\n");
      }
      Serial.print("Mode ");Serial.println(_currentMode);
      mode_switch_state = LOW;
      _currentMode = nextMode;
    }
    _lastSwitchTime = thisTime;
  }
  
  if (!dmpReady) return;
  getData();
  
  if (thisTime - _lastTime > GEN_DELAY_TIME)
  {
    switch (_currentMode)
    {
      case MODE_ROBOT_CONTROL:
        if (activate_state == HIGH)
        {
          Serial.println("Activate");
          if (ypr[1] > 0.5) // right
          {
            for (int l = 0; l < (int)(ypr[1] / 0.5); ++l)
            {
              blueSerial.println("R");
              Serial.println("R");
            }
          }
          else if (ypr[1] < -0.5) // left
          {
            for (int l = 0; l < (int)(-ypr[1] / 0.5); ++l)
            {
              blueSerial.println("L");
              Serial.println("L");
            }
          }
          else if (ypr[2] > 0.5) // forward
          {
            blueSerial.println("W");
            Serial.println("W");
          }
          else if (ypr[2] < -0.5)
          {
            blueSerial.println("S");
            Serial.println("S");
          }
        }
        break;      
      case MODE_FLIGHT_SIMULATION:

        blueSerial.print(0); blueSerial.print("\t");
        blueSerial.print(0); blueSerial.print("\t");
        blueSerial.print(0); blueSerial.print("\t");
        blueSerial.print(ypr[0]); blueSerial.print("\t");
        blueSerial.print(ypr[1]); blueSerial.print("\t");
        blueSerial.println(ypr[2]);
        
        Serial.print(0); Serial.print("\t");
        Serial.print(0); Serial.print("\t");
        Serial.print(0); Serial.print("\t");
        Serial.print(ypr[0]); Serial.print("\t");
        Serial.print(ypr[1]); Serial.print("\t");
        Serial.println(ypr[2]);
        
        break;
      default:
        mpu.getMotion6(&_memAccelGyro[_memAccelGyroCurPos][0], &_memAccelGyro[_memAccelGyroCurPos][1], &_memAccelGyro[_memAccelGyroCurPos][2],   // Accelerometer
                             &_memAccelGyro[_memAccelGyroCurPos][3], &_memAccelGyro[_memAccelGyroCurPos][4], &_memAccelGyro[_memAccelGyroCurPos][5]); // Gyroscope
        applyFilter();
        Serial.print(_memAccelGyro[_memAccelGyroCurPos][0]); Serial.print("\t");
        Serial.print(_memAccelGyro[_memAccelGyroCurPos][1]); Serial.print("\t");
        Serial.print(_memAccelGyro[_memAccelGyroCurPos][2]); Serial.print("\t");
        Serial.print(_memAccelGyro[_memAccelGyroCurPos][3]); Serial.print("\t");
        Serial.print(_memAccelGyro[_memAccelGyroCurPos][4]); Serial.print("\t");
        Serial.println(_memAccelGyro[_memAccelGyroCurPos][5]);
        blueSerial.print(_memAccelGyro[_memAccelGyroCurPos][0]); blueSerial.print("\t");
        blueSerial.print(_memAccelGyro[_memAccelGyroCurPos][1]); blueSerial.print("\t");
        blueSerial.print(_memAccelGyro[_memAccelGyroCurPos][2]); blueSerial.print("\t");
        blueSerial.print(_memAccelGyro[_memAccelGyroCurPos][3]); blueSerial.print("\t");
        blueSerial.print(_memAccelGyro[_memAccelGyroCurPos][4]); blueSerial.print("\t");
        blueSerial.println(_memAccelGyro[_memAccelGyroCurPos][5]);
      
    }
    _memAccelGyroCurPos = (_memAccelGyroCurPos++) % MAX_MEM_ACCELGYRO;
    activate_state = LOW;
    _lastTime = thisTime;
  }
}

