using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace magneto
{
    public class iselController
    {
        SerialPort port;
        int axes;

        public iselController(string port_name, int axes)
        {
            this.port = new SerialPort(port_name, 19200);
            this.port.Open();
            this.axes = axes;
            this.initialize(axes);
            this.reference(axes);

        }

        public bool initialize(int axes)
        {
            if (axes == 1)
            {
                this.send_command("@01");
            }
            else if (axes == 3)
            {
                this.send_command("@07");
            }
            
            this.send_command("@0IX");
            this.send_command("@0d3000,3000,3000");
            this.send_command("@0IR7");
            this.send_command("@0IW");

            return true;
        }

        bool reference(int axes)
        {
            if (axes == 1)
            {
                this.send_command("@0R1");
            }
            else if (axes == 3)
            {
                this.send_command("@0R7");
            }

            return true;
        }

        public string send_command(string command)
        {
            this.port.Write(command + "\r");
            char c = (char)this.port.ReadChar();
            Dictionary<char, string> returnCodes = new Dictionary<char, string>();
            
            returnCodes.Add('0', "Okay");
            returnCodes.Add('1', "Error in numeric value");
            returnCodes.Add('2', "Limit switch triggered, run reference");
            returnCodes.Add('3', "Incorrect axis specification");
            returnCodes.Add('4', "No Axis defined");
            returnCodes.Add('5', "Syntax Error");
            returnCodes.Add('6', "End of CNC Memory");
            returnCodes.Add('7', "Incorrect Number of parameters");
            returnCodes.Add('8', "Command not allowed");
            returnCodes.Add('9', "System Error");
            returnCodes.Add('D', "Speed not permitted");
            returnCodes.Add('F', "User Stop");
            returnCodes.Add('G', "Invalid Data Field");
            returnCodes.Add('H', "Cover Open");
            returnCodes.Add('R', "Reference Error, run reference");
            
            return returnCodes[c];
        }

        public bool move_rel_steps(long x, long y, long z)
        {
            if (this.axes == 1)
            {
                y = z = 0;
            }
            string command = "@0A " + x.ToString() + ",5000," + y.ToString() + ",5000," + z.ToString() + ",5000,0,100";
            this.send_command(command);
            return true;
        }

        public bool move_rel_mm(double x, double y, double z)
        {
            if (this.axes == 1)
            {
                y = z = 0;
            }

            long xs, ys, zs;

            xs = mm_to_steps(x);
            ys = mm_to_steps(y);
            zs = mm_to_steps(z);

            return this.move_rel_steps(xs, ys, zs);
        }

        double step_to_mm(long steps)
        {
            return steps * 0.00625;
        }

        int mm_to_steps(double mm)
        {
            return (int)(mm / 0.00625);
        }
    }
}
