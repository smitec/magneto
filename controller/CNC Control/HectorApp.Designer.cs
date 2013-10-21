namespace HectorApp
{
    partial class HectorApp
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnMoveZ = new System.Windows.Forms.Button();
            this.rdoAbsolute = new System.Windows.Forms.RadioButton();
            this.rdoRelative = new System.Windows.Forms.RadioButton();
            this.btnMoveY = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnMoveX = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtYMove = new System.Windows.Forms.TextBox();
            this.txtZMove = new System.Windows.Forms.TextBox();
            this.txtXMove = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSetZero = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtZPos = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtYPos = new System.Windows.Forms.TextBox();
            this.txtXPos = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnInit = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.lstCOMPort = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.rdo3Axis = new System.Windows.Forms.RadioButton();
            this.rdo2Axis = new System.Windows.Forms.RadioButton();
            this.rdo1Axis = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.btnStopTrapezoid = new System.Windows.Forms.Button();
            this.btnStartTrapezoid = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.txtTrapV = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtTrapFall = new System.Windows.Forms.TextBox();
            this.txtTrapHigh = new System.Windows.Forms.TextBox();
            this.txtTrapRise = new System.Windows.Forms.TextBox();
            this.txtTrapLow = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.btnTestPulse = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.prgexp = new System.Windows.Forms.ProgressBar();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btnMoveZ);
            this.groupBox1.Controls.Add(this.rdoAbsolute);
            this.groupBox1.Controls.Add(this.rdoRelative);
            this.groupBox1.Controls.Add(this.btnMoveY);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnMoveX);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtYMove);
            this.groupBox1.Controls.Add(this.txtZMove);
            this.groupBox1.Controls.Add(this.txtXMove);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(219, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(276, 153);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Jog Controls";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Move Type:";
            // 
            // btnMoveZ
            // 
            this.btnMoveZ.Location = new System.Drawing.Point(191, 108);
            this.btnMoveZ.Name = "btnMoveZ";
            this.btnMoveZ.Size = new System.Drawing.Size(75, 23);
            this.btnMoveZ.TabIndex = 10;
            this.btnMoveZ.Text = "Move";
            this.btnMoveZ.UseVisualStyleBackColor = true;
            this.btnMoveZ.Click += new System.EventHandler(this.btnMoveZ_Click);
            // 
            // rdoAbsolute
            // 
            this.rdoAbsolute.AutoSize = true;
            this.rdoAbsolute.Location = new System.Drawing.Point(155, 24);
            this.rdoAbsolute.Name = "rdoAbsolute";
            this.rdoAbsolute.Size = new System.Drawing.Size(66, 17);
            this.rdoAbsolute.TabIndex = 4;
            this.rdoAbsolute.Text = "Absolute";
            this.rdoAbsolute.UseVisualStyleBackColor = true;
            // 
            // rdoRelative
            // 
            this.rdoRelative.AutoSize = true;
            this.rdoRelative.Checked = true;
            this.rdoRelative.Location = new System.Drawing.Point(85, 24);
            this.rdoRelative.Name = "rdoRelative";
            this.rdoRelative.Size = new System.Drawing.Size(64, 17);
            this.rdoRelative.TabIndex = 3;
            this.rdoRelative.TabStop = true;
            this.rdoRelative.Text = "Relative";
            this.rdoRelative.UseVisualStyleBackColor = true;
            // 
            // btnMoveY
            // 
            this.btnMoveY.Location = new System.Drawing.Point(191, 79);
            this.btnMoveY.Name = "btnMoveY";
            this.btnMoveY.Size = new System.Drawing.Size(75, 23);
            this.btnMoveY.TabIndex = 9;
            this.btnMoveY.Text = "Move";
            this.btnMoveY.UseVisualStyleBackColor = true;
            this.btnMoveY.Click += new System.EventHandler(this.btnMoveY_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Move X (mm):";
            // 
            // btnMoveX
            // 
            this.btnMoveX.Location = new System.Drawing.Point(191, 50);
            this.btnMoveX.Name = "btnMoveX";
            this.btnMoveX.Size = new System.Drawing.Size(75, 23);
            this.btnMoveX.TabIndex = 2;
            this.btnMoveX.Text = "Move";
            this.btnMoveX.UseVisualStyleBackColor = true;
            this.btnMoveX.Click += new System.EventHandler(this.btnMoveX_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Move Z (mm):";
            // 
            // txtYMove
            // 
            this.txtYMove.Location = new System.Drawing.Point(85, 81);
            this.txtYMove.Name = "txtYMove";
            this.txtYMove.Size = new System.Drawing.Size(100, 20);
            this.txtYMove.TabIndex = 5;
            this.txtYMove.Text = "0";
            // 
            // txtZMove
            // 
            this.txtZMove.Location = new System.Drawing.Point(85, 110);
            this.txtZMove.Name = "txtZMove";
            this.txtZMove.Size = new System.Drawing.Size(100, 20);
            this.txtZMove.TabIndex = 6;
            this.txtZMove.Text = "0";
            // 
            // txtXMove
            // 
            this.txtXMove.Location = new System.Drawing.Point(85, 52);
            this.txtXMove.Name = "txtXMove";
            this.txtXMove.Size = new System.Drawing.Size(100, 20);
            this.txtXMove.TabIndex = 1;
            this.txtXMove.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Move Y (mm):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSetZero);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txtZPos);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtYPos);
            this.groupBox2.Controls.Add(this.txtXPos);
            this.groupBox2.Location = new System.Drawing.Point(501, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(139, 153);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Position Info (mm)";
            // 
            // btnSetZero
            // 
            this.btnSetZero.Enabled = false;
            this.btnSetZero.Location = new System.Drawing.Point(9, 105);
            this.btnSetZero.Name = "btnSetZero";
            this.btnSetZero.Size = new System.Drawing.Size(120, 28);
            this.btnSetZero.TabIndex = 5;
            this.btnSetZero.Text = "Set Zero";
            this.btnSetZero.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 71);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Z:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "X:";
            // 
            // txtZPos
            // 
            this.txtZPos.Enabled = false;
            this.txtZPos.Location = new System.Drawing.Point(29, 68);
            this.txtZPos.Name = "txtZPos";
            this.txtZPos.Size = new System.Drawing.Size(100, 20);
            this.txtZPos.TabIndex = 3;
            this.txtZPos.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Y:";
            // 
            // txtYPos
            // 
            this.txtYPos.Enabled = false;
            this.txtYPos.Location = new System.Drawing.Point(29, 42);
            this.txtYPos.Name = "txtYPos";
            this.txtYPos.Size = new System.Drawing.Size(100, 20);
            this.txtYPos.TabIndex = 2;
            this.txtYPos.Text = "0";
            // 
            // txtXPos
            // 
            this.txtXPos.Enabled = false;
            this.txtXPos.Location = new System.Drawing.Point(29, 16);
            this.txtXPos.Name = "txtXPos";
            this.txtXPos.Size = new System.Drawing.Size(100, 20);
            this.txtXPos.TabIndex = 1;
            this.txtXPos.Text = "0";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnInit);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.lstCOMPort);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.rdo3Axis);
            this.groupBox3.Controls.Add(this.rdo2Axis);
            this.groupBox3.Controls.Add(this.rdo1Axis);
            this.groupBox3.Location = new System.Drawing.Point(13, 13);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 153);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Configuration";
            // 
            // btnInit
            // 
            this.btnInit.Location = new System.Drawing.Point(6, 123);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(188, 22);
            this.btnInit.TabIndex = 7;
            this.btnInit.Text = "Initialise";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 95);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "COM Port:";
            // 
            // lstCOMPort
            // 
            this.lstCOMPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lstCOMPort.FormattingEnabled = true;
            this.lstCOMPort.Location = new System.Drawing.Point(68, 92);
            this.lstCOMPort.Name = "lstCOMPort";
            this.lstCOMPort.Size = new System.Drawing.Size(121, 21);
            this.lstCOMPort.Sorted = true;
            this.lstCOMPort.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 20);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Axis Style:";
            // 
            // rdo3Axis
            // 
            this.rdo3Axis.AutoSize = true;
            this.rdo3Axis.Location = new System.Drawing.Point(68, 64);
            this.rdo3Axis.Name = "rdo3Axis";
            this.rdo3Axis.Size = new System.Drawing.Size(95, 17);
            this.rdo3Axis.TabIndex = 2;
            this.rdo3Axis.Text = "3 Axis (X, Y, Z)";
            this.rdo3Axis.UseVisualStyleBackColor = true;
            // 
            // rdo2Axis
            // 
            this.rdo2Axis.AutoSize = true;
            this.rdo2Axis.Enabled = false;
            this.rdo2Axis.Location = new System.Drawing.Point(68, 41);
            this.rdo2Axis.Name = "rdo2Axis";
            this.rdo2Axis.Size = new System.Drawing.Size(79, 17);
            this.rdo2Axis.TabIndex = 1;
            this.rdo2Axis.Text = "2 Axis (X,Y)";
            this.rdo2Axis.UseVisualStyleBackColor = true;
            // 
            // rdo1Axis
            // 
            this.rdo1Axis.AutoSize = true;
            this.rdo1Axis.Checked = true;
            this.rdo1Axis.Location = new System.Drawing.Point(68, 18);
            this.rdo1Axis.Name = "rdo1Axis";
            this.rdo1Axis.Size = new System.Drawing.Size(69, 17);
            this.rdo1Axis.TabIndex = 0;
            this.rdo1Axis.TabStop = true;
            this.rdo1Axis.Text = "1 Axis (X)";
            this.rdo1Axis.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label19);
            this.groupBox4.Controls.Add(this.btnStopTrapezoid);
            this.groupBox4.Controls.Add(this.btnStartTrapezoid);
            this.groupBox4.Controls.Add(this.label18);
            this.groupBox4.Controls.Add(this.txtTrapV);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.label16);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.txtTrapFall);
            this.groupBox4.Controls.Add(this.txtTrapHigh);
            this.groupBox4.Controls.Add(this.txtTrapRise);
            this.groupBox4.Controls.Add(this.txtTrapLow);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Location = new System.Drawing.Point(13, 173);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(203, 330);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Trapezoid Paramters";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(291, 50);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(120, 23);
            this.btnLoad.TabIndex = 18;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.button1_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 230);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(191, 13);
            this.label19.TabIndex = 17;
            this.label19.Text = "(remember: the amplifier gain is 2.5A/V)";
            // 
            // btnStopTrapezoid
            // 
            this.btnStopTrapezoid.Location = new System.Drawing.Point(103, 289);
            this.btnStopTrapezoid.Name = "btnStopTrapezoid";
            this.btnStopTrapezoid.Size = new System.Drawing.Size(91, 35);
            this.btnStopTrapezoid.TabIndex = 16;
            this.btnStopTrapezoid.Text = "Stop Output";
            this.btnStopTrapezoid.UseVisualStyleBackColor = true;
            this.btnStopTrapezoid.Click += new System.EventHandler(this.btnStopTrapezoid_Click);
            // 
            // btnStartTrapezoid
            // 
            this.btnStartTrapezoid.Location = new System.Drawing.Point(6, 289);
            this.btnStartTrapezoid.Name = "btnStartTrapezoid";
            this.btnStartTrapezoid.Size = new System.Drawing.Size(91, 35);
            this.btnStartTrapezoid.TabIndex = 15;
            this.btnStartTrapezoid.Text = "Start Output";
            this.btnStartTrapezoid.UseVisualStyleBackColor = true;
            this.btnStartTrapezoid.Click += new System.EventHandler(this.btnStartTrapezoid_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(10, 206);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(85, 13);
            this.label18.TabIndex = 14;
            this.label18.Text = "Max Voltage (V):";
            // 
            // txtTrapV
            // 
            this.txtTrapV.Location = new System.Drawing.Point(94, 203);
            this.txtTrapV.Name = "txtTrapV";
            this.txtTrapV.Size = new System.Drawing.Size(100, 20);
            this.txtTrapV.TabIndex = 13;
            this.txtTrapV.Text = "0.5";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(135, 151);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(20, 13);
            this.label17.TabIndex = 12;
            this.label17.Text = "fall";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(100, 138);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(27, 13);
            this.label16.TabIndex = 11;
            this.label16.Text = "high";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(67, 151);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(23, 13);
            this.label15.TabIndex = 10;
            this.label15.Text = "rise";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(37, 167);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(23, 13);
            this.label14.TabIndex = 9;
            this.label14.Text = "low";
            // 
            // txtTrapFall
            // 
            this.txtTrapFall.Location = new System.Drawing.Point(68, 100);
            this.txtTrapFall.Name = "txtTrapFall";
            this.txtTrapFall.Size = new System.Drawing.Size(100, 20);
            this.txtTrapFall.TabIndex = 7;
            this.txtTrapFall.Text = "2";
            // 
            // txtTrapHigh
            // 
            this.txtTrapHigh.Location = new System.Drawing.Point(68, 73);
            this.txtTrapHigh.Name = "txtTrapHigh";
            this.txtTrapHigh.Size = new System.Drawing.Size(100, 20);
            this.txtTrapHigh.TabIndex = 6;
            this.txtTrapHigh.Text = "20";
            // 
            // txtTrapRise
            // 
            this.txtTrapRise.Location = new System.Drawing.Point(68, 46);
            this.txtTrapRise.Name = "txtTrapRise";
            this.txtTrapRise.Size = new System.Drawing.Size(100, 20);
            this.txtTrapRise.TabIndex = 5;
            this.txtTrapRise.Text = "2";
            // 
            // txtTrapLow
            // 
            this.txtTrapLow.Location = new System.Drawing.Point(68, 19);
            this.txtTrapLow.Name = "txtTrapLow";
            this.txtTrapLow.Size = new System.Drawing.Size(100, 20);
            this.txtTrapLow.TabIndex = 4;
            this.txtTrapLow.Text = "50";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 76);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(54, 13);
            this.label13.TabIndex = 3;
            this.label13.Text = "High (ms):";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 103);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 13);
            this.label12.TabIndex = 2;
            this.label12.Text = "Fall (ms):";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 49);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 13);
            this.label11.TabIndex = 1;
            this.label11.Text = "Rise (ms):";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 22);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Low (ms):";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.prgexp);
            this.groupBox5.Controls.Add(this.btnRun);
            this.groupBox5.Controls.Add(this.btnTestPulse);
            this.groupBox5.Controls.Add(this.btnLoad);
            this.groupBox5.Controls.Add(this.label20);
            this.groupBox5.Controls.Add(this.txtFolder);
            this.groupBox5.Controls.Add(this.button2);
            this.groupBox5.Location = new System.Drawing.Point(219, 173);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(421, 151);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Load Experiment";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(381, 22);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(30, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtFolder
            // 
            this.txtFolder.Location = new System.Drawing.Point(107, 24);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(268, 20);
            this.txtFolder.TabIndex = 1;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(7, 27);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(94, 13);
            this.label20.TabIndex = 4;
            this.label20.Text = "Experiment Folder:";
            // 
            // btnTestPulse
            // 
            this.btnTestPulse.Location = new System.Drawing.Point(165, 108);
            this.btnTestPulse.Name = "btnTestPulse";
            this.btnTestPulse.Size = new System.Drawing.Size(120, 23);
            this.btnTestPulse.TabIndex = 19;
            this.btnTestPulse.Text = "Test Pulse";
            this.btnTestPulse.UseVisualStyleBackColor = true;
            this.btnTestPulse.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(291, 108);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(120, 23);
            this.btnRun.TabIndex = 20;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            // 
            // prgexp
            // 
            this.prgexp.Location = new System.Drawing.Point(11, 79);
            this.prgexp.Name = "prgexp";
            this.prgexp.Size = new System.Drawing.Size(400, 23);
            this.prgexp.TabIndex = 21;
            // 
            // HectorApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 515);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "HectorApp";
            this.Text = "HectorApp";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnMoveX;
        private System.Windows.Forms.TextBox txtXMove;
        private System.Windows.Forms.RadioButton rdoAbsolute;
        private System.Windows.Forms.RadioButton rdoRelative;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnMoveZ;
        private System.Windows.Forms.Button btnMoveY;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtYMove;
        private System.Windows.Forms.TextBox txtZMove;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSetZero;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtZPos;
        private System.Windows.Forms.TextBox txtYPos;
        private System.Windows.Forms.TextBox txtXPos;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox lstCOMPort;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton rdo3Axis;
        private System.Windows.Forms.RadioButton rdo2Axis;
        private System.Windows.Forms.RadioButton rdo1Axis;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnStopTrapezoid;
        private System.Windows.Forms.Button btnStartTrapezoid;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtTrapV;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtTrapFall;
        private System.Windows.Forms.TextBox txtTrapHigh;
        private System.Windows.Forms.TextBox txtTrapRise;
        private System.Windows.Forms.TextBox txtTrapLow;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ProgressBar prgexp;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnTestPulse;
    }
}

