#include "I2Cdev.h"
#include "MPU6050.h"

//================================================== Accel & Gyro system ===================================================
// Arduino Wire library is required if I2Cdev I2CDEV_ARDUINO_WIRE implementation
// is used in I2Cdev.h
#if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
    #include "Wire.h"
#endif

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
int _lastTime;               // General last time

#define MODE_ROBOT_CONTROL 1
#define MODE_PC_CONTROL 2
#define MODE_3D_SIMULATION 3
#define MODE_FLIGHT_SIMULATION 4
int _currentMode = MODE_ROBOT_CONTROL;

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

  // initialize device
  mpu.initialize();

  // use the code below to change accel/gyro offset values
  /*
  Serial.println("Updating internal sensor offsets...");
  // -76  -2359 1688  0 0 0
  Serial.print(accelgyro.getXAccelOffset()); Serial.print("\t"); // -76
  Serial.print(accelgyro.getYAccelOffset()); Serial.print("\t"); // -2359
  Serial.print(accelgyro.getZAccelOffset()); Serial.print("\t"); // 1688
  Serial.print(accelgyro.getXGyroOffset()); Serial.print("\t"); // 0
  Serial.print(accelgyro.getYGyroOffset()); Serial.print("\t"); // 0
  Serial.print(accelgyro.getZGyroOffset()); Serial.print("\t"); // 0
  Serial.print("\n");
  accelgyro.setXGyroOffset(220);
  accelgyro.setYGyroOffset(76);
  accelgyro.setZGyroOffset(-85);
  Serial.print(accelgyro.getXAccelOffset()); Serial.print("\t"); // -76
  Serial.print(accelgyro.getYAccelOffset()); Serial.print("\t"); // -2359
  Serial.print(accelgyro.getZAccelOffset()); Serial.print("\t"); // 1688
  Serial.print(accelgyro.getXGyroOffset()); Serial.print("\t"); // 0
  Serial.print(accelgyro.getYGyroOffset()); Serial.print("\t"); // 0
  Serial.print(accelgyro.getZGyroOffset()); Serial.print("\t"); // 0
  Serial.print("\n");
  */
  _lastTime = millis();

  _currentMode = MODE_PC_CONTROL;
}

//========================================================= Main ===========================================================
void loop() {
  int thisTime = millis();

  if (thisTime - _lastTime > GEN_DELAY_TIME)
  {
    
    switch (_currentMode)
    {
      case MODE_ROBOT_CONTROL:
        
        break;      
      case MODE_FLIGHT_SIMULATION:
        
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
      
    }
    _memAccelGyroCurPos = (_memAccelGyroCurPos++) % MAX_MEM_ACCELGYRO;
    _lastTime = thisTime;
  }
}

