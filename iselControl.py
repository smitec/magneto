#!/usr/bin/env python

import serial

class ISELController:
    #written for imc-p3-1
    def __init__(self, comPort):
        self.comPort = comPort
        self.port = serial.Serial(comPort,19200)

    def __del__(self):
        self.port.close()

    #for the intiernationals
    def initialise(self):
        return self.intialize()

    def initialize(self):
        #initializes machine for 3 axis x,y,z
        self.send_command("@0d2500,2500,2500") #set ref speed
        self.send_command("@0IR1") #invert z axis so it starts at motor end
        self.send_command("@07") #Section 2.2.1
        self.reference()
        
    def reference(self):
        #runs motors back to limit switch
        self.send_command("@0R7") #section 2.2.22

    def relative_move(self, x, y, z):
        self.send_command("@0A " + ','.join([str(a) for a in x+y+z]))

    def absolute_move(self, x, y, z):
        self.send_command("@0M " + ','.join([str(a) for a in x+y+z]))

    def move_rel_quick(self, x, y, z):
        self.relative_move([x, 20000], [y,20000], [z,20000,0,100])

    def move_abs_quick(self, x, y, z):
        self.relative_move([x, 20000], [y,20000], [z,20000,0,100])
        
    def send_command(self, command):
        print "Sending %s..." % command,
        self.port.write(command+"\r")
        c = self.port.read()
        mess = {'0':"Okay",
                '1':"Error in numeric value",
                '2':"Limit switch triggered, run reference",
                '3':"Incorrect axis specification",
                '4':"No Axis defined",
                '5':"Syntax Error",
                '6':"End of CNC Memory",
                '7':"Incorrect Number of parameters",
                '8':"Command not allowed",
                '9':"System Error",
                'D':"Speed not permitted",
                'F':"User Stop",
                'G':"Invalid Data Field",
                'H':"Cover Open",
                'R':"Reference Error, run reference"
                }
        if ( c in mess.keys()):
            print mess[c]
        else:
            print "Unknown Error Code (%s)" % c 
            
        return c        

if __name__ == '__main__':
    a = ISELController("/dev/tty.PL2303-00002006")
    a.initialize()
    a.move_rel_quick(50000,-50000,-50000)
    a.move_rel_quick(-50000,50000,50000)
