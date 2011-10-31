import os
 
class usbtmc: 
    def __init__(self, device):
        self.device = device
        self.FILE = os.open(device, os.O_RDWR)
 
        # TODO: Test that the file opened
 
    def write(self, command):
        os.write(self.FILE, command);
 
    def read(self, length = 4000):
        return os.read(self.FILE, length)
 
    def getName(self):
        self.write("*IDN?")
        return self.read(300)
 
    def sendReset(self):
        self.write("*RST")
 
class RigolInstrument:
    """Class to control any compliant Rigol instrument"""
    def __init__(self, device):
        self.meas = usbtmc(device)
 
        self.name = self.meas.getName()
 
    def write(self, command):
        """Send an arbitrary command directly to the instrument"""
        self.meas.write(command)
 
    def read(self, command):
        """Read an arbitrary amount of data directly from the instrument"""
        return self.meas.read(command)
 
    def reset(self):
        """Reset the instrument"""
        self.meas.sendReset()

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
	
	def get_waveform(self, channel, mode = "NOR"):
		#from testing it always seems to b in normal mode
		self.instrument.write(":WAV:POINT:MODE mode")
		self.instrument.write("WAV:DATA? CHAN%i" % channel)
		s = "*"
		while s[0] = "*":
			s = self.instrument.read(1024)
		return s
		
