
import os
import wx
import wx.grid as gridlib
import wxutil

# This can be generalized by removing references to onlyEditableColumn
# This only handles rectangular selections, but understands it.
# Return True if handled, False if unhandled
def handle_keyboard_events(sheet, evt, dictEditableColumns):
    bHandledEvent = False
    key = evt.GetKeyCode()
    bOnlyCtrl = evt.ControlDown() and not evt.ShiftDown() and not evt.AltDown()
    
    # return early if no cell is selected
    if (not sheet.GetGridCursorRow() and sheet.GetGridCursorRow()!=0) or sheet.GetGridCursorRow()<0:
        return bHandledEvent
        
    if key == ord('C') and bOnlyCtrl:
        bHandledEvent = True
        if sheet.GetSelectedCells():
            # Non-contiguous selection (user ctrl-clicked some cells), not supported
            pass
        elif sheet.GetSelectionBlockTopLeft() and sheet.GetSelectionBlockBottomRight():
            # Rectangular selection
            top, left = sheet.GetSelectionBlockTopLeft()[0][0], sheet.GetSelectionBlockTopLeft()[0][1]
            bottom, right = sheet.GetSelectionBlockBottomRight()[0][0], sheet.GetSelectionBlockBottomRight()[0][1]
            alldata = []
            for row in range(top, bottom+1):
                coldata = []
                for col in range(left, right+1):
                    coldata.append(sheet.GetCellValue(row,col))
                alldata.append('\t'.join(coldata))
            
            #Now set the clipboard to result
            wxutil.setClipboardText(os.linesep.join(alldata))
        else:
            # Simply copy contents of current cell
            row,col = sheet.GetGridCursorRow(), sheet.GetGridCursorCol()
            result = sheet.GetCellValue(row,col)
            wxutil.setClipboardText(result)
        
    elif key==ord('V') and bOnlyCtrl:
        bHandledEvent = True
        clip = wxutil.getClipboardText()
        if not clip:
            return bHandledEvent
        clip = clip.replace('\r\n','\n').split('\n')
        
        # only paste into one column, easiest to test.
        if sheet.GetSelectedCells():
            # Non-contiguous selection (user ctrl-clicked some cells), not supported
            pass
        else:
            if sheet.GetSelectionBlockTopLeft() and sheet.GetSelectionBlockBottomRight():
                # rectangular selection. Just use the top / leftmost cell of the selection
                row,col = sheet.GetSelectionBlockTopLeft()[0][0], sheet.GetSelectionBlockTopLeft()[0][1]
            else:
                # if only a single cell selected, replace other things in the way, but select everything to indicate this.
                row,col = sheet.GetGridCursorRow(), sheet.GetGridCursorCol()
            
            if col in dictEditableColumns:
                # paste into the column, and then select what was pasted. truncate if there are too many rows
                for i in range(len(clip)):
                    rownumber = row+i
                    if rownumber >= sheet.GetNumberRows(): break
                    sheet.SetCellValue(rownumber,col, clip[i])
                sheet.SelectBlock(row,col, min(sheet.GetNumberRows()-1, row+len(clip)-1), col)
            
    return bHandledEvent

