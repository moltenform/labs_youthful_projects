import wx
import wx.grid as gridlib
import sheet_sheetclipboard

COL_FILENAME = 0
COL_NEWNAME = 1
COL_SIZE = 2
COLFIELDS = {0: 'filename', 1: 'newname', 2: 'size'}


class CellRenameGrid(gridlib.Grid):
	lastsortcol = -1
	lastsortdirection = False
	def __init__(self, parent, nRows, fnSortEvent):
		gridlib.Grid.__init__(self, parent, -1, size=wx.Size(550,200))
		self.fnSortEvent = fnSortEvent
		self.CreateGrid(nRows, len(COLFIELDS)) #i.e. (40 rows, 5 cols)
		
		self.SetRowLabelSize(23)
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
		
	def ensureRows(self, n):
		if n==self.GetNumberRows():
			# This will be the majority of cases
			return
		else:
			# delete the rows and recreate them. Not the most elegant. We'll refill them very soon at least.
			if (self.GetNumberRows() > 0):
				self.DeleteRows(0,self.GetNumberRows())
			self.InsertRows(0, n)
		
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
		# Ctrl+C for Copy, Ctrl+V for paste, should all be Excel-compatible
		sheet_sheetclipboard.handle_keyboard_events(self, evt, COL_NEWNAME)
		evt.Skip()
		

