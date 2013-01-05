import wx
import wx.grid as gridlib

#This can be generalized by removing references to onlyEditableColumn
#This only handles rectangular selections, but understands it.

def handle_keyboard_events(sheet, evt, onlyEditableColumn):
	return True
	key = evt.GetKeyCode()
	if key == ord('C') and evt.ControlDown():
		if sheet.GetSelectedCells():
			print 'Non-rectangular selection, can\'t copy.'
		elif sheet.GetSelectionBlockTopLeft() and sheet.GetSelectionBlockBottomRight():
			#Rectangular selection
			top, left = sheet.GetSelectionBlockTopLeft()[0][0], sheet.GetSelectionBlockTopLeft()[0][1]
			bottom, right = sheet.GetSelectionBlockBottomRight()[0][0], sheet.GetSelectionBlockBottomRight()[0][1]
			alldata = []
			for row in range(top, bottom+1):
				coldata = []
				for col in range(left, right+1):
					#~ print sheet.GetCellValue(row,col)
					coldata.append(sheet.GetCellValue(row,col))
				alldata.append('\t'.join(coldata))
			result = '\r\n'.join(alldata)
			
			#Now set the clipboard to result
			set_clipboard(result)
		else:
			row,col = sheet.GetGridCursorRow(), sheet.GetGridCursorCol()
			result = sheet.GetCellValue(row,col)
			set_clipboard(result)
		
	elif key==ord('V') and evt.ControlDown():
		clip = get_clipboard().split('\r\n')
		clip = [line.split('\t') for line in clip]
		if len(clip)==1 and len(clip[0])==1:
			if sheet.GetGridCursorCol() == onlyEditableColumn:
				# Single guy, simply copy value into that cell.
				sheet.SetCellValue(sheet.GetGridCursorRow(), sheet.GetGridCursorCol(), clip[0][0])
				sheet.ClearSelection()
		
		elif sheet.GetSelectedCells():
			print 'Non-rectangular selection, can\'t paste.'
			return
		elif sheet.GetSelectionBlockTopLeft() and sheet.GetSelectionBlockBottomRight():
			print 'Select one cell to paste data.'
			return
			# This was not really worth it to implement.
			#The following code should be thoroughly tested before use, I'm not sure if it works:
			#~ top,left = sheet.GetSelectionBlockTopLeft()[0][0], sheet.GetSelectionBlockTopLeft()[0][1]
			#~ bottom,right = sheet.GetSelectionBlockBottomRight()[0][0], sheet.GetSelectionBlockBottomRight()[0][1]
			#~ print sheet.GetSelectionBlockTopLeft(), sheet.GetSelectionBlockBottomRight()
			#~ row,col = left, top
			
			#~ for x in range(len(clip)):
				#~ print (row+x), bottom
				#~ if (row+x)>bottom: continue
				#~ currentRow = clip[x]
				#~ for y in range(len(currentRow)):
					#~ print x,y
					#~ if (col+y)>right: continue
					#~ print 'set stuff'
					#~ sheet.SetCellValue(row+x,col+y, clip[x][y])
			
		else:
			# If only a single cell selected, replace other things in the way, but select everything to indicate this.
			row,col = sheet.GetGridCursorRow(), sheet.GetGridCursorCol()
			if col== onlyEditableColumn:
				maxcol = 0
				for x in range(len(clip)):
					currentRow = clip[x]
					if len(currentRow)>maxcol: maxcol = len(currentRow)-1
					for y in range(len(currentRow)):
						
							sheet.SetCellValue(row+x,col+y, clip[x][y])
				sheet.SelectBlock(row,col, row+len(clip)-1, col+maxcol)
			
	elif key==127:
		# user pressed Delete, clear the selected cells
		if sheet.GetSelectedCells():
			print 'Non-rectangular selection, can\'t clear.'
		elif sheet.GetSelectionBlockTopLeft() and sheet.GetSelectionBlockBottomRight():
			#Rectangular selection
			left, top = sheet.GetSelectionBlockTopLeft()[0][0], sheet.GetSelectionBlockTopLeft()[0][1]
			right, bottom = sheet.GetSelectionBlockBottomRight()[0][0], sheet.GetSelectionBlockBottomRight()[0][1]
			for row in range(left, right+1):
				for col in range(top, bottom+1):
					if col == onlyEditableColumn:
						sheet.SetCellValue(row,col,'')
		else:
			row,col = sheet.GetGridCursorRow(), sheet.GetGridCursorCol()
			if col == onlyEditableColumn:
				sheet.SetCellValue(row,col,'')
	else:
		#the event was not handled
		return True 



def set_clipboard(s):
	if wx.TheClipboard.Open():
		otext = wx.TextDataObject()
		otext.SetText(s)
		wx.TheClipboard.Clear()
		wx.TheClipboard.AddData(otext)
		wx.TheClipboard.Close()
		
def get_clipboard():
	s = None
	if wx.TheClipboard.Open():
		otext = wx.TextDataObject()
		wx.TheClipboard.GetData(otext)
		s = otext.GetText()
		wx.TheClipboard.Close()
	return s

if __name__=='__main__':
	class CellTestFrame(wx.Frame):
		def __init__(self, parent):
			wx.Frame.__init__(self, parent, -1, "RenameByCells", size=(640,480))
			self.grid = CopyGrid(self)
	
	gridapp = wx.PySimpleApp(0)
	main = CellTestFrame(None)
	gridapp.SetTopWindow(main)
	main.Show()
	gridapp.MainLoop()