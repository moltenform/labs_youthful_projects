from link_djoser import *
import ImageTk

class PreviewImage(DjApplicationSubwindow):
	def __init__(self, strName, imagePIL):
		self.name = strName		
		self.imtk = ImageTk.PhotoImage(imagePIL)
		
		DjApplicationSubwindow.__init__(self)
		
	def layout(self,window):
		window.title = self.name
		
		self.imControl = window.image()
		self.imControl.image = self.imtk
		
		window.add_all()
		window.set_style('tool')
		window.set_resizeable(False,False)
		window.onclose = lambda: False #prevent closing
		
	def setImage(self, imagePIL):
		imtk = ImageTk.PhotoImage(imagePIL)
		self.imControl.image = imtk

if __name__=='__main__':
	def testPreview():
		import Image
		impil = Image.open('im/test.png')
		#~ imtk = ImageTk.PhotoImage(impil)
		
		p = PreviewImage('Test',impil)
		
	class TestApp(DjApplication):
		def layout(self, window):
			window.button('Click',testPreview)
			window.add_all()
	
	
	p = TestApp()
	p.run()