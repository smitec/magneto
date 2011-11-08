#!/usr/bin/env python

#40000 steps = 25cm
#1step = 0.00625mm

import serial

class ISELController:
	#written for imc-p3-1
	def __init__(self, comPort):
		self.comPort = comPort
		try:
			self.port = serial.Serial(comPort,19200)
		else:
			self.port = None
		self.x = 0
		self.y = 0
		self.z = 0

	def __del__(self):
		if self.port:
			self.port.close()

	#for the intiernationals
	def initialise(self):
		return self.intialize()

	def initialize(self):
		self.send_command("@07") #Section 2.2.1
		self.write_mem_def()
		self.reference()
		
	def reference(self):
		#runs motors back to limit switch
		self.send_command("@0R7") #section 2.2.22
		self.x = 0
		self.y = 0
		self.z = 0

	def relative_move(self, x, y, z):
		self.send_command("@0A " + ','.join([str(a) for a in x+y+z]))
		self.x += x[0]
		self.y += y[0]
		self.z += z[0]

	def absolute_move(self, x, y, z):
		self.send_command("@0M " + ','.join([str(a) for a in x+y+z]))
		self.x = x[0]
		self.y = y[0]
		self.z = z[0]
	
	def move_rel_quick_mm(self, xmm, ymm, zmm):
		self.relative_move(
		[int(xmm/0.00625), 20000], 
		[int(ymm/0.00625),10000], 
		[int(zmm/0.00625),20000,0,100]
		)
	
	def move_rel_quick(self, x, y, z):
		self.relative_move([x, 20000], [y,10000], [z,10000,0,100])

	def move_abs_quick(self, x, y, z):
		self.absolute_move([x, 20000], [y,10000], [z,10000,0,100])
		
	def send_command(self, command):
		if self.port:
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
		else:
			return "P" #no port message (custom)

	def write_mem_def(self):
		self.send_command("@0IX")
		self.send_command("@0d3000,3000,3000")
		self.send_command("@0IR7")
		
		self.send_command("@0IW")
	#	self.send_command("@0ID7")

def setup():
	a = ISELController("/dev/tty.PL2303-00002006")
	a.initialize()
	return a
