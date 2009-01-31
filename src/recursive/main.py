"""
Recursive Shapes 0.2
Ben Fisher, 2007

This program draws recursively-defined shapes. 
A blog post at Half Hour Hacks (halfhourhacks.blogspot.com) explains it further.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, see
http://www.gnu.org/licenses/gpl.html or write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
02110-1301 USA
"""

from Tkinter import *
from recursivemath import *
from recursiveshapes import *
import math
import wrappers

import ScrolledText
import tkFileDialog

class App:
	strLastSearch = None
	pxHeight = 400.
	pxWidth = 400.
	zoomFactor = 1.15
	def reset_dimensions(self):
		self.xMin = -1.
		self.xMax = 1.
		self.yMin = -1.
		self.yMax = 1.
		
	def __init__(self, root):
		self.inter = None
		root.title('Recursive Shapes')
		frTop = Frame(root)
		frTop.pack(side=TOP)
		
		frTopLeft = Frame(frTop)
		self.canvas = Canvas(frTopLeft, width=self.pxWidth+5, height=self.pxHeight+5, bg='white')
		self.canvas.pack(expand = YES, fill=BOTH)
		frTopLeft.pack(side=LEFT)

		frTopRight = Frame(frTop)
		frTopRightPanel1 = Frame(frTopRight)
		Button(frTopRightPanel1, text="Zoom In", command=self.zoom_in, padx=4).pack(side=LEFT)
		Button(frTopRightPanel1, text="Zoom Out", command=self.zoom_out, padx=4).pack(side=LEFT)
		Button(frTopRightPanel1, text="Reset", command=self.zoom_reset).pack(side=LEFT)
		frTopRightPanel1.pack(side=TOP)
		frTopRightPanel2 = Frame(frTopRight)
		Label(frTopRightPanel2, text="X").pack(side=LEFT)
		self.zoomPanelXbound = Entry(frTopRightPanel2, width=15)
		self.zoomPanelXbound.pack(side=LEFT)
		Label(frTopRightPanel2, text="Y").pack(side=LEFT)
		self.zoomPanelYbound = Entry(frTopRightPanel2, width=15)
		self.zoomPanelYbound.pack(side=LEFT)
		Button(frTopRightPanel2, text="Set", command=self.zoom_manual).pack(side=LEFT)
		frTopRightPanel2.pack(side=TOP)
		
		self.txtScript = Text(frTopRight, height=8, width=35, wrap=NONE)
		self.txtScript.pack(side=TOP)
		self.txtScript.bind('<Control-Key-a>', wrappers.Callable(self._select_all, self.txtScript))
		frTopRightPanel3 = Frame(frTopRight)
		Button(frTopRightPanel3, text="Draw", command=self.redraw).pack(side=LEFT)
		Button(frTopRightPanel3, text="Clear", command=self.clear).pack(side=LEFT)
		Button(frTopRightPanel3, text="Draw On", command=self.draw).pack(side=LEFT)
		frTopRightPanel3.pack(side=TOP)
		frTopRight.pack(side=LEFT)
		
		frBottomClass = Frame(root)
		Label(frBottomClass, text="Shape:").pack(side=LEFT)
		frBottomClass.pack(side=TOP)

		frBottom = Frame(root)
		self.txtClass = ScrolledText.ScrolledText( width=100, wrap=NONE)
		self.txtClass.pack(side=TOP, fill=BOTH, expand=True)
		self.txtClass.bind('<Control-Key-a>', wrappers.Callable(self._select_all, self.txtClass))
		frBottom.pack(side=TOP)

		# Dimensions code
		self.reset_dimensions()
		self._update_zoom_panel()
		
		self._create_menubar(root)
	
	# Dimensions and Sizing
	
	def _update_zoom_panel(self):
		self.zoomPanelXbound.delete(0, END)
		self.zoomPanelXbound.insert(0,str(self.xMin) + ',' + str(self.yMax))
		self.zoomPanelYbound.delete(0, END)
		self.zoomPanelYbound.insert(0,str(self.yMin) + ',' + str(self.yMax))
	
	def scale_points(self, p):
		x = p[0]
		y = p[1]
		return [((( x - self.xMin)/(self.xMax-self.xMin))*self.pxWidth) , 
		(self.pxHeight - ((y - self.yMin)/(self.yMax-self.yMin))*self.pxHeight)]
	
	def _zoom_change(self, factor):
		self.xMin = self.xMin/factor
		self.xMax = self.xMax/factor
		self.yMin = self.yMin/factor
		self.yMax = self.yMax/factor
		self._update_zoom_panel()
		self.redraw()
		
	def zoom_in(self):
		self._zoom_change(self.zoomFactor)
	def zoom_out(self):
		self._zoom_change(1/self.zoomFactor)
	def zoom_reset(self):
		self.reset_dimensions()
		self._update_zoom_panel()
		self.redraw()
	def zoom_manual(self):
		# Uses format -1,1
		x = self.zoomPanelXbound.get().split(',')
		y = self.zoomPanelYbound.get().split(',')
		self.xMin = float(x[0])
		self.yMin = float(y[0])
		self.xMax = float(x[1])
		self.yMax = float(y[1])
		self.redraw()
	
	# Drawing on the Canvas

	def redraw(self):
		"""Clear the canvas and redraw the shape."""
		self.clear()
		self.draw()
	def draw(self):
		"""Draw the shape without clearing the canvas."""
		source = self.txtClass.get(1.0, END) + '\n' + self.txtScript.get(1.0, END)
		self.inter.run_code(source, '<user-provided code>')
	def clear(self):
		"""Clear the canvas"""
		self.canvas.delete(ALL)
	
	def raw_draw_line(self, p1, p2, color='red'):
		"""Draw a line in pixel coordinates"""
		self.canvas.create_line(p1[0],p1[1],p2[0],p2[1], fill=color)
	
	def draw_line(self, p1, p2, color='red'):
		"""Draw a line and scale the coordinates"""
		self.raw_draw_line(self.scale_points(p1), self.scale_points(p2), color)
	
	def draw_lines(self, coords, complete=1, color='red'):
		"""Connect all points in the list by lines. By default also connects first and last point, to close the shape."""
		for i in range(len(coords) - 1):
			self.draw_line(coords[i], coords[i+1], color)
		if complete==1:
			self.draw_line(coords[-1], coords[0], color)
	
	def draw_curved_lines(self, coords, curve, complete=1, showcircles=False, color='red'):
		"""Connect all points in the list by curved lines."""
		for i in range(len(coords) - 1):
			self.draw_curved_line( coords[i], coords[i+1], curve, showcircles)
		if complete==1:
			self.draw_curved_line( coords[-1], coords[0], curve, showcircles)

	def draw_curved_line(self, p1, p2, curve, showcircles=False):
		"""Draw an arc of a circle between the two points, with a given "curve" factor. 
		As the curve approaches zero, it should draw a straight line.
		I came up with this, thinking it would be a good way to add variety to a shape.
		This is working well so far but I should add Beziers sometime."""
		p1 = self.scale_points(p1)
		p2 = self.scale_points(p2)
		p3 = extendpt( mid(p1, p2), getangle(p2,p1) + twopi/4., distance(p1,p2)*curve/2.)
		x1=p1[0]
		y1=p1[1]
		x2=p2[0]
		y2=p2[1]
		x3=p3[0]
		y3=p3[1]
		# Hack!!! if we are going to divide by zero, fudge the value a bit.
		ep = 0.0000001
		if abs(x2-x1)<ep: x1+=ep
		if abs(x3-x2)<ep: x2+=ep
		ma = (y2-y1)/(x2-x1)
		mb = (y3-y2)/(x3-x2)
		centerx = (ma*mb*(y1-y3) + mb*(x1+x2) - ma*(x2+x3))/(2.*(mb-ma))
		# Another similar hack. TODO: fix this!!
		if abs(ma)<ep: ma+=ep/10000.
		centery = -1*(centerx - (x1+x2)/2.)/ma +  (y1+y2)/2.
		
		radius = distance([centerx, centery], p1)
		sc =  360./twopi
		angle1 = getangle(p1, [centerx, centery]) * sc
		angle2 = getangle(p2, [centerx, centery]) * sc
		if angle2 < 0:
			angle2 += 360
		if showcircles:
			self.canvas.create_arc(centerx-radius, centery-radius, centerx+radius, centery+radius, start=0, extent=359, style=ARC)
		else:
			self.canvas.create_arc(centerx-radius, centery-radius, centerx+radius, centery+radius, start=180-angle1, extent=angle1-angle2, style=ARC)
	
	def draw_circle(self, p, radius):
		radiusX = radius*self.pxWidth/(self.xMax - self.xMin)
		radiusY = radius*self.pxHeight/(self.yMax - self.yMin)
		pxp = self.scale_points(p)
		self.canvas.create_oval(pxp[0] - radiusX, pxp[1]-radiusY, pxp[0] + radiusX, pxp[1]+radiusY)

	# Drawing Images: http://effbot.org/tkinterbook/photoimage.htm, http://www.daniweb.com/code/snippet296.html
	
	# Loading and saving shape classes
	
	def load_builtin_recursiveshapes(self):
		"""Open the source and make a list of the classes that can be created."""
		f = open('recursiveshapes.py','r')
		# For some reason this seems to open in binary, so strip \r
		acl = f.read().replace('\r\n','\n').split('\nclass ')[1:] #Do not keep code before the first class.
		f.close()
		baseClasses = ['RecShape', 'PaintShape']
		self.shapeClasses = []
		for cl in acl:
			className = cl.split('(',2)[0]
			if className not in baseClasses:
				# The example code in the script box, and the class will be put in the class box.
				classParts = cl.split('\n#$example$')
				if len(classParts) > 1:
					clCode, clExample = classParts[0], classParts[1].replace('#$ ','').replace('#$','')
				else:
					clCode, clExample = classParts, ''
				self.shapeClasses.append(( className, clCode, clExample))
				
	def load_builtin_shape(self, strClassName):
		"""Open and draw one of the builtin shape classes in recursiveshapes.py"""
		tClass = None
		for cl in self.shapeClasses:
			if cl[0]==strClassName: tClass = cl
		if tClass==None: return
		clCode = 'class ' + tClass[1].replace(strClassName, 'CurrentShape')
		clExample = tClass[2].replace(strClassName, 'CurrentShape')
		self.txtClass.delete('1.0', END)
		self.txtClass.insert(END, clCode)
		self.txtScript.delete('1.0', END)
		self.txtScript.insert(END, clExample)
		if self.inter:
			self.zoom_reset()
	
	def save_shape(self):
		"""Save current class and script into a file"""
		#See http://effbot.org/tkinterbook/tkinter-file-dialogs.htm
		strFileName = tkFileDialog.asksaveasfilename(defaultextension='.pyr', filetypes=[('Recursive scripts','.pyr')])
		if not strFileName: return
		try:
			f = open(strFileName,'w')
		except:
			print 'Could not open file.'
			return
		f.write(self.txtClass.get(1.0, END))
		f.write('\n#$$$$$#\n')
		f.write(self.txtScript.get(1.0, END))
		f.close()
	def open_shape(self):
		"""Load a file containing a class and script."""
		strFileName = tkFileDialog.askopenfilename(defaultextension='.pyr', filetypes=[('Recursive scripts','.pyr')])
		if not strFileName: return
		try:
			f = open(strFileName,'r')
		except:
			print 'Could not open file.'
			return
		astr = f.read().split('\n#$$$$$#\n')
		f.close()
		self.txtClass.delete('1.0', END)
		self.txtClass.insert(END, astr[0])
		self.txtScript.delete('1.0', END)
		self.txtScript.insert(END, astr[1])
		
		self.zoom_reset() #which draws and resets the zoom.
		
	def _select_all(self, txtField):
		"""Select all text in this field."""
		txtField.tag_add(SEL, '1.0', 'end-1c')
		txtField.mark_set(INSERT, '1.0')
		txtField.see(INSERT)
		return 'break'
	
	def _create_menubar(self,root):
		root.bind('<Control-Key-o>', wrappers.Callable(self.open_shape))
		root.bind('<Control-Key-s>', wrappers.Callable(self.save_shape))
		root.bind('<Control-Key-]>', wrappers.Callable(self.zoom_in))
		root.bind('<Control-Key-[>', wrappers.Callable(self.zoom_out))
		
		menubar = Menu(root)
		menuFile = Menu(menubar, tearoff=0)
		menuFile.add_command(label="Open", command=self.open_shape, underline=0, accelerator='Ctrl+O')
		menuFile.add_command(label="Save As", command=self.save_shape, underline=0, accelerator='Ctrl+S')
		menuFile.add_separator()
		menuFile.add_command(label="Exit", command=root.quit, underline=1)
		menubar.add_cascade(label="File", menu=menuFile, underline=0)
		
		menuView = Menu(menubar, tearoff=0)
		menuView.add_command(label="Zoom In", command=self.zoom_in, accelerator='Ctrl+]')
		menuView.add_command(label="Zoom Out", command=self.zoom_out, accelerator='Ctrl+[')
		menuView.add_command(label="Reset Zoom", command=self.zoom_reset)
		menubar.add_cascade(label="View", menu=menuView, underline=0)
		
		# Add shapes
		menuShape = Menu(menubar, tearoff=0)
		self.load_builtin_recursiveshapes()
		options = [t[0] for t in self.shapeClasses]
		for className in options:
			menuShape.add_command(label=className, command=wrappers.Callable(self.load_builtin_shape, className))
		menubar.add_cascade(label="Shapes", menu=menuShape, underline=0)
		
		menuHelp = Menu(menubar, tearoff=0)
		menuHelp.add_command(label="Interface Notes", command=self.help_interface)
		menuHelp.add_command(label="Engine Notes", command=self.help_engine)
		menuHelp.add_command(label="About", command=self.help_about)
		menubar.add_cascade(label="Help", menu=menuHelp, underline=0)
		root.config(menu=menubar)
		
		# Draw first shape?
		self.load_builtin_shape(options[0]) # But we can't yet, because the inter hasn't been set up.
		
	def help_about(self):
		self.clear()
		self.canvas.create_text(115,15,text='Recursive Shapes 0.2')
		self.canvas.create_text(115,35,text='Ben Fisher, 2007')
	def help_interface(self):
		strInterfaceNotes = """
		Recursive Shapes 0.2 Interface Help
		
		First, choose a shape class from the shapes menu. The class code will appear in the bottom field,
		and a short script to draw the shape will appear in a text field on the right. Click "draw" to draw.
		Click "draw on" to draw without erasing the previous image. Set the x and y bounds and click "set"
		to manually change the scale of the image.
		
		Your are encouraged to change the shape code. The changes will be updated as soon as you click Draw.
		You should also change the run script. The number passed to the draw() method is the number of iterations. 
		WARNING: Many iterations will, for most shapes, take far too long to compute. There are often also parameters
		that can be changed.
		
		One can create an animation by specifying a parameter to vary. Animations are made by changing a parameter slowly
		and redrawing at each frame. In the animate function you specify starting value, stop value, and number of frames, as 
		well as the number of iterations for each frame. The syntax is:
		shape.animate(nIterationsPerFrame, strParameter, start value, stop value, number of frames,time=400)
		
		Some shapes have examples. Uncomment the .animate line and press Draw to start the animation.
		It is not advised to zoom in or zoom out while playing an animation.
		"""
		self.txtClass.delete('1.0', END)
		self.txtClass.insert(END, strInterfaceNotes)
	def help_engine(self):
		strInterfaceNotes = """
		Recursive Shapes 0.2 Engine Notes
		
		Shapes derive from either RecShape or PaintShape. A RecShape instance is supposed to accumulate
		a list of points, which eventually will be connected. This class is most natural for some fractal curves.
		A PaintShape is expected to execute its own drawing code in the Draw method. This allows greater flexibility.
		
		The basic concept is that a shape provides an initial figure, or axiom, and a rule that draws lines and specifies 
		parts of the image that are similar to the entire image. I call each region of the shape that can be replaced with the
		entire shape an "active site." So, the axiom() method returns a list of "active sites," called parray. The rule() method takes this list 
		and replaces it with another list of "active sites." In the limit, each of the active sites is replaced with the entire image.
		
		The SquareSpiral is a good example of a simple case. It starts with a square. It then defines the next active site to be 
		a new square fitting inside of a square. So, the resulting picture is a spiral of squares inwards. In this case, we know 
		there will always be one active site at a time, so I don't actually loop over the parray. In a sense, each active site will eventually
		contain a scaled-down version of the entire image!
		
		In cases like the SerpTriangle, there are many active sites. Note that every iteration, the number increases by a factor of 3.
		
		However, it can be more complicated because an active site does not have to be an actual list of coordinates.
		For example, defining a circle as a point and radius may be more convinient, and this is possible. All that matters is that
		the format returned is the same format that gets passed in as a parameter.
		
		While working on a custom shape, what I find most helpful is sketching one iteration on paper and thinking about how to 
		get from one coordinate to the next.
		"""
		self.txtClass.delete('1.0', END)
		self.txtClass.insert(END, strInterfaceNotes)

root = Tk()
app = App(root)

#Initiate interpreter and draw the shape
app.inter = wrappers.Interpreter(app)
app.redraw()
root.mainloop()