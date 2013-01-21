
import os
import wx
import wx.grid as gridlib
import wxutil

# implement multi-cell copy and paste.
# pass in dictEditableColumns to specify which columns can be edited.
# currently only supports pasting into one column at a time.
# return True if handled, False if unhandled
def handle_keyboard_events(sheet, evt, dictEditableColumns):
    bHandledEvent = False
    key = evt.GetKeyCode()
    bOnlyCtrl = evt.ControlDown() and not evt.ShiftDown() and not evt.AltDown()
    
    # return early if no cell is selected
    if (not sheet.GetGridCursorRow() and sheet.GetGridCursorRow()!=0) or sheet.GetGridCursorRow()<0:
        return bHandledEvent
        
    if key==ord('C') and bOnlyCtrl:
        bHandledEvent = True
        if sheet.GetSelectedCells():
            # non-contiguous selection (user ctrl-clicked some cells), not supported
            pass
        elif sheet.GetSelectionBlockTopLeft() and sheet.GetSelectionBlockBottomRight():
            # rectangular selection
            top, left = sheet.GetSelectionBlockTopLeft()[0][0], sheet.GetSelectionBlockTopLeft()[0][1]
            bottom, right = sheet.GetSelectionBlockBottomRight()[0][0], sheet.GetSelectionBlockBottomRight()[0][1]
            allData = []
            for row in range(top, bottom+1):
                colData = []
                for col in range(left, right+1):
                    colData.append(sheet.GetCellValue(row,col))
                allData.append('\t'.join(colData))
            
            #now set the clipboard to result
            wxutil.setClipboardText(os.linesep.join(allData))
        else:
            # simply copy contents of current cell
            row, col = sheet.GetGridCursorRow(), sheet.GetGridCursorCol()
            result = sheet.GetCellValue(row,col)
            wxutil.setClipboardText(result)
        
    elif key==ord('V') and bOnlyCtrl:
        bHandledEvent = True
        sClipboard = wxutil.getClipboardText()
        if not sClipboard:
            return bHandledEvent
        aClipboard = sClipboard.replace('\r\n','\n').split('\n')
        
        # only paste into one column, easiest to test.
        if sheet.GetSelectedCells():
            # non-contiguous selection (user ctrl-clicked some cells), not supported
            pass
        else:
            if sheet.GetSelectionBlockTopLeft() and sheet.GetSelectionBlockBottomRight():
                # rectangular selection. Just use the top / leftmost cell of the selection
                row, col = sheet.GetSelectionBlockTopLeft()[0][0], sheet.GetSelectionBlockTopLeft()[0][1]
            else:
                # if only a single cell selected, replace other things in the way, but select everything to indicate this.
                row, col = sheet.GetGridCursorRow(), sheet.GetGridCursorCol()
            
            if col in dictEditableColumns:
                # paste into the column, and then select what was pasted. truncate if there are too many rows
                for i in range(len(aClipboard)):
                    nRow = row+i
                    if nRow >= sheet.GetNumberRows(): break
                    sheet.SetCellValue(nRow,col, aClipboard[i])
                sheet.SelectBlock(row, col, min(sheet.GetNumberRows()-1, row+len(aClipboard)-1), col)
            
    return bHandledEvent

