from Tkinter import *
import tkMessageBox
import iselControl as isel
import RigolControl.instrument as instrument

class CNCControl:
	
	def __init__(self, master):
		frame = Frame(master)
		frame.pack()
		
		self.canvasItems = []
		self.scope = None
		self.func = None
		
		#everything to the right of the sidebar
		self.mainFrame = Frame(frame)
		
		#create canvas frame (for graphing I guess)
		self.graphFrame = Frame(self.mainFrame, width=600, height=512, bd=1, relief=SUNKEN)
		self.plotArea = Canvas(self.graphFrame, width=600, height=512)
		self.plotArea.grid(row = 0, column=0)
		
		self.graphFrame.grid(row = 0, columns = 1)
		
		#create controls frame
		self.controlFrame = Frame(self.mainFrame, width=600)
		
		#quit Button
		self.btnQuit = Button(self.controlFrame, text="Exit", command=frame.quit)
		self.btnQuit.grid(row=0, column=0)
		#test draw button
		self.btnTest = Button(self.controlFrame, text="Draw", command=self.draw)
		self.btnTest.grid(row=0, column=1)
		
		#get waveform button
		self.btnTest = Button(self.controlFrame, text="Get Waveform", command=self.get_waveform)
		self.btnTest.grid(row=0, column=2)
		
		#clear Button
		self.btnClear = Button(self.controlFrame, text="Clear", command=self.clear_canvas)
		self.btnClear.grid(row=0, column=3)
		
		#pack control Frame
		self.controlFrame.grid(row = 1, column = 0)
		
		#pack main frame
		self.mainFrame.grid(row = 0, column = 0)
		
		#sidebar frame
		self.sideBarFrame = Frame(frame)
		
		#equipement status labels
		#variables that can be updated to change the labels
		self.statusMessages = {
			"Scope" : StringVar(None,"Scope: Disconnected"), 
			"Func" : StringVar(None,"Func: Disconnected"), 
			"Isel" : StringVar(None,"Isel: Disconnected")
			}
		
		self.lblScope = Label(self.sideBarFrame, textvariable=self.statusMessages["Scope"])
		self.lblFunc = Label(self.sideBarFrame, textvariable=self.statusMessages["Func"])
		self.lblIsel = Label(self.sideBarFrame, textvariable=self.statusMessages["Isel"])
		
		#connect button for scope and func
		self.btnConnectRigol = Button(self.sideBarFrame, text="Connect to Rigol", command=self.rigol_connect)
		
		#connect button for isel
		self.btnConnectIsel = Button(self.sideBarFrame, text="Connect to Isel", command=self.isel_connect)
		
		self.lblScope.grid(row=0)
		self.lblFunc.grid(row=1)
		self.lblIsel.grid(row=2)
		
		self.btnConnectRigol.grid(row=3)
		self.btnConnectIsel.grid(row=4)
		
		self.sideBarFrame.grid(row = 0, column = 1)
		
	def draw(self):
		#all coords are flipped
		points = [512 - 0.01*i*i for i in range(600)]
		for i in range(len(points)-1):
			self.canvasItems.append(self.plotArea.create_line(i,points[i], i+1, points[i+1]))
			
	def get_waveform(self):
		self.clear_canvas()
		if (self.scope and self.func):
			self.func.set_output("ON")
			#todo single trigger
			result = self.scope.get_waveform()
			self.func.set_output("OFF")
			
			for i in range(len(result) - 1):
				self.canvasItems.append(self.plotArea.create_line(i,result[i], i+1, result[i+1]))
		else:
			tkMessageBox.showinfo(message="One or More Instruments Not Connected")
			
	
	def clear_canvas(self):
		self.plotArea.delete(ALL)
		self.canvasItems = []
		
	def rigol_connect(self):
		self.scope, self.func = instrument.find_instruments()
		if not (self.scope):
			tkMessageBox.showinfo(message="Couldn't Find Scope, Connect and Try Again")
			self.statusMessages["Scope"].set("Scope: Disconnected")
		else:
			self.statusMessages["Scope"].set("Scope: Connected")
			
		if not (self.func):
			tkMessageBox.showinfo(message="Couldn't Find Function Gen, Conect and Try Again")
			self.statusMessages["Func"].set("Func: Disconnected")
		else:
			self.statusMessages["Func"].set("Func: Connected")
	
	def isel_connect(self):
		#todo make this a variable
		self.IselController = isel.ISELController("/dev/tty.PL2303-00002006")
		
		if not (self.IselController.port):
			tkMessageBox.showinfo(message="Couldn't Find Isel Controller, Conect and Try Again")
			self.statusMessages["Isel"].set("Isel: Disconnected")
		else:
			self.statusMessages["Isel"].set("Isel: Connected")
		
if __name__ == "__main__":
	root = Tk()
	app = CNCControl(root)
	root.mainloop()