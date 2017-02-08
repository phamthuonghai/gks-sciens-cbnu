namespace VS1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series10 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series11 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series12 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.cbDataBits = new System.Windows.Forms.ComboBox();
            this.serialSettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cbStopBits = new System.Windows.Forms.ComboBox();
            this.cbParity = new System.Windows.Forms.ComboBox();
            this.cbBaudRate = new System.Windows.Forms.ComboBox();
            this.cbPortName = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbFileName = new System.Windows.Forms.Label();
            this.btnFileName = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.rdNegative = new System.Windows.Forms.RadioButton();
            this.rdPositive = new System.Windows.Forms.RadioButton();
            this.tbObjectName = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tbErr = new System.Windows.Forms.TextBox();
            this.Function = new System.Windows.Forms.GroupBox();
            this.rdExperiment = new System.Windows.Forms.RadioButton();
            this.rdMouseControl = new System.Windows.Forms.RadioButton();
            this.rdGameControl = new System.Windows.Forms.RadioButton();
            this.rdRobotControl = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.serialSettingsBindingSource)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.Function.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(651, 557);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbLog.Size = new System.Drawing.Size(220, 105);
            this.tbLog.TabIndex = 0;
            // 
            // chart1
            // 
            this.chart1.BackColor = System.Drawing.Color.Black;
            this.chart1.BackSecondaryColor = System.Drawing.Color.Transparent;
            this.chart1.BorderSkin.BorderColor = System.Drawing.Color.Transparent;
            this.chart1.BorderSkin.PageColor = System.Drawing.Color.Black;
            chartArea3.AxisX.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Triangle;
            chartArea3.AxisX.LabelStyle.ForeColor = System.Drawing.Color.Lime;
            chartArea3.AxisX.LineColor = System.Drawing.Color.Lime;
            chartArea3.AxisX.MajorGrid.LineColor = System.Drawing.Color.Lime;
            chartArea3.AxisX.TitleForeColor = System.Drawing.Color.Yellow;
            chartArea3.AxisX2.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Triangle;
            chartArea3.AxisX2.LineColor = System.Drawing.Color.Lime;
            chartArea3.AxisX2.MajorGrid.LineColor = System.Drawing.Color.Lime;
            chartArea3.AxisY.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Triangle;
            chartArea3.AxisY.LabelStyle.ForeColor = System.Drawing.Color.Lime;
            chartArea3.AxisY.LineColor = System.Drawing.Color.Lime;
            chartArea3.AxisY.MajorGrid.LineColor = System.Drawing.Color.Chartreuse;
            chartArea3.AxisY.TitleForeColor = System.Drawing.Color.MediumBlue;
            chartArea3.AxisY2.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Triangle;
            chartArea3.AxisY2.LineColor = System.Drawing.Color.Lime;
            chartArea3.AxisY2.MajorGrid.LineColor = System.Drawing.Color.Lime;
            chartArea3.BackColor = System.Drawing.Color.Black;
            chartArea3.BackSecondaryColor = System.Drawing.Color.Transparent;
            chartArea3.BorderColor = System.Drawing.Color.Transparent;
            chartArea3.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea3);
            legend3.BackColor = System.Drawing.Color.Black;
            legend3.Enabled = false;
            legend3.ForeColor = System.Drawing.Color.Lime;
            legend3.Name = "Legend1";
            this.chart1.Legends.Add(legend3);
            this.chart1.Location = new System.Drawing.Point(12, 5);
            this.chart1.Name = "chart1";
            this.chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Fire;
            series7.BorderWidth = 2;
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series7.Color = System.Drawing.Color.Red;
            series7.Legend = "Legend1";
            series7.MarkerColor = System.Drawing.Color.White;
            series7.MarkerSize = 3;
            series7.Name = "Ax";
            series7.XValueMember = "X";
            series7.YValueMembers = "Y";
            series8.BorderWidth = 2;
            series8.ChartArea = "ChartArea1";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series8.Color = System.Drawing.Color.Yellow;
            series8.Legend = "Legend1";
            series8.Name = "Ay";
            series9.BorderWidth = 2;
            series9.ChartArea = "ChartArea1";
            series9.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series9.Color = System.Drawing.Color.Lime;
            series9.Legend = "Legend1";
            series9.Name = "Az";
            series9.YValuesPerPoint = 4;
            this.chart1.Series.Add(series7);
            this.chart1.Series.Add(series8);
            this.chart1.Series.Add(series9);
            this.chart1.Size = new System.Drawing.Size(1107, 270);
            this.chart1.TabIndex = 1;
            this.chart1.Text = "Accelerometer";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnStop);
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Controls.Add(this.cbDataBits);
            this.groupBox1.Controls.Add(this.cbStopBits);
            this.groupBox1.Controls.Add(this.cbParity);
            this.groupBox1.Controls.Add(this.cbBaudRate);
            this.groupBox1.Controls.Add(this.cbPortName);
            this.groupBox1.Location = new System.Drawing.Point(12, 557);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(306, 105);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Port Manager";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Data bits";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(167, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Stop bits";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(167, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Parity";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Baud rate";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Port name";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(234, 68);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(62, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(170, 68);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(58, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // cbDataBits
            // 
            this.cbDataBits.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.serialSettingsBindingSource, "DataBits", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbDataBits.FormattingEnabled = true;
            this.cbDataBits.Location = new System.Drawing.Point(67, 67);
            this.cbDataBits.Name = "cbDataBits";
            this.cbDataBits.Size = new System.Drawing.Size(92, 21);
            this.cbDataBits.TabIndex = 0;
            // 
            // cbStopBits
            // 
            this.cbStopBits.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.serialSettingsBindingSource, "StopBits", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbStopBits.FormattingEnabled = true;
            this.cbStopBits.Location = new System.Drawing.Point(219, 41);
            this.cbStopBits.Name = "cbStopBits";
            this.cbStopBits.Size = new System.Drawing.Size(77, 21);
            this.cbStopBits.TabIndex = 0;
            // 
            // cbParity
            // 
            this.cbParity.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.serialSettingsBindingSource, "Parity", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbParity.FormattingEnabled = true;
            this.cbParity.Location = new System.Drawing.Point(219, 14);
            this.cbParity.Name = "cbParity";
            this.cbParity.Size = new System.Drawing.Size(77, 21);
            this.cbParity.TabIndex = 0;
            // 
            // cbBaudRate
            // 
            this.cbBaudRate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.serialSettingsBindingSource, "BaudRate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbBaudRate.FormattingEnabled = true;
            this.cbBaudRate.Location = new System.Drawing.Point(67, 41);
            this.cbBaudRate.Name = "cbBaudRate";
            this.cbBaudRate.Size = new System.Drawing.Size(92, 21);
            this.cbBaudRate.TabIndex = 0;
            // 
            // cbPortName
            // 
            this.cbPortName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.serialSettingsBindingSource, "PortName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbPortName.FormattingEnabled = true;
            this.cbPortName.Location = new System.Drawing.Point(67, 14);
            this.cbPortName.Name = "cbPortName";
            this.cbPortName.Size = new System.Drawing.Size(92, 21);
            this.cbPortName.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbFileName);
            this.groupBox2.Controls.Add(this.btnFileName);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.rdNegative);
            this.groupBox2.Controls.Add(this.rdPositive);
            this.groupBox2.Controls.Add(this.tbObjectName);
            this.groupBox2.Location = new System.Drawing.Point(451, 557);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(194, 105);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Data Info";
            // 
            // lbFileName
            // 
            this.lbFileName.AutoSize = true;
            this.lbFileName.Location = new System.Drawing.Point(83, 28);
            this.lbFileName.Name = "lbFileName";
            this.lbFileName.Size = new System.Drawing.Size(0, 13);
            this.lbFileName.TabIndex = 5;
            // 
            // btnFileName
            // 
            this.btnFileName.Location = new System.Drawing.Point(77, 19);
            this.btnFileName.Name = "btnFileName";
            this.btnFileName.Size = new System.Drawing.Size(109, 23);
            this.btnFileName.TabIndex = 3;
            this.btnFileName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFileName.UseVisualStyleBackColor = true;
            this.btnFileName.Click += new System.EventHandler(this.btnFileName_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 72);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Data label";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Data file";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Object name";
            // 
            // rdNegative
            // 
            this.rdNegative.AutoSize = true;
            this.rdNegative.Checked = true;
            this.rdNegative.Location = new System.Drawing.Point(139, 70);
            this.rdNegative.Name = "rdNegative";
            this.rdNegative.Size = new System.Drawing.Size(31, 17);
            this.rdNegative.TabIndex = 1;
            this.rdNegative.TabStop = true;
            this.rdNegative.Text = "0";
            this.rdNegative.UseVisualStyleBackColor = true;
            // 
            // rdPositive
            // 
            this.rdPositive.AutoSize = true;
            this.rdPositive.Location = new System.Drawing.Point(95, 70);
            this.rdPositive.Name = "rdPositive";
            this.rdPositive.Size = new System.Drawing.Size(31, 17);
            this.rdPositive.TabIndex = 1;
            this.rdPositive.Text = "1";
            this.rdPositive.UseVisualStyleBackColor = true;
            // 
            // tbObjectName
            // 
            this.tbObjectName.Location = new System.Drawing.Point(77, 46);
            this.tbObjectName.Name = "tbObjectName";
            this.tbObjectName.Size = new System.Drawing.Size(109, 20);
            this.tbObjectName.TabIndex = 0;
            this.tbObjectName.TextChanged += new System.EventHandler(this.tbObjectName_TextChanged);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "CSV (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            // 
            // chart2
            // 
            this.chart2.BackColor = System.Drawing.Color.Black;
            this.chart2.BackSecondaryColor = System.Drawing.Color.Transparent;
            this.chart2.BorderSkin.BorderColor = System.Drawing.Color.Transparent;
            this.chart2.BorderSkin.PageColor = System.Drawing.Color.Black;
            chartArea4.AxisX.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Triangle;
            chartArea4.AxisX.LabelStyle.ForeColor = System.Drawing.Color.Lime;
            chartArea4.AxisX.LineColor = System.Drawing.Color.Lime;
            chartArea4.AxisX.MajorGrid.LineColor = System.Drawing.Color.Lime;
            chartArea4.AxisX.TitleForeColor = System.Drawing.Color.Yellow;
            chartArea4.AxisX2.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Triangle;
            chartArea4.AxisX2.LineColor = System.Drawing.Color.Lime;
            chartArea4.AxisX2.MajorGrid.LineColor = System.Drawing.Color.Lime;
            chartArea4.AxisY.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Triangle;
            chartArea4.AxisY.LabelStyle.ForeColor = System.Drawing.Color.Lime;
            chartArea4.AxisY.LineColor = System.Drawing.Color.Lime;
            chartArea4.AxisY.MajorGrid.LineColor = System.Drawing.Color.Chartreuse;
            chartArea4.AxisY.TitleForeColor = System.Drawing.Color.MediumBlue;
            chartArea4.AxisY2.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Triangle;
            chartArea4.AxisY2.LineColor = System.Drawing.Color.Lime;
            chartArea4.AxisY2.MajorGrid.LineColor = System.Drawing.Color.Lime;
            chartArea4.BackColor = System.Drawing.Color.Black;
            chartArea4.BackSecondaryColor = System.Drawing.Color.Transparent;
            chartArea4.BorderColor = System.Drawing.Color.Transparent;
            chartArea4.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea4);
            legend4.BackColor = System.Drawing.Color.Black;
            legend4.Enabled = false;
            legend4.ForeColor = System.Drawing.Color.Lime;
            legend4.Name = "Legend1";
            this.chart2.Legends.Add(legend4);
            this.chart2.Location = new System.Drawing.Point(12, 281);
            this.chart2.Name = "chart2";
            this.chart2.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Fire;
            series10.BorderWidth = 2;
            series10.ChartArea = "ChartArea1";
            series10.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series10.Color = System.Drawing.Color.Red;
            series10.Legend = "Legend1";
            series10.MarkerColor = System.Drawing.Color.White;
            series10.MarkerSize = 3;
            series10.Name = "Gx";
            series10.XValueMember = "X";
            series10.YValueMembers = "Y";
            series11.BorderWidth = 2;
            series11.ChartArea = "ChartArea1";
            series11.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series11.Color = System.Drawing.Color.Yellow;
            series11.Legend = "Legend1";
            series11.Name = "Gy";
            series12.BorderWidth = 2;
            series12.ChartArea = "ChartArea1";
            series12.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series12.Color = System.Drawing.Color.Lime;
            series12.Legend = "Legend1";
            series12.Name = "Gz";
            series12.YValuesPerPoint = 4;
            this.chart2.Series.Add(series10);
            this.chart2.Series.Add(series11);
            this.chart2.Series.Add(series12);
            this.chart2.Size = new System.Drawing.Size(1107, 270);
            this.chart2.TabIndex = 4;
            this.chart2.Text = "Gyroscope";
            // 
            // tbErr
            // 
            this.tbErr.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.tbErr.Location = new System.Drawing.Point(877, 557);
            this.tbErr.Multiline = true;
            this.tbErr.Name = "tbErr";
            this.tbErr.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbErr.Size = new System.Drawing.Size(242, 105);
            this.tbErr.TabIndex = 0;
            // 
            // Function
            // 
            this.Function.Controls.Add(this.rdRobotControl);
            this.Function.Controls.Add(this.rdExperiment);
            this.Function.Controls.Add(this.rdMouseControl);
            this.Function.Controls.Add(this.rdGameControl);
            this.Function.Location = new System.Drawing.Point(324, 560);
            this.Function.Name = "Function";
            this.Function.Size = new System.Drawing.Size(108, 102);
            this.Function.TabIndex = 5;
            this.Function.TabStop = false;
            this.Function.Text = "Function";
            // 
            // rdExperiment
            // 
            this.rdExperiment.AutoSize = true;
            this.rdExperiment.Checked = true;
            this.rdExperiment.Location = new System.Drawing.Point(6, 79);
            this.rdExperiment.Name = "rdExperiment";
            this.rdExperiment.Size = new System.Drawing.Size(77, 17);
            this.rdExperiment.TabIndex = 0;
            this.rdExperiment.TabStop = true;
            this.rdExperiment.Text = "Experiment";
            this.rdExperiment.UseVisualStyleBackColor = true;
            // 
            // rdMouseControl
            // 
            this.rdMouseControl.AutoSize = true;
            this.rdMouseControl.Location = new System.Drawing.Point(6, 40);
            this.rdMouseControl.Name = "rdMouseControl";
            this.rdMouseControl.Size = new System.Drawing.Size(93, 17);
            this.rdMouseControl.TabIndex = 0;
            this.rdMouseControl.Text = "Mouse Control";
            this.rdMouseControl.UseVisualStyleBackColor = true;
            // 
            // rdGameControl
            // 
            this.rdGameControl.AutoSize = true;
            this.rdGameControl.Location = new System.Drawing.Point(6, 19);
            this.rdGameControl.Name = "rdGameControl";
            this.rdGameControl.Size = new System.Drawing.Size(89, 17);
            this.rdGameControl.TabIndex = 0;
            this.rdGameControl.Text = "Game Control";
            this.rdGameControl.UseVisualStyleBackColor = true;
            // 
            // rdRobotControl
            // 
            this.rdRobotControl.AutoSize = true;
            this.rdRobotControl.Location = new System.Drawing.Point(6, 59);
            this.rdRobotControl.Name = "rdRobotControl";
            this.rdRobotControl.Size = new System.Drawing.Size(90, 17);
            this.rdRobotControl.TabIndex = 1;
            this.rdRobotControl.Text = "Robot Control";
            this.rdRobotControl.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1131, 670);
            this.Controls.Add(this.Function);
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.tbErr);
            this.Controls.Add(this.tbLog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VS - PC Receiver";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.serialSettingsBindingSource)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.Function.ResumeLayout(false);
            this.Function.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbParity;
        private System.Windows.Forms.ComboBox cbBaudRate;
        private System.Windows.Forms.ComboBox cbPortName;
        private System.Windows.Forms.ComboBox cbDataBits;
        private System.Windows.Forms.ComboBox cbStopBits;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.BindingSource serialSettingsBindingSource;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rdNegative;
        private System.Windows.Forms.RadioButton rdPositive;
        private System.Windows.Forms.TextBox tbObjectName;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lbFileName;
        private System.Windows.Forms.Button btnFileName;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.TextBox tbErr;
        private System.Windows.Forms.GroupBox Function;
        private System.Windows.Forms.RadioButton rdGameControl;
        private System.Windows.Forms.RadioButton rdExperiment;
        private System.Windows.Forms.RadioButton rdMouseControl;
        private System.Windows.Forms.RadioButton rdRobotControl;
    }
}

