
import wx
import wx.grid as gridlib
import cellrename_gridclipboard
import sys
import unittests
import wxutil

COL_FILENAME = 0
COL_NEWNAME = 1
COL_SIZE = 2
COLFIELDS = {COL_FILENAME: 'filename', COL_NEWNAME: 'newname', COL_SIZE: 'size'}

class CellRenameGrid(gridlib.Grid):
	lastsortcol = -1
	lastsortdirection = False
	def __init__(self, parent, nRows, fnSortEvent):
		gridlib.Grid.__init__(self, parent, -1, size=wx.Size(550,200))
		self.fnSortEvent = fnSortEvent
		self.CreateGrid(nRows, len(COLFIELDS)) #i.e. (40 rows, 5 cols)
		
		self.SetRowLabelSize(22)
		self.SetColLabelValue(COL_FILENAME, "Filename")
		self.SetColLabelValue(COL_NEWNAME, "New name")
		self.SetColLabelValue(COL_SIZE, "Size")
		
		#Set size
		self.SetColSize(0, 175)
		self.SetColSize(1, 175)
		
		# Make cols read-only
		attrReadonly = gridlib.GridCellAttr()
		attrReadonly.SetReadOnly()
		for c in (COL_FILENAME,COL_SIZE):
			self.SetColAttr(c, attrReadonly)
			
		self.EnableDragRowSize(False)
			
		#Events
		self.Bind(gridlib.EVT_GRID_LABEL_LEFT_CLICK, self.OnLabelLeftClick)
		self.GetGridWindow().Bind(wx.EVT_KEY_DOWN, self.OnKeyDown)
		
	def OnLabelLeftClick(self, evt):
		col = evt.GetCol()
		if col==self.lastsortcol:
			bDirection = not self.lastsortdirection
		else:
			bDirection = False
		self.fnSortEvent(COLFIELDS[col], bDirection)
		self.lastsortcol = col
		self.lastsortdirection = bDirection
		
	def OnKeyDown(self,evt):
		if evt.GetKeyCode()==ord('U') and evt.ControlDown() and evt.ShiftDown():
			wxutil.alertDialog(self, 'running unit tests')
			try: unittests.runall()
			except: wxutil.alertDialog(self, 'e:'+str(sys.exc_info()[0]))
			else: wxutil.alertDialog(self, 'all tests pass')
			
		# Ctrl+C for Copy, Ctrl+V for paste
		dictEditableColumns = {COL_NEWNAME: 1}
		ret = cellrename_gridclipboard.handle_keyboard_events(self, evt, dictEditableColumns)
		if not ret:
			evt.Skip() #if we didn't handle the event, pass it upwards
		
		
	def writeToModel(self, obj):
		for i in range(len(obj.data)):
			elem = obj.data[i]
			elem.newname = self.GetCellValue(i, COL_NEWNAME)
	
	def loadFromModel(self, obj):
		if len(obj.data)!=self.GetNumberRows():
			# delete the rows and recreate them.
			if (self.GetNumberRows() > 0):
				self.DeleteRows(0,self.GetNumberRows())
			if (len(obj.data)>0):
				self.InsertRows(0, len(obj.data))
			
		for i in range(len(obj.data)):
			elem = obj.data[i]
			self.SetCellValue(i, COL_FILENAME, elem.filename)
			self.SetCellValue(i, COL_NEWNAME, elem.newname)
			self.SetCellValue(i, COL_SIZE, elem.sizeRendered)
		
		

