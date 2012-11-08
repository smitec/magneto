using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using Magneto;
using NationalInstruments.DAQmx;
using NationalInstruments;

namespace HectorApp
{
    public partial class HectorApp : Form
    {

        iselController isc;
        Task trapezoid_output, pulse_output;
        double current_x = 0, current_y = 0, current_z = 0;
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

            double[] rVal = new double[intSamplesPerBuffer*2];
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

            trapezoid_output = new Task();
            trapezoid_output.AOChannels.CreateVoltageChannel("/Dev1/ao0", "", -1*volts, volts, AOVoltageUnits.Volts);

            pulse_output = new Task();
            pulse_output.DOChannels.CreateChannel("Dev1/port0/line0", "", ChannelLineGrouping.OneChannelForAllLines);

            // configure the sample rates
            trapezoid_output.Timing.ConfigureSampleClock("", 10000, SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, 1000);
            pulse_output.Timing.ConfigureSampleClock("/Dev1/ao/SampleClock", 10000, SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, 1000);

            // write the analog data
            double total = low + rise + high + fall;
            double[] trapezoid = GenerateTrapezoid(low, rise, high, fall, volts, 10000, total * 10000);
          
            AnalogSingleChannelWriter w = new AnalogSingleChannelWriter(trapezoid_output.Stream);
            w.WriteMultiSample(false, trapezoid);

            DigitalWaveform wfm = new DigitalWaveform(trapezoid.Length, 1);
            DigitalSingleChannelWriter d = new DigitalSingleChannelWriter(pulse_output.Stream);
            d.WriteWaveform(false, generate_pulse(trapezoid, volts));

            pulse_output.Start();
            trapezoid_output.Start();
        }

        private void btnStopTrapezoid_Click(object sender, EventArgs e)
        {
            trapezoid_output.Dispose();
            pulse_output.Dispose();

        }
    }
}
