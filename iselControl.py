#!/usr/bin/env python3

import serial

class ISELController:
    #written for imc-p3-1
    def __init__(self, comPort):
        self.comPort = comPort
        self.port = serial.Serial(comPort,19200, timeout=1)

    def __del__(self):
        self.port.close()

    #for the intiernationals
    def initialise(self):
        return self.intialize()

    def initialize(self):
        #initializes machine for 3 axis x,y,z
        self.send_command("@07") #Section 2.2.1
        self.reference()
        
    def reference(self):
        #runs motors back to limit switch
        self.send_command("@0R7") #section 2.2.22
        
    def send_command(self, command):
        print("Sending %s..." % command, end='')
        self.port.write(command)
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
            print(mess[c])
        else:
            print("Unknown Error Code %c" % c)
            
        return c        
