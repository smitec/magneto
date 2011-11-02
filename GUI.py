from Tkinter import *
import tkMessageBox
import iselControl
import RigolControl.instrument as instrument

class CNCControl:
	
	def __init__(self, master):
		frame = Frame(master)
		frame.pack()
		
		
		self.canvasItems = []
		
		#create canvas frame (for graphing I guess)
		self.graphFrame = Frame(frame, width=600, height=512, bd=1, relief=SUNKEN)
		self.plotArea = Canvas(self.graphFrame, width=600, height=512)
		self.plotArea.pack(side=LEFT)
		self.graphFrame.pack()
		
		#create controls frame
		self.controlFrame = Frame(frame, width=600)
		#quit Button
		self.btnQuit = Button(self.controlFrame, text="Exit", command=frame.quit)
		self.btnQuit.pack(side=LEFT)
		#test draw button
		self.btnTest = Button(self.controlFrame, text="Draw", command=self.draw)
		self.btnTest.pack(side=LEFT)
		#clear Button
		self.btnClear = Button(self.controlFrame, text="Clear", command=self.clear_canvas)
		self.btnClear.pack(side=LEFT)
		#connect button for scope and func
		self.btnConnect = Button(self.controlFrame, text="Connect to Rigol", command=self.rigol_connect)
		self.btnConnect.pack(side=LEFT)
		
		self.controlFrame.pack()
		
	def draw(self):
		#all coords are flipped
		points = [512 - 0.01*i*i for i in range(600)]
		for i in range(len(points)-1):
			self.canvasItems.append(self.plotArea.create_line(i,points[i], i+1, points[i+1]))
			
	def clear_canvas(self):
		 self.plotArea.delete(ALL)
		
	def rigol_connect(self):
		self.scope, self.func = instrument.find_instruments()
		if not (self.scope):
			tkMessageBox.showinfo(message="Couldn't Find Scope, Connect and Try Again")
			
		if not (self.func):
			tkMessageBox.showinfo(message="Couldn't Fing Function Gen, Conect and Try Again")
		
if __name__ == "__main__":
	root = Tk()
	app = CNCControl(root)
	root.mainloop()