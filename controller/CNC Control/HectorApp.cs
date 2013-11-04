using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Threading;
using Magneto;
using NationalInstruments.DAQmx;
using NationalInstruments;

namespace HectorApp
{
    public partial class HectorApp : Form
    {

        iselController isc;
        Task data_output, pulse_output;
        double current_x = 0, current_y = 0, current_z = 0;
        double zero_x, zero_y, zero_z;
        private RadioButton rdTrap;
        private GroupBox grpSin;
        private TextBox txtFreq;
        private Label label14;
        private GroupBox grpTrap;
        private TextBox txtTrapFall;
        private TextBox txtTrapHigh;
        private TextBox txtTrapRise;
        private TextBox txtTrapLow;
        private Label label13;
        private Label label12;
        private Label label11;
        private Label label10;
        private RadioButton rdSin;
        bool init = true;
        private Label label16;
        private Label label15;
        private TextBox txtZWorld;
        private TextBox txtYWorld;
        private TextBox txtXWorld;
        private Button btnLoadPoints;

        int sampleRateG = 50000;
        private Label lblPoints;
        private Button btnRun;
        private ProgressBar prgExp;
        private Button btnResetZero;
        private Label lblStatus;
        private Label lblFreq;
        private Button btnLoadFreq;

        private class SpacePt
        {
            public double x, y, z;
        }

        List<SpacePt> points = null;
        private ProgressBar prgSub;
        private Button btnMoveAll;
        private Label lblFolder;
        private Button btnFolder;
        List<double> frequencies = null;

        String folder = "";




        public HectorApp()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (string s in SerialPort.GetPortNames())
            {
                lstCOMPort.Items.Add(s);
                lstCOMPort.SelectedIndex = 0;
            }
            toggle_controls(false);
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            int axes = 0;
            if (rdo1Axis.Checked) axes = 1;
            else if (rdo2Axis.Checked) axes = 2;
            else axes = 3;

            if (init == false)
            {
                this.isc.reference(axes);
            }
            else
            {
                this.isc = new iselController(lstCOMPort.SelectedItem.ToString(), axes);
            }

            init = false;
            this.current_x = this.current_y = this.current_z = 0;
            this.zero_x = this.zero_y = this.zero_z = 0;
            this.update_positions();
            toggle_controls(true);
        }

        private void toggle_controls(bool init_done)
        {
            // disable the init controls
            //btnInit.Enabled = !init_done;
            rdo1Axis.Enabled = rdo3Axis.Enabled = !init_done;
            lstCOMPort.Enabled = !init_done;

            // enable the jog controls
            rdoAbsolute.Enabled = rdoRelative.Enabled = init_done;
            txtXMove.Enabled = txtYMove.Enabled = txtZMove.Enabled = init_done;
            btnMoveX.Enabled = init_done;
            btnMoveY.Enabled = init_done && (this.isc.axes >= 2);
            btnMoveZ.Enabled = init_done && (this.isc.axes >= 3);
            btnMoveAll.Enabled = init_done;
            btnSetZero.Enabled = init_done;
            btnResetZero.Enabled = init_done;
        }

        private void update_positions()
        {
            //local coords
            txtXPos.Text = current_x.ToString();
            txtYPos.Text = current_y.ToString();
            txtZPos.Text = current_z.ToString();

            //world coords
            txtXWorld.Text = (current_x + zero_x).ToString();
            txtYWorld.Text = (current_y + zero_y).ToString();
            txtZWorld.Text = (current_z + zero_z).ToString();
        }

        private void btnMoveX_Click(object sender, EventArgs e)
        {
            if (rdoAbsolute.Checked)
            {
                current_x = Convert.ToDouble(txtXMove.Text);
                this.isc.move_abs_mm(current_x+zero_x, current_y+zero_y, current_z+zero_z);
            }
            else
            {
                current_x += Convert.ToDouble(txtXMove.Text);
                this.isc.move_rel_mm(Convert.ToDouble(txtXMove.Text), 0, 0);
            }

            update_positions();
        }

        private void btnMoveY_Click(object sender, EventArgs e)
        {
            if (rdoAbsolute.Checked)
            {
                current_y = Convert.ToDouble(txtYMove.Text);
                this.isc.move_abs_mm(current_x+zero_x, current_y+zero_y, current_z+zero_z);
            }
            else
            {
                current_y += Convert.ToDouble(txtYMove.Text);
                this.isc.move_rel_mm(0, Convert.ToDouble(txtYMove.Text), 0);
            }

            update_positions();
        }

        private void btnMoveZ_Click(object sender, EventArgs e)
        {
            if (rdoAbsolute.Checked)
            {
                current_z = Convert.ToDouble(txtZMove.Text);
                this.isc.move_abs_mm(current_x+zero_x, current_y+zero_y, current_z+zero_z);
            }
            else
            {
                current_z += Convert.ToDouble(txtZMove.Text);
                this.isc.move_rel_mm(0, 0, Convert.ToDouble(txtZMove.Text));
            }

            update_positions();
        }

        public static double[] GenerateTri(double time, double start, double sampleRate, double amp)
        {
            double[] tri = new double[(int)(time*sampleRate)];


            for (int i = 0; i < (int)(time*sampleRate); i++)
            {
                if (i > start * sampleRate)
                {
                    tri[i] = (2*amp / ((time - start)*sampleRate) *(i - start*(sampleRate))) - amp;
                }
                else
                {
                    tri[i] = -amp;
                }
            }

            return tri;
        }

        public static double[] GenerateTrapezoid(
            double lowTime,
            double riseTime,
            double highTime,
            double fallTime,
            double amplitude,
            double sampleClockRate,
            double samplesPerBuffer)
        {
            double deltaT = 1 / sampleClockRate;
            int intSamplesPerBuffer = (int)samplesPerBuffer;

            double[] rVal = new double[intSamplesPerBuffer];

            int j = 0;
            double step = 0;
            //low
            for (int i = 0; i < (int)(lowTime / deltaT); i++)
                rVal[i + j] = 0;
            //rise
            j += (int)(lowTime / deltaT);
            step = amplitude / (riseTime / deltaT);
            for (int i = 0; i < (int)(riseTime / deltaT); i++)
                rVal[i + j] = i * step;
            //high
            j += (int)(riseTime / deltaT);
            for (int i = 0; i < (int)(highTime / deltaT); i++)
                rVal[i + j] = amplitude;
            //fall
            j += (int)(highTime / deltaT);
            step = amplitude / (fallTime / deltaT);
            for (int i = 0; i < (int)(fallTime / deltaT); i++)
                rVal[i + j] = amplitude - i * step;

            return rVal;
        }

        public static DigitalWaveform generate_pulse(double[] trapezoid, double max_voltage) 
        {
            DigitalWaveform wfm = new DigitalWaveform(trapezoid.Length, 1);

            for (int i=0; i<trapezoid.Length; i++) {
                if (trapezoid[i] == max_voltage) wfm.Signals[0].States[i] = DigitalState.ForceUp;
                else wfm.Signals[0].States[i] = DigitalState.ForceDown;
            }

            return wfm;
        }

        public static double[] GenerateSin(double freq, double sampleRate, double amp)
        {
            double time = 1/freq;
            double[] sin = new double[(int)(time*sampleRate)];


            for (int i = 0; i < (int)(time * sampleRate); i++)
            {
                sin[i] = amp * Math.Sin((2 * Math.PI * freq) * (i / (sampleRate)));
            }

            return sin;
        }
        private void btnStartTrapezoid_Click(object sender, EventArgs e)
        {
            double low = Convert.ToDouble(txtTrapLow.Text.ToString())/1000;
            double rise = Convert.ToDouble(txtTrapRise.Text.ToString())/1000;
            double high = Convert.ToDouble(txtTrapHigh.Text.ToString())/1000;
            double fall = Convert.ToDouble(txtTrapFall.Text.ToString())/1000;
            double volts = Convert.ToDouble(txtTrapV.Text.ToString());
            double freq = Convert.ToDouble(txtFreq.Text.ToString());
            data_output = new Task();
            data_output.AOChannels.CreateVoltageChannel("/Dev1/ao1", "", -volts, volts, AOVoltageUnits.Volts);
            data_output.AOChannels.CreateVoltageChannel("/Dev1/ao0", "", -10, 10, AOVoltageUnits.Volts);

            pulse_output = new Task();
            pulse_output.DOChannels.CreateChannel("Dev1/port0/line0", "", ChannelLineGrouping.OneChannelForAllLines);

            // configure the sample rates
            data_output.Timing.ConfigureSampleClock("", sampleRateG, SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, 1000);
            pulse_output.Timing.ConfigureSampleClock("/Dev1/ao/SampleClock", sampleRateG, SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, 1000);

            // write the analog data
            double total = 0;
            double[] outData = null;
            double[] tri = null;

            if (rdTrap.Checked)
            {
                total = low + rise + high + fall;
                outData = GenerateTrapezoid(low, rise, high, fall, volts, sampleRateG, total * sampleRateG);
                tri = GenerateTri(total, 0, sampleRateG, 10);
            }
            else if (rdSin.Checked)
            {
                outData = GenerateSin(freq, sampleRateG, volts);
                total = outData.Length / (1.0*sampleRateG);
                tri = GenerateTri(total, 0, sampleRateG, 10);
            }

            double[,] comb = new double[2, (int)(total * sampleRateG)];

            for (int i = 0; i < (int)(total * sampleRateG); i++)
            {
                comb[0,i] = outData[i];
                comb[1,i] = tri[i];
            }

            AnalogMultiChannelWriter ww = new AnalogMultiChannelWriter(data_output.Stream);
            ww.WriteMultiSample(false, comb); //-ve for sin

            DigitalWaveform wfm = new DigitalWaveform(outData.Length, 1);
            DigitalSingleChannelWriter d = new DigitalSingleChannelWriter(pulse_output.Stream);
            d.WriteWaveform(false, generate_pulse(outData, volts));

            pulse_output.Start();
            data_output.Start();

            //record_channels("./output/", (int)(total * sampleRateG), 5);

            MessageBox.Show("Done");

        }

        private void btnStopTrapezoid_Click(object sender, EventArgs e)
        {
            data_output.Dispose();
            pulse_output.Dispose();

        }

        private void record_channels(string path, int samples, int recordings)
        {
            Task myTaskIn = new Task();
            //ramp
            myTaskIn.AIChannels.CreateVoltageChannel("Dev1/ai0", "", AITerminalConfiguration.Rse, -10, 10, AIVoltageUnits.Volts);
            //out
            myTaskIn.AIChannels.CreateVoltageChannel("Dev1/ai1", "", AITerminalConfiguration.Rse, -10, 10, AIVoltageUnits.Volts);
            //in
            myTaskIn.AIChannels.CreateVoltageChannel("Dev1/ai2", "", AITerminalConfiguration.Rse, -10, 10, AIVoltageUnits.Volts);

            myTaskIn.Timing.ConfigureSampleClock("", sampleRateG, SampleClockActiveEdge.Rising, SampleQuantityMode.FiniteSamples, samples);
            myTaskIn.Stream.Timeout = 20000;
            AnalogMultiChannelReader r = new AnalogMultiChannelReader(myTaskIn.Stream);

            int steps = samples;

            double[,] data;

            double[,] average = new double[steps,2]; //top row is data out , botom row data in
            int[] average_count = new int[steps];

            //TODO: characterize the amp


            for (int i = 0; i < samples; i++)
            {
                average[i, 0] = 0;
                average[i, 1] = 0;
                average_count[i] = 0;
            }

            for (int u = 0; u < recordings; u++)
            {
                Thread.Sleep(samples / 10);
                data = r.ReadMultiSample(samples);

                //place the sample in the right place
                for (int i = 0; i < samples; i++)
                {
                    int ramp_v = (int) Math.Round(steps*((data[1, i]+10) / 20.0));

                    ramp_v = Math.Max(0, ramp_v);
                    ramp_v = Math.Min(samples, ramp_v);

                    average[ramp_v, 0] += data[0, i];
                    average[ramp_v, 1] += data[2, i];
                    average_count[ramp_v]++;
                }

            }

            for (int i = 0; i < samples; i++)
            {
                average[i, 0] /= average_count[i];
                average[i, 1] /= average_count[i];
            }

            //write the data
            TextWriter tx = new StreamWriter(path);
            tx.Write("time,out,in\n");

            for (int i = 0; i < samples; i++)
            {
                tx.Write((i / (sampleRateG*1.0)) + "," + average[i, 0] + "," + average[i, 1] + "\n");
            }

            tx.Close();
            myTaskIn.Dispose();
        }

        //first letter is across second is down
        const int TYPE_XY = 0;
        const int TYPE_YZ = 1;
        const int TYPE_ZX = 2;

        private void sample_reactangle(double[] topLeft, double[] bottomRight, int steps, int type, int avgFac) {
            //open connection to ftdi


            TextWriter tx = new StreamWriter("./plane.csv");
            tx.Write("Point x,Point y,Point z,Field x, Field y, field z\n");

            double xpos, ypos, zpos;
            double xstep, ystep, zstep;
            double xfield = 0, yfield = 0, zfield = 0;

            xpos = topLeft[0];
            ypos = topLeft[1];
            zpos = topLeft[2];

            //move to the top left
            this.isc.move_abs_mm(xpos, ypos, 0);
            this.isc.move_abs_mm(xpos, ypos, zpos);

            xstep = bottomRight[0] - topLeft[0];
            ystep = bottomRight[0] - topLeft[1];
            zstep = bottomRight[0] - topLeft[2];

            xstep /= steps;
            ystep /= steps;
            zstep /= steps;

            for (int across = 0; across < steps; across++) {

                for (int down = 0; down < steps; down++) {
                    this.isc.move_abs_mm(xpos, ypos, zpos);

                    //take samples
                    for (int i = 0; i < avgFac; i++) {

                    }
                    xfield /= avgFac;
                    yfield /= avgFac;
                    zfield /= avgFac;



                    //tx.Write("Point x,Point y,Point z,Field x, Field y, field z\n");
                    tx.Write(xpos + "," + ypos + "," + zpos + "," + xfield + "," + yfield + "," + zfield + "\n");


                    //move down one
                    switch (type) {
                    case TYPE_XY:
                        ypos += ystep;
                        break;
                    case TYPE_ZX:
                        xpos += xstep;
                        break;
                    case TYPE_YZ:
                        zpos += zstep;
                        break;
                };

                }
                // move across one
                switch (type) {
                    case TYPE_XY:
                        xpos += xstep;
                        break;
                    case TYPE_ZX:
                        zpos += zstep;
                        break;
                    case TYPE_YZ:
                        ypos += ystep;
                        break;
                };
            }

            this.isc.move_abs_mm(xpos, ypos, 0);
            this.isc.move_abs_mm(0, 0, 0);
        }


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
            this.btnResetZero = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.txtZWorld = new System.Windows.Forms.TextBox();
            this.txtYWorld = new System.Windows.Forms.TextBox();
            this.txtXWorld = new System.Windows.Forms.TextBox();
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
            this.prgExp = new System.Windows.Forms.ProgressBar();
            this.lblPoints = new System.Windows.Forms.Label();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnLoadPoints = new System.Windows.Forms.Button();
            this.rdSin = new System.Windows.Forms.RadioButton();
            this.rdTrap = new System.Windows.Forms.RadioButton();
            this.grpSin = new System.Windows.Forms.GroupBox();
            this.txtFreq = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.grpTrap = new System.Windows.Forms.GroupBox();
            this.txtTrapFall = new System.Windows.Forms.TextBox();
            this.txtTrapHigh = new System.Windows.Forms.TextBox();
            this.txtTrapRise = new System.Windows.Forms.TextBox();
            this.txtTrapLow = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.btnStopTrapezoid = new System.Windows.Forms.Button();
            this.btnStartTrapezoid = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.txtTrapV = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnLoadFreq = new System.Windows.Forms.Button();
            this.lblFreq = new System.Windows.Forms.Label();
            this.prgSub = new System.Windows.Forms.ProgressBar();
            this.btnMoveAll = new System.Windows.Forms.Button();
            this.btnFolder = new System.Windows.Forms.Button();
            this.lblFolder = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.grpSin.SuspendLayout();
            this.grpTrap.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnMoveAll);
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
            this.groupBox1.Size = new System.Drawing.Size(276, 162);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Manual Control";
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
            this.btnMoveZ.Location = new System.Drawing.Point(191, 105);
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
            this.btnMoveY.Location = new System.Drawing.Point(191, 76);
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
            this.label1.Location = new System.Drawing.Point(7, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Move X (mm):";
            // 
            // btnMoveX
            // 
            this.btnMoveX.Location = new System.Drawing.Point(191, 47);
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
            this.label3.Location = new System.Drawing.Point(7, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Move Z (mm):";
            // 
            // txtYMove
            // 
            this.txtYMove.Location = new System.Drawing.Point(85, 78);
            this.txtYMove.Name = "txtYMove";
            this.txtYMove.Size = new System.Drawing.Size(100, 20);
            this.txtYMove.TabIndex = 5;
            this.txtYMove.Text = "0";
            // 
            // txtZMove
            // 
            this.txtZMove.Location = new System.Drawing.Point(85, 107);
            this.txtZMove.Name = "txtZMove";
            this.txtZMove.Size = new System.Drawing.Size(100, 20);
            this.txtZMove.TabIndex = 6;
            this.txtZMove.Text = "0";
            // 
            // txtXMove
            // 
            this.txtXMove.Location = new System.Drawing.Point(85, 49);
            this.txtXMove.Name = "txtXMove";
            this.txtXMove.Size = new System.Drawing.Size(100, 20);
            this.txtXMove.TabIndex = 1;
            this.txtXMove.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Move Y (mm):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnResetZero);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.txtZWorld);
            this.groupBox2.Controls.Add(this.txtYWorld);
            this.groupBox2.Controls.Add(this.txtXWorld);
            this.groupBox2.Controls.Add(this.btnSetZero);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txtZPos);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtYPos);
            this.groupBox2.Controls.Add(this.txtXPos);
            this.groupBox2.Location = new System.Drawing.Point(501, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(139, 162);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Position (mm)";
            // 
            // btnResetZero
            // 
            this.btnResetZero.Enabled = false;
            this.btnResetZero.Location = new System.Drawing.Point(13, 133);
            this.btnResetZero.Name = "btnResetZero";
            this.btnResetZero.Size = new System.Drawing.Size(111, 20);
            this.btnResetZero.TabIndex = 11;
            this.btnResetZero.Text = "Reset Zero";
            this.btnResetZero.UseVisualStyleBackColor = true;
            this.btnResetZero.Click += new System.EventHandler(this.btnResetZero_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(82, 16);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(35, 13);
            this.label16.TabIndex = 10;
            this.label16.Text = "World";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(30, 16);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(33, 13);
            this.label15.TabIndex = 9;
            this.label15.Text = "Local";
            // 
            // txtZWorld
            // 
            this.txtZWorld.Enabled = false;
            this.txtZWorld.Location = new System.Drawing.Point(85, 82);
            this.txtZWorld.Name = "txtZWorld";
            this.txtZWorld.Size = new System.Drawing.Size(39, 20);
            this.txtZWorld.TabIndex = 8;
            this.txtZWorld.Text = "0";
            // 
            // txtYWorld
            // 
            this.txtYWorld.Enabled = false;
            this.txtYWorld.Location = new System.Drawing.Point(85, 57);
            this.txtYWorld.Name = "txtYWorld";
            this.txtYWorld.Size = new System.Drawing.Size(39, 20);
            this.txtYWorld.TabIndex = 7;
            this.txtYWorld.Text = "0";
            // 
            // txtXWorld
            // 
            this.txtXWorld.Enabled = false;
            this.txtXWorld.Location = new System.Drawing.Point(85, 31);
            this.txtXWorld.Name = "txtXWorld";
            this.txtXWorld.Size = new System.Drawing.Size(39, 20);
            this.txtXWorld.TabIndex = 6;
            this.txtXWorld.Text = "0";
            // 
            // btnSetZero
            // 
            this.btnSetZero.Enabled = false;
            this.btnSetZero.Location = new System.Drawing.Point(13, 110);
            this.btnSetZero.Name = "btnSetZero";
            this.btnSetZero.Size = new System.Drawing.Size(111, 20);
            this.btnSetZero.TabIndex = 5;
            this.btnSetZero.Text = "Set Zero";
            this.btnSetZero.UseVisualStyleBackColor = true;
            this.btnSetZero.Click += new System.EventHandler(this.btnSetZero_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 87);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Z:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "X:";
            // 
            // txtZPos
            // 
            this.txtZPos.Enabled = false;
            this.txtZPos.Location = new System.Drawing.Point(33, 84);
            this.txtZPos.Name = "txtZPos";
            this.txtZPos.Size = new System.Drawing.Size(39, 20);
            this.txtZPos.TabIndex = 3;
            this.txtZPos.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Y:";
            // 
            // txtYPos
            // 
            this.txtYPos.Enabled = false;
            this.txtYPos.Location = new System.Drawing.Point(33, 58);
            this.txtYPos.Name = "txtYPos";
            this.txtYPos.Size = new System.Drawing.Size(39, 20);
            this.txtYPos.TabIndex = 2;
            this.txtYPos.Text = "0";
            // 
            // txtXPos
            // 
            this.txtXPos.Enabled = false;
            this.txtXPos.Location = new System.Drawing.Point(33, 32);
            this.txtXPos.Name = "txtXPos";
            this.txtXPos.Size = new System.Drawing.Size(39, 20);
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
            this.groupBox3.Size = new System.Drawing.Size(200, 162);
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
            this.groupBox4.Controls.Add(this.lblFolder);
            this.groupBox4.Controls.Add(this.btnFolder);
            this.groupBox4.Controls.Add(this.prgSub);
            this.groupBox4.Controls.Add(this.lblFreq);
            this.groupBox4.Controls.Add(this.btnLoadFreq);
            this.groupBox4.Controls.Add(this.lblStatus);
            this.groupBox4.Controls.Add(this.prgExp);
            this.groupBox4.Controls.Add(this.lblPoints);
            this.groupBox4.Controls.Add(this.btnRun);
            this.groupBox4.Controls.Add(this.btnLoadPoints);
            this.groupBox4.Controls.Add(this.rdSin);
            this.groupBox4.Controls.Add(this.rdTrap);
            this.groupBox4.Controls.Add(this.grpSin);
            this.groupBox4.Controls.Add(this.grpTrap);
            this.groupBox4.Controls.Add(this.label19);
            this.groupBox4.Controls.Add(this.btnStopTrapezoid);
            this.groupBox4.Controls.Add(this.btnStartTrapezoid);
            this.groupBox4.Controls.Add(this.label18);
            this.groupBox4.Controls.Add(this.txtTrapV);
            this.groupBox4.Location = new System.Drawing.Point(13, 181);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(627, 389);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Experiment";
            // 
            // prgExp
            // 
            this.prgExp.Location = new System.Drawing.Point(206, 337);
            this.prgExp.Name = "prgExp";
            this.prgExp.Size = new System.Drawing.Size(334, 20);
            this.prgExp.TabIndex = 25;
            // 
            // lblPoints
            // 
            this.lblPoints.AutoSize = true;
            this.lblPoints.Location = new System.Drawing.Point(326, 61);
            this.lblPoints.Name = "lblPoints";
            this.lblPoints.Size = new System.Drawing.Size(92, 13);
            this.lblPoints.TabIndex = 24;
            this.lblPoints.Text = "No Points Loaded";
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(546, 337);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 46);
            this.btnRun.TabIndex = 23;
            this.btnRun.Text = "Run Experiment";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnLoadPoints
            // 
            this.btnLoadPoints.Location = new System.Drawing.Point(200, 56);
            this.btnLoadPoints.Name = "btnLoadPoints";
            this.btnLoadPoints.Size = new System.Drawing.Size(120, 23);
            this.btnLoadPoints.TabIndex = 22;
            this.btnLoadPoints.Text = "Load Points";
            this.btnLoadPoints.UseVisualStyleBackColor = true;
            this.btnLoadPoints.Click += new System.EventHandler(this.btnLoadPoints_Click_1);
            // 
            // rdSin
            // 
            this.rdSin.AutoSize = true;
            this.rdSin.Location = new System.Drawing.Point(11, 188);
            this.rdSin.Name = "rdSin";
            this.rdSin.Size = new System.Drawing.Size(65, 17);
            this.rdSin.TabIndex = 21;
            this.rdSin.Text = "Sinusoid";
            this.rdSin.UseVisualStyleBackColor = true;
            this.rdSin.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // rdTrap
            // 
            this.rdTrap.AutoSize = true;
            this.rdTrap.Checked = true;
            this.rdTrap.Location = new System.Drawing.Point(11, 28);
            this.rdTrap.Name = "rdTrap";
            this.rdTrap.Size = new System.Drawing.Size(72, 17);
            this.rdTrap.TabIndex = 20;
            this.rdTrap.TabStop = true;
            this.rdTrap.Text = "Trapezoid";
            this.rdTrap.UseVisualStyleBackColor = true;
            this.rdTrap.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // grpSin
            // 
            this.grpSin.Controls.Add(this.txtFreq);
            this.grpSin.Controls.Add(this.label14);
            this.grpSin.Enabled = false;
            this.grpSin.Location = new System.Drawing.Point(11, 211);
            this.grpSin.Name = "grpSin";
            this.grpSin.Size = new System.Drawing.Size(183, 73);
            this.grpSin.TabIndex = 19;
            this.grpSin.TabStop = false;
            this.grpSin.Text = "Sinusoid";
            // 
            // txtFreq
            // 
            this.txtFreq.Location = new System.Drawing.Point(64, 19);
            this.txtFreq.Name = "txtFreq";
            this.txtFreq.Size = new System.Drawing.Size(100, 20);
            this.txtFreq.TabIndex = 17;
            this.txtFreq.Text = "2000";
            this.txtFreq.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 22);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(53, 13);
            this.label14.TabIndex = 16;
            this.label14.Text = "Freq (Hz):";
            this.label14.Click += new System.EventHandler(this.label14_Click);
            // 
            // grpTrap
            // 
            this.grpTrap.Controls.Add(this.txtTrapFall);
            this.grpTrap.Controls.Add(this.txtTrapHigh);
            this.grpTrap.Controls.Add(this.txtTrapRise);
            this.grpTrap.Controls.Add(this.txtTrapLow);
            this.grpTrap.Controls.Add(this.label13);
            this.grpTrap.Controls.Add(this.label12);
            this.grpTrap.Controls.Add(this.label11);
            this.grpTrap.Controls.Add(this.label10);
            this.grpTrap.Location = new System.Drawing.Point(11, 50);
            this.grpTrap.Name = "grpTrap";
            this.grpTrap.Size = new System.Drawing.Size(183, 132);
            this.grpTrap.TabIndex = 18;
            this.grpTrap.TabStop = false;
            this.grpTrap.Text = "Trapezoid";
            // 
            // txtTrapFall
            // 
            this.txtTrapFall.Location = new System.Drawing.Point(64, 94);
            this.txtTrapFall.Name = "txtTrapFall";
            this.txtTrapFall.Size = new System.Drawing.Size(100, 20);
            this.txtTrapFall.TabIndex = 15;
            this.txtTrapFall.Text = "2";
            // 
            // txtTrapHigh
            // 
            this.txtTrapHigh.Location = new System.Drawing.Point(64, 67);
            this.txtTrapHigh.Name = "txtTrapHigh";
            this.txtTrapHigh.Size = new System.Drawing.Size(100, 20);
            this.txtTrapHigh.TabIndex = 14;
            this.txtTrapHigh.Text = "20";
            // 
            // txtTrapRise
            // 
            this.txtTrapRise.Location = new System.Drawing.Point(64, 40);
            this.txtTrapRise.Name = "txtTrapRise";
            this.txtTrapRise.Size = new System.Drawing.Size(100, 20);
            this.txtTrapRise.TabIndex = 13;
            this.txtTrapRise.Text = "2";
            // 
            // txtTrapLow
            // 
            this.txtTrapLow.Location = new System.Drawing.Point(64, 13);
            this.txtTrapLow.Name = "txtTrapLow";
            this.txtTrapLow.Size = new System.Drawing.Size(100, 20);
            this.txtTrapLow.TabIndex = 12;
            this.txtTrapLow.Text = "50";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 70);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(54, 13);
            this.label13.TabIndex = 11;
            this.label13.Text = "High (ms):";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 97);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 13);
            this.label12.TabIndex = 10;
            this.label12.Text = "Fall (ms):";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 43);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 13);
            this.label11.TabIndex = 9;
            this.label11.Text = "Rise (ms):";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "Low (ms):";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(7, 320);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(191, 13);
            this.label19.TabIndex = 17;
            this.label19.Text = "(remember: the amplifier gain is 2.5A/V)";
            // 
            // btnStopTrapezoid
            // 
            this.btnStopTrapezoid.Location = new System.Drawing.Point(102, 348);
            this.btnStopTrapezoid.Name = "btnStopTrapezoid";
            this.btnStopTrapezoid.Size = new System.Drawing.Size(91, 35);
            this.btnStopTrapezoid.TabIndex = 16;
            this.btnStopTrapezoid.Text = "Stop Output";
            this.btnStopTrapezoid.UseVisualStyleBackColor = true;
            this.btnStopTrapezoid.Click += new System.EventHandler(this.btnStopTrapezoid_Click);
            // 
            // btnStartTrapezoid
            // 
            this.btnStartTrapezoid.Location = new System.Drawing.Point(5, 348);
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
            this.label18.Location = new System.Drawing.Point(11, 296);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(85, 13);
            this.label18.TabIndex = 14;
            this.label18.Text = "Max Voltage (V):";
            // 
            // txtTrapV
            // 
            this.txtTrapV.Location = new System.Drawing.Point(95, 293);
            this.txtTrapV.Name = "txtTrapV";
            this.txtTrapV.Size = new System.Drawing.Size(100, 20);
            this.txtTrapV.TabIndex = 13;
            this.txtTrapV.Text = "0.5";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(204, 320);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(37, 13);
            this.lblStatus.TabIndex = 26;
            this.lblStatus.Text = "Status";
            // 
            // btnLoadFreq
            // 
            this.btnLoadFreq.Location = new System.Drawing.Point(200, 83);
            this.btnLoadFreq.Name = "btnLoadFreq";
            this.btnLoadFreq.Size = new System.Drawing.Size(120, 23);
            this.btnLoadFreq.TabIndex = 27;
            this.btnLoadFreq.Text = "Load Frequencies";
            this.btnLoadFreq.UseVisualStyleBackColor = true;
            this.btnLoadFreq.Click += new System.EventHandler(this.btnLoadFreq_Click);
            // 
            // lblFreq
            // 
            this.lblFreq.AutoSize = true;
            this.lblFreq.Location = new System.Drawing.Point(326, 88);
            this.lblFreq.Name = "lblFreq";
            this.lblFreq.Size = new System.Drawing.Size(121, 13);
            this.lblFreq.TabIndex = 28;
            this.lblFreq.Text = "No Frequencies Loaded";
            // 
            // prgSub
            // 
            this.prgSub.Location = new System.Drawing.Point(206, 363);
            this.prgSub.Name = "prgSub";
            this.prgSub.Size = new System.Drawing.Size(334, 20);
            this.prgSub.TabIndex = 29;
            // 
            // btnMoveAll
            // 
            this.btnMoveAll.Enabled = false;
            this.btnMoveAll.Location = new System.Drawing.Point(191, 134);
            this.btnMoveAll.Name = "btnMoveAll";
            this.btnMoveAll.Size = new System.Drawing.Size(75, 23);
            this.btnMoveAll.TabIndex = 12;
            this.btnMoveAll.Text = "Move All";
            this.btnMoveAll.UseVisualStyleBackColor = true;
            this.btnMoveAll.Click += new System.EventHandler(this.btnMoveAll_Click);
            // 
            // btnFolder
            // 
            this.btnFolder.Location = new System.Drawing.Point(200, 112);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(120, 23);
            this.btnFolder.TabIndex = 30;
            this.btnFolder.Text = "Select Output Folder";
            this.btnFolder.UseVisualStyleBackColor = true;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            // 
            // lblFolder
            // 
            this.lblFolder.AutoSize = true;
            this.lblFolder.Location = new System.Drawing.Point(326, 117);
            this.lblFolder.Name = "lblFolder";
            this.lblFolder.Size = new System.Drawing.Size(98, 13);
            this.lblFolder.TabIndex = 31;
            this.lblFolder.Text = "No Folder Selected";
            // 
            // HectorApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 575);
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
            this.grpSin.ResumeLayout(false);
            this.grpSin.PerformLayout();
            this.grpTrap.ResumeLayout(false);
            this.grpTrap.PerformLayout();
            this.ResumeLayout(false);

        }

        

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
        private System.Windows.Forms.Label label19;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            grpSin.Enabled = rdSin.Checked;
            grpTrap.Enabled = rdTrap.Checked;
        }

        private void btnSetZero_Click(object sender, EventArgs e)
        {
            this.set_zero();
        }

        private void set_zero() {
            this.zero_x = this.current_x;
            this.zero_y = this.current_y;
            this.zero_z = this.current_z;
            this.current_x = this.current_y = this.current_z = 0;
            this.update_positions();
        }

        private void btnLoadPoints_Click(object sender, EventArgs e)
        {

        }

        

        private void btnLoadPoints_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            List<SpacePt> result = new List<SpacePt>();

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String pts = fd.FileName;
                StreamReader sr = new StreamReader(pts);
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();

                    String[] line_pts = line.Split(',');
                    SpacePt next_pt = new SpacePt();

                    next_pt.x = double.Parse(line_pts[0]);
                    next_pt.y = double.Parse(line_pts[1]);
                    next_pt.z = double.Parse(line_pts[2]);

                    result.Add(next_pt);

                }
                sr.Close();
            }

            this.points = result;
            this.lblPoints.Text = "Loaded " + this.points.Count.ToString() + " Points.";
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (this.points == null)
            {
                MessageBox.Show("No Points Loaded. Aborting");
                return;
            }

            if (this.frequencies == null)
            {
                MessageBox.Show("No Frequencies Loaded. Aborting");
                return;
                //TODO pule shapes
            }

            //set zero
            DialogResult cd = MessageBox.Show("Set current Poition as (0,0,0)?", "Zero position", MessageBoxButtons.YesNoCancel);

            if (cd == System.Windows.Forms.DialogResult.Yes)
            {
                this.set_zero();
            }
            else if (cd == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            //check points
            lblStatus.Text = "Checking Points";
            prgExp.Maximum = this.points.Count - 1;
            for (int i = 0; i < this.points.Count; i++)
            {
                if (this.points[i].x + zero_x < 0 || this.points[i].x + zero_x > 1000)
                {
                    MessageBox.Show("Point " + (i+1).ToString() + " Exceeds the X boundaries (0-1000) with the current zero point. Aborting");
                    return;
                }

                if (this.points[i].y + zero_y < 0 || this.points[i].y + zero_y > 1000)
                {
                    MessageBox.Show("Point " + (i+1).ToString() + " Exceeds the Y boundaries (0-1000) with the current zero point. Aborting");
                    return;
                }

                if (this.points[i].z + zero_z < 0 || this.points[i].z + zero_z > 1000)
                {
                    MessageBox.Show("Point " + (i+1).ToString() + " Exceeds the Z boundaries (0-1000) with the current zero point. Aborting");
                    return;
                }
                this.prgExp.Value = i;
            }


            //check Frequencies
            lblStatus.Text = "Checking Frequencies";
            prgExp.Maximum = this.frequencies.Count - 1;
            for (int i = 0; i < this.frequencies.Count; i++)
            {
                if (this.frequencies[i] > 10000.0)
                {
                    MessageBox.Show("Frequency " + (i + 1).ToString() + " Exceeds the 10kHz Limit. Aborting");
                    return;
                }
                this.prgExp.Value = i;
            }


            // Run Experiment
            lblStatus.Text = "Running Experiment";
            prgExp.Maximum = this.points.Count-1;
            // move through the points
            for (int i = 0; i < this.points.Count; i++)
            {
                current_x = Convert.ToDouble(this.points[i].x);
                current_y = Convert.ToDouble(this.points[i].y);
                current_z = Convert.ToDouble(this.points[i].z);
                this.isc.move_abs_mm(current_x + zero_x, current_y + zero_y, current_z + zero_z);
                this.update_positions();
                this.prgExp.Value = i;
                Thread.Sleep(500);
                this.prgSub.Maximum = this.frequencies.Count - 1;
                for (int f = 0; f < this.frequencies.Count; f++)
                {
                    this.record_sin(this.frequencies[f], 5, 5, i.ToString() + "_" + f.ToString() + ".csv");
                    this.prgSub.Value = f;
                }
            }

            lblStatus.Text = "Done!";
        }

        private void btnResetZero_Click(object sender, EventArgs e)
        {
            this.current_x = this.zero_x;
            this.current_y = this.zero_y;
            this.current_z = this.zero_z;
            this.zero_x = this.zero_y = this.zero_z = 0;
            this.update_positions();
        }

        private void btnLoadFreq_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            List<double> result = new List<double>();

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String pts = fd.FileName;
                StreamReader sr = new StreamReader(pts);
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine();

                    double next_f = double.Parse(line);

                    result.Add(next_f);

                }
                sr.Close();
            }

            this.frequencies = result;
            this.lblFreq.Text = "Loaded " + this.frequencies.Count.ToString() + " Frequencies.";
        }

        private void btnMoveAll_Click(object sender, EventArgs e)
        {
            if (rdoAbsolute.Checked)
            {
                current_x = Convert.ToDouble(txtXMove.Text);
                current_y = Convert.ToDouble(txtYMove.Text);
                current_z = Convert.ToDouble(txtZMove.Text);
                this.isc.move_abs_mm(current_x + zero_x, current_y + zero_y, current_z + zero_z);
            }
            else
            {
                current_x += Convert.ToDouble(txtXMove.Text);
                current_y += Convert.ToDouble(txtYMove.Text);
                current_z += Convert.ToDouble(txtZMove.Text);
                this.isc.move_rel_mm(Convert.ToDouble(txtXMove.Text), Convert.ToDouble(txtYMove.Text), Convert.ToDouble(txtZMove.Text));
            }

            update_positions();
        }


        private void record_sin(double freq, double volts, int averages, string filename)
        {
            data_output = new Task();
            data_output.AOChannels.CreateVoltageChannel("/Dev1/ao1", "", -volts, volts, AOVoltageUnits.Volts);
            data_output.AOChannels.CreateVoltageChannel("/Dev1/ao0", "", -10, 10, AOVoltageUnits.Volts);

            pulse_output = new Task();
            pulse_output.DOChannels.CreateChannel("Dev1/port0/line0", "", ChannelLineGrouping.OneChannelForAllLines);

            // configure the sample rates
            data_output.Timing.ConfigureSampleClock("", sampleRateG, SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, 1000);
            pulse_output.Timing.ConfigureSampleClock("/Dev1/ao/SampleClock", sampleRateG, SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, 1000);

            // write the analog data
            double total = 0;
            double[] outData = null;
            double[] tri = null;

 
            outData = GenerateSin(freq, sampleRateG, volts);
            total = outData.Length / (1.0 * sampleRateG);
            tri = GenerateTri(total, 0, sampleRateG, 10);

            double[,] comb = new double[2, (int)(total * sampleRateG)];

            for (int i = 0; i < (int)(total * sampleRateG); i++)
            {
                comb[0, i] = outData[i];
                comb[1, i] = tri[i];
            }

            AnalogMultiChannelWriter ww = new AnalogMultiChannelWriter(data_output.Stream);
            ww.WriteMultiSample(false, comb); //-ve for sin

            DigitalWaveform wfm = new DigitalWaveform(outData.Length, 1);
            DigitalSingleChannelWriter d = new DigitalSingleChannelWriter(pulse_output.Stream);
            d.WriteWaveform(false, generate_pulse(outData, volts));

            pulse_output.Start();
            data_output.Start();

            record_channels(this.folder + "\\" + filename, (int)(total * sampleRateG), 5);

            pulse_output.Dispose();
            data_output.Dispose();

        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                this.folder = fd.SelectedPath;
                this.lblFolder.Text = "Selected Folder: " + this.folder;
            }
        }

    }
}
