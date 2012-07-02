import os, time
 
class usbtmc: 
	def __init__(self, device):
		self.device = device
		self.FILE = os.open(device, os.O_RDWR)
		# TODO: Test that the file opened
 
	def write(self, command):
		os.write(self.FILE, command);
 
	def read(self, length = 4000):
		return os.read(self.FILE, length)

	def name(self):
		self.write("*IDN?")
		return self.read(300)	

class RigolDG3000:
	"""Class to control the DG3000 series function gens"""
	def __init__(self, instrument):
		self.instrument = instrument
		
	def __del__(self):
		#release the controls back to the user
		self.instrument.write("SYST:LOC")
		
	def sin_out(self, freq, amp, offset):
		self.instrument.write("APPL:SIN:CH1 %i,%.2f,%.2f" % (freq, amp, offset))
	
	def make_arb(self, volt):
		comms = [
			"*IDN?",
			"FUNC USER", 
			"FREQ 2441.40625", #one sample approx .1us
			"VOLT:UNIT VPP",
			"VOLT:HIGH 5",
			"VOLT:LOW -5",
			"DATA VOLATILE," + ",".join(volt),
			"FUNC:USER VOLATILE",
			"OUTP ON",
			"FUNC USER"
			]
		for c in comms:
			print c
			self.instrument.write(c)
			time.sleep(1)
			if (len(c) > 100):
				time.sleep(5)
	
	def set_output(self, stat="ON"):
		#todo this should really try ensure it starts from the beginning
		self.instrument.write("FUCN USER")
		self.instrument.write("OUTP %s" % stat)
	
	def make_trap(self, rise, high, fall):
		"""Make a trapezoid, all times in micro seconds (min .2us resolution)"""
		#10K points gives that much such that the rest of the 1ms is low
		dat = []
		for i in range(int(rise*10)):
			dat.append(str(i/(rise*10.0)))
		for i in range(int(high*10)):
			dat.append(str(1))
		for i in range(int(fall*10)):
			dat.append(str((int(fall*10)-i)/(fall*10.0)))
		for i in range(4096 - int(rise*5 + high*5 + fall*5)):
			dat.append(str(0))
		self.make_arb(dat)
	
class RigolDSE1000:
	"""Class to control the DSE1000 series oscilliscopes""" 
	def __init__(self, instrument):
		self.instrument = instrument
		self.measureable = { 
			"clr":"CLE",
			"pkpk":"VPP?",
			"vmax":"VAMX?",
			"vmin":"VMIN?",
			"amp":"VAMP?",
			"vtop":"VTOP?",
			"vbase":"VBAS?",
			"vavg":"VAV?",
			"vrms":"VRMS?",
			"overshoot":"OVER?",
			"preshoot":"PRES?",
			"freq":"FREQ?",
			"risetime":"RIS?",
			"falltime":"FALL?",
			"period":"PER?",
			"poswidth":"PWID?",
			"negwidth":"NWID?",
			"pdutycycle":"PDUT?",
			"ndutycycle":"NDUT?"
			}
			
	def __del__(self):
		#release the controls back to the user
		self.instrument.write(":KEY:FORCE")
	
	def autoset(self, waitTime = 5):
		"""Run and autoset with an optional wait time"""
		self.instrument.write(":AUTO") # run an autoset
		time.sleep(waitTime)
	
	def measure(self, quantity, chan):
		"""make a measurement on the scope"""
		self.instrument.write(":MEAS:SOUR CHAN%i" % chan)
		s = '*'
		while s[0] == '*':
			self.instrument.write(":MEAS:%s" % self.measurable[quantity])
			s = self.instrument.read(20)
		return s
	
	def get_voltage_info(self, channel):
		need = ["SCAL?", "PROB?", "OFFS?"]
		res = []
		for item in need:
			self.instrument.write(":CHAN%i:%s" % (channel, item))
			s = '*'
			while s[0] == '*':
				self.instrument.write(":CHAN%i:%s" % (channel, item))
				s = self.instrument.read(20)
				res.append[s]
			scale, probe, offset = float(res[0]), float(res[1]), float(res[2])
			return [(scale + offset)*probe, (scale + offset)*probe*255]
	
	def get_time_info(self):
		self.instrument.write(":TIM:SCAL?")
		s = '*'
		while s[0] == '*':
			self.instrument.write(":TIM:SCAL?")
			s = self.instrument.read(20)
		return float(s)
		
	def get_waveform(self, channel, mode = "NOR"):
		self.instrument.write(":TIMEBASE:SCALE %f" % float(50e-6))
		time.sleep(3)
		#from testing it always seems to b in normal mode
		self.instrument.write(":WAV:POIN:MODE %s" % mode)
		s = "*"
		while s[0] == "*":
			self.instrument.write(":WAV:DATA? CHAN%i" % channel)
			s = self.instrument.read(1024)
		return s[10:] #first 10 bytes are packet info so discard these
	
	def wait_for_ready(self):
		self.measure("period", 1) #once this return we know eveything is good
		
	def get_name(self):
		self.write("*IDN?")
		return self.read(300)

	def send_reset(self):
		self.instrument.write("*RST")
		
	def run(self):
		self.instrument.write(":RUN")
	
	def stop(self):
		self.instrument.write(":STOP")
		

def find_instruments():
	print "Looking for devices..."

	# find the scope and function generator
	#TODO make this search all tmc devices not just these two
	if not (os.path.exists('/dev/usbtmc0') and os.path.exists('/dev/usbtmc1')):
		print "Could not find two usbtmc devices."
		print "Make sure that the oscilloscope and function generator are both connected and switched on."
		return None, None
		#sys.exit(1)

	tmc1 = instrument.usbtmc('/dev/usbtmc0')
	tmc2 = instrument.usbtmc('/dev/usbtmc1')

	scope = ''
	funcgen = ''

	if tmc1.name().find('DS1102E') > -1:
		print "Found DS1102E on /dev/usbtmc0"
		scope = tmc1
	else:
		print "Found DS1102E on /dev/usbtmc1"
		scope = tmc2

	if tmc1.name().find('DG1022') > -1:
		print "Found DG1022 on /dev/usbtmc0"
		funcgen = tmc1
	else:
		print "Found DG1022 on /dev/usbtmc1"
		funcgen = tmc2

	if not (scope and funcgen):
		print "Could not initialise scope and function generator"
		return None, None
		#sys.exit(1)

	return RigolDSE1000(scope), RigolDG3000(funcgen)