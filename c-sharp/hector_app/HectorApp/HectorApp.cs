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
        Task data_output, pulse_output, recording_task;
        double current_x = 0, current_y = 0, current_z = 0, voltage = 1, pulse_spacing = 0.050;
        bool init = true;

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
                this.current_x = this.current_y = this.current_z = 0;
            }
            else
            {
                this.isc = new iselController(lstCOMPort.SelectedItem.ToString(), axes);
            }

            init = false;
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
           // btnSetZero.Enabled = init_done;
        }

        private void update_positions()
        {
            txtXPos.Text = current_x.ToString();
            txtYPos.Text = current_y.ToString();
            txtZPos.Text = current_z.ToString();
        }

        private void btnMoveX_Click(object sender, EventArgs e)
        {
            if (rdoAbsolute.Checked)
            {
                current_x = Convert.ToDouble(txtXMove.Text);
                this.isc.move_abs_mm(Convert.ToDouble(txtXMove.Text), current_y, current_z);
            }
            else
            {
                current_x += Convert.ToDouble(txtXMove.Text);
                this.isc.move_rel_mm(Convert.ToDouble(txtXMove.Text), current_y, current_z);
            }

            update_positions();
        }

        private void btnMoveY_Click(object sender, EventArgs e)
        {
            if (rdoAbsolute.Checked)
            {
                current_y = Convert.ToDouble(txtYMove.Text);
                this.isc.move_abs_mm(current_x, Convert.ToDouble(txtYMove.Text), current_z);
            }
            else
            {
                current_y += Convert.ToDouble(txtYMove.Text);
                this.isc.move_rel_mm(current_x, Convert.ToDouble(txtYMove.Text), current_z);
            }

            update_positions();
        }

        private void btnMoveZ_Click(object sender, EventArgs e)
        {
            if (rdoAbsolute.Checked)
            {
                current_z = Convert.ToDouble(txtZMove.Text);
                this.isc.move_abs_mm(current_x, current_y, Convert.ToDouble(txtZMove.Text));
            }
            else
            {
                current_z += Convert.ToDouble(txtZMove.Text);
                this.isc.move_rel_mm(current_x, current_y, Convert.ToDouble(txtZMove.Text));
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

            /*
            //NEGATIVE TRAP
            amplitude *= -1;
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
            */

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

        private void btnStartTrapezoid_Click(object sender, EventArgs e)
        {
            double low = Convert.ToDouble(txtTrapLow.Text.ToString())/1000;
            double rise = Convert.ToDouble(txtTrapRise.Text.ToString())/1000;
            double high = Convert.ToDouble(txtTrapHigh.Text.ToString())/1000;
            double fall = Convert.ToDouble(txtTrapFall.Text.ToString())/1000;
            double volts = Convert.ToDouble(txtTrapV.Text.ToString());

            data_output = new Task();
            data_output.AOChannels.CreateVoltageChannel("/Dev1/ao1", "", 0, volts, AOVoltageUnits.Volts);
            data_output.AOChannels.CreateVoltageChannel("/Dev1/ao0", "", -10, 10, AOVoltageUnits.Volts);

            pulse_output = new Task();
            pulse_output.DOChannels.CreateChannel("Dev1/port0/line0", "", ChannelLineGrouping.OneChannelForAllLines);

            // configure the sample rates
            data_output.Timing.ConfigureSampleClock("", 10000, SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, 1000);
            pulse_output.Timing.ConfigureSampleClock("/Dev1/ao/SampleClock", 10000, SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, 1000);

            // write the analog data
            double total = low + rise + high + fall;
            double[] trapezoid = GenerateTrapezoid(low, rise, high, fall, volts, 10000, total * 10000);
            double[] tri = GenerateTri(total, low, 10000, 10);

            double[,] comb = new double[2,(int)(total*10000)];

            for (int i = 0; i < (int)(total*10000); i++) {
                comb[0,i] = trapezoid[i];
                comb[1,i] = tri[i];
            }

            AnalogMultiChannelWriter ww = new AnalogMultiChannelWriter(data_output.Stream);
            ww.WriteMultiSample(false, comb);

            DigitalWaveform wfm = new DigitalWaveform(trapezoid.Length, 1);
            DigitalSingleChannelWriter d = new DigitalSingleChannelWriter(pulse_output.Stream);
            d.WriteWaveform(false, generate_pulse(trapezoid, volts));

            pulse_output.Start();
            data_output.Start();

        }

        private void btnStopTrapezoid_Click(object sender, EventArgs e)
        {
            data_output.Dispose();
            pulse_output.Dispose();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            double volts = Convert.ToDouble(txtTrapV.Text.ToString());

            data_output = new Task();
            data_output.AOChannels.CreateVoltageChannel("/Dev1/ao1", "", -10, 10, AOVoltageUnits.Volts);
            data_output.AOChannels.CreateVoltageChannel("/Dev1/ao0", "", 0, voltage, AOVoltageUnits.Volts);

            pulse_output = new Task();
            pulse_output.DOChannels.CreateChannel("Dev1/port0/line0", "", ChannelLineGrouping.OneChannelForAllLines);

            // configure the sample rates
            data_output.Timing.ConfigureSampleClock("", 10000, SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, 1000);
            pulse_output.Timing.ConfigureSampleClock("/Dev1/ao/SampleClock", 10000, SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, 1000);

            // write the analog data
            string filename = txtFolder.Text + "\\config\\pulse.csv";
            double val = 0;

            String[] values = File.ReadAllText(filename).Split(',');

            //loop through the values interpolating if needed

            double[] data = new double[(int)((pulse_spacing * 10000) + values.Length)];

            data[0] = 0.0;

            for (int i = 0; i < (int)(pulse_spacing * 10000); i++)
            {
                data[i] = 0;
            }

            for (int i = 0; i < values.Length; i++)
            {
                val = double.Parse(values[i]);

                data[i + (int)(10000 * pulse_spacing)] = val;

            }

            double[] tri = GenerateTri(pulse_spacing + (values.Length/10000.0), pulse_spacing, 10000, 10);
            double[,] comb = new double[2, (int)((pulse_spacing * 10000) + values.Length)];

            for (int i = 0; i < (int)(pulse_spacing * 10000) + values.Length; i++)
            {
                comb[0, i] = tri[i];
                comb[1, i] = data[i];
            }

            AnalogMultiChannelWriter ww = new AnalogMultiChannelWriter(data_output.Stream);
            ww.WriteMultiSample(false, comb);

            DigitalWaveform wfm = new DigitalWaveform(data.Length, 1);
            DigitalSingleChannelWriter d = new DigitalSingleChannelWriter(pulse_output.Stream);
            d.WriteWaveform(false, generate_pulse(data, volts));

            pulse_output.Start();
            data_output.Start();

            record_channels(txtFolder.Text + "/output/", 1000, 1);

            MessageBox.Show("Done");
        }

        private void record_channels(string foldername, int samples, int recordings)
        {
            Task myTaskIn = new Task();
            //ramp
            myTaskIn.AIChannels.CreateVoltageChannel("Dev1/ai0", "", AITerminalConfiguration.Rse, -10, 10, AIVoltageUnits.Volts);
            //out
            myTaskIn.AIChannels.CreateVoltageChannel("Dev1/ai1", "", AITerminalConfiguration.Rse, -10, 10, AIVoltageUnits.Volts);
            //in
            myTaskIn.AIChannels.CreateVoltageChannel("Dev1/ai2", "", AITerminalConfiguration.Rse, -10, 10, AIVoltageUnits.Volts);

            myTaskIn.Timing.ConfigureSampleClock("", 10000, SampleClockActiveEdge.Rising, SampleQuantityMode.FiniteSamples, samples);
            myTaskIn.Stream.Timeout = 20000;
            AnalogMultiChannelReader r = new AnalogMultiChannelReader(myTaskIn.Stream);

            double[,] data;

            for (int u = 0; u <= recordings; u++)
            {
                Thread.Sleep(samples / 10);
                data = r.ReadMultiSample(samples);

                TextWriter tx = new StreamWriter(foldername + u.ToString() + ".csv");
                tx.Write("Ramp,Output,Sensor\n");
                for (int i = 0; i < samples; i++)
                {
                    tx.Write(data[0, i].ToString() + "," + data[1, i].ToString() + "," + data[2, i].ToString() + "\n");
                }

                tx.Close();

            }
            myTaskIn.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtFolder.Text = fd.SelectedPath;
            }
        }
    }
}
