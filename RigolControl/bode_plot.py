#!/usr/bin/env python

import instrument
import os.path, sys, time, pdb
import numpy as np
import matplotlib.pyplot as plt

def find_instruments():
	print "Looking for devices..."

	# find the scope and function generator
	if not (os.path.exists('/dev/usbtmc0') and os.path.exists('/dev/usbtmc1')):
		print "Could not find two usbtmc devices. Make sure that the oscilloscope and function generator are both connected and switched on."
		sys.exit(1)

	tmc1 = instrument.usbtmc('/dev/usbtmc0')
	tmc2 = instrument.usbtmc('/dev/usbtmc1')

	scope = ''
	funcgen = ''

	if tmc1.get_name.find('DS1102E') > -1:
		print "Found DS1102E on /dev/usbtmc0"
		scope = tmc1
	else:
		print "Found DS1102E on /dev/usbtmc1"
		scope = tmc2

	if tmc1.get_name.find('DG1022') > -1:
		print "Found DG1022 on /dev/usbtmc0"
		funcgen = tmc1
	else:
		print "Found DG1022 on /dev/usbtmc1"
		funcgen = tmc2

	if not (scope and funcgen):
		print "Could not initialise scope and function generator"
		sys.exit(1)

	return scope, funcgen
	
def wait_for_ready(scope):
	s = '*'
	while s[0] == '*':
		scope.write(":MEAS:VAMP?")
		s = scope.read(20)

def scope_meas(scope, param):
	s = '*'
	while s[0] == '*':
		scope.write(":MEAS:%s" % param)
		s = scope.read(20)
	return s

def do_plot(voltage, st, stp):
	start = st
	stop = stp
	step_scale = 1

	# initial setup
	scope, funcgen = find_instruments()
	
	# set the funcgen to 100hz, 5vpp sine with 0v offset
	funcgen.write("APPL:SIN %i,%.2f,0" % (start, voltage))
	time.sleep(1)
	funcgen.write("OUTP ON")
	
	scope.write(":AUTO") # run an autoset
	time.sleep(4)
	#scope.write(":CHAN1:BWL OFF")
	scope.write(":CHAN1:COUP AC")
	scope.write(":COUN:ENAB ON")
	time.sleep(2)
	scope.write(":CHAN1:OFFS 0")
	wait_for_ready(scope)

	i = int(start)
	ticker = int(start)

	voltage_l = []
	freq_l = []
	
	scope.write(':TIM:SCAL %2.10f' % (0.25/start))
	time.sleep(1)
	
	while i <= int(stop):
		
		funcgen.write('APPL:SIN %i,%.2f,0' % (i, voltage))
		scope.write(":TIM:SCAL %2.10f" % (0.75/i))
		time.sleep(0.75)

		wait_for_ready(scope)	
		vamp = scope_meas(scope, 'VAMP?')
		freq = scope_meas(scope, 'FREQ?')

		print "actual freq: %1.2f freq: %s; amplitude: %s, timebase set: %1.10f, ticker: %i,%i" % (i, freq, vamp, 0.25/i, ticker, i % ticker)
		
		voltage_l.append(float(vamp)/(3.7*voltage))
		freq_l.append(i)
		
		scope.write(":CHAN1:SCAL %5.3f" % (float(vamp)/4))
		time.sleep(0.2)
		scope.write(":T%50")
		time.sleep(0.2)
		
		#add start/step_scale 
		i += ticker/step_scale
		if (1.0 * i / ticker) >= 10:
			ticker *= 10


	plt.semilogx(np.array(freq_l), 20*np.log10(np.array(voltage_l)))
	plt.xlabel('frequency (Hz)')
	plt.ylabel('gain (dB)')
	plt.title('frequency response (Vpp = 5V)')
	plt.grid(True)
	plt.show()
	#pdb.set_trace()
	f = open("output/v%.2f_data" % voltage, "w")
	for k in range(len(freq_l)):
		f.write("%f,%f\n" % (freq_l[k], voltage_l[k]))
	f.close()
	
def read_wave():
	# initial setup
	scope, funcgen = find_instruments()
	
	# set the funcgen to 100hz, 5vpp sine with 0v offset
	funcgen.write("APPL:SIN %i,%.2f,0" % (100, 5))
	time.sleep(1)
	funcgen.write("OUTP ON")
	
	scopeControl = instrument.RigolDSE1000(scope)
	funcControl = instrument.RigolDG3000(funcgen)
	
	funcControl.make_arb([16383*(i%2) for i in range(100000)])
	
	scopeControl.autoset()
	d = scopeControl.get_waveform(2)
	
	plt.plot(range(len(d)), np.fromstring(d, np.uint8))
	plt.show()
	
	
if __name__ == "__main__":
	read_wave()

