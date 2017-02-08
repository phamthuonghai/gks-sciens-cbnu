using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VS1
{
    public partial class Form1 : Form
    {
        private SerialPortManager _spManager;
        private List<float> _RRPeakData;
        private List<float> _PPGSignal;
        private BindingList<float> _Ax;
        private BindingList<float> _Ay;
        private BindingList<float> _Az;
        private BindingList<float> _Gx;
        private BindingList<float> _Gy;
        private BindingList<float> _Gz;

        public Form1()
        {
            InitializeComponent();
            UserInitialization();
        }

        private void UserInitialization()
        {
            _spManager = new SerialPortManager();
            SerialSettings mySerialSettings = _spManager.CurrentSerialSettings;
            serialSettingsBindingSource.DataSource = mySerialSettings;
            cbPortName.DataSource = mySerialSettings.PortNameCollection;
            cbBaudRate.DataSource = mySerialSettings.BaudRateCollection;
            cbDataBits.DataSource = mySerialSettings.DataBitsCollection;
            cbParity.DataSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            cbStopBits.DataSource = Enum.GetValues(typeof(System.IO.Ports.StopBits));

            _spManager.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved);
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);

            _RRPeakData = new List<float>();
            _PPGSignal = new List<float>();

            _Ax = new BindingList<float>();
            _Ay = new BindingList<float>();
            _Az = new BindingList<float>();

            _Gx = new BindingList<float>();
            _Gy = new BindingList<float>();
            _Gz = new BindingList<float>();

            List<int> xvalue = new List<int>();
            for (int i = 0; i < 100; ++i)
            {
                xvalue.Add(i);
                _Ax.Add(0);
                _Ay.Add(0);
                _Az.Add(0);
                _Gx.Add(0);
                _Gy.Add(0);
                _Gz.Add(0);
            }

            chart1.Series[0].Points.DataBindY(_Ax);
            chart1.Series[1].Points.DataBindY(_Ay);
            chart1.Series[2].Points.DataBindY(_Az);
            chart2.Series[0].Points.DataBindY(_Gx);
            chart2.Series[1].Points.DataBindY(_Gy);
            chart2.Series[2].Points.DataBindY(_Gz);
        }

        private string _strCurrentData = "";

        void _spManager_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            if (this.InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                this.BeginInvoke(new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved), new object[] { sender, e });
                return;
            }

            _strCurrentData = _strCurrentData + Encoding.ASCII.GetString(e.Data);
            // printLog(_strCurrentData + "\n");

            string[] datapoint = Regex.Split(_strCurrentData, "\r\n");
            if (datapoint[datapoint.Length - 1] != "")
                _strCurrentData = datapoint[datapoint.Length - 1];

            for (int i = 0; i < datapoint.Length - 1; ++i)
            {
                // =========================== ROBOT CONTROL ===========================

                if (rdRobotControl.Checked)
                {
                    printLog(datapoint[i] + "\n");
                    continue;
                }

                string[] dataAG = Regex.Split(datapoint[i], "\t");
                if (dataAG.Length != 6)
                {
                    printLog("!!! " + dataAG.Length.ToString() + " data points in this segment\n", 1);
                    continue;
                }

                float ax = 0, ay = 0, az = 0, gx = 0, gy = 0, gz = 0;
                try
                {
                    ax = float.Parse(dataAG[0]);
                    ay = float.Parse(dataAG[1]);
                    az = float.Parse(dataAG[2]);
                    gx = float.Parse(dataAG[3]);
                    gy = float.Parse(dataAG[4]);
                    gz = float.Parse(dataAG[5]);
                }
                catch (Exception)
                {
                    printLog("Cannot parse string " + dataAG.ToString() + "\n", 1);
                }
                _Ax.Add(ax);
                _Ay.Add(ay);
                _Az.Add(az);
                _Gx.Add(gx);
                _Gy.Add(gy);
                _Gz.Add(gz);

                printLog("Acc: " + ax.ToString() + " ," + ay.ToString() + " ," + az.ToString() + "\n");
                printLog("Gys: " + gx.ToString() + " ," + gy.ToString() + " ," + gz.ToString() + "\n");

                _Ax.RemoveAt(0);
                _Ay.RemoveAt(0);
                _Az.RemoveAt(0);
                _Gx.RemoveAt(0);
                _Gy.RemoveAt(0);
                _Gz.RemoveAt(0);

                // =========================== GAME CONTROL ===========================
                if (rdGameControl.Checked == true)
                {
                    // Go left/right
                    if (gy > 0) // right
                    {
                        for (int l = 0; l < (int)(gy / 0.5); ++l)
                        {
                            SendKeys.Send("{RIGHT}");
                            printLog("R", 1);
                        }
                    }
                    else // left
                    {
                        for (int l = 0; l < (int)(-gy / 0.5); ++l)
                        {
                            SendKeys.Send("{LEFT}");
                            printLog("L", 1);
                        }
                    }

                    // Flight forward/backward
                    if (gz > 0) // forward
                    {
                        for (int l = 0; l < (int)(gz / 0.5); ++l)
                        {
                            SendKeys.Send("{UP}");
                            printLog("F", 1);
                        }
                    }
                    else // backward
                    {
                        for (int l = 0; l < (int)(-gz / 0.5); ++l)
                        {
                            SendKeys.Send("{DOWN}");
                            printLog("B", 1);
                        }
                    }
                }
                // =========================== MOUSE CONTROL ===========================
                else if (rdMouseControl.Checked)
                {
                    this.Cursor = new Cursor(Cursor.Current.Handle);
                    Cursor.Position = new Point((int)(Screen.PrimaryScreen.Bounds.Width * (gx / 2 + 0.5)), 
                                                (int)(Screen.PrimaryScreen.Bounds.Height * (gz / 2 + 0.5)));
                }
            }

            chart1.Series[0].Points.DataBindY(_Ax);
            chart1.Series[1].Points.DataBindY(_Ay);
            chart1.Series[2].Points.DataBindY(_Az);
            chart2.Series[0].Points.DataBindY(_Gx);
            chart2.Series[1].Points.DataBindY(_Gy);
            chart2.Series[2].Points.DataBindY(_Gz);

        }

        // Handles the "Start Listening"-buttom click event
        private void btnStart_Click(object sender, EventArgs e)
        {
            enablePortManager(false);

            if (btnFileName.Text != "")
            {
                if ((myStream = new StreamWriter(btnFileName.Text, true)) == null)
                {
                    printLog("!!! - Can't open file\n");
                }
            }

            _spManager.StartListening();
        }

        private void enablePortManager(bool p)
        {
            btnStart.Enabled = p;
            btnStop.Enabled = !p;
            cbPortName.Enabled = p;
            cbBaudRate.Enabled = p;
            cbDataBits.Enabled = p;
            cbParity.Enabled = p;
            cbStopBits.Enabled = p;
            tbObjectName.Enabled = p;
            btnFileName.Enabled = p;
        }

        // Handles the "Stop Listening"-buttom click event
        private void btnStop_Click(object sender, EventArgs e)
        {
            enablePortManager(true);
            _spManager.StopListening();
            if (myStream != null)
                myStream.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _spManager.Dispose();
        }

        void printLog(string msg, int logType = 0)
        {
            switch (logType)
            {
                case 1:
                    tbErr.AppendText(msg);
                    tbErr.ScrollToCaret();
                    break;
                default:
                    tbLog.AppendText(msg);
                    tbLog.ScrollToCaret();
                    break;
            }
        }

        StreamWriter myStream;
        private void btnFileName_Click(object sender, EventArgs e)
        {
            if (myStream != null)
                myStream.Close();

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    btnFileName.Text = saveFileDialog1.FileName;
                }
            }
        }

        private void tbObjectName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
