
# cellrename, by Ben Fisher. GPLv3.
# https://github.com/downpoured/labs_youthful_projects/tree/master/cellrename

import wx

class CellRenameComboBoxAsk(wx.Dialog):
    def __init__(self, parent, id, title, prompt, arChoices, sDefault=None):
        self.confirmedOK = False
        if not arChoices:
            arChoices = ['']
        
        if sDefault is None:
            sDefault = arChoices[0]
        
        wx.Dialog.__init__(self, parent, id, title)
        panel = wx.Panel(self, -1)
        vbox = wx.BoxSizer(wx.VERTICAL)
        hbox1 = wx.BoxSizer(wx.HORIZONTAL)
        hbox2 = wx.BoxSizer(wx.HORIZONTAL)
        hbox3 = wx.BoxSizer(wx.HORIZONTAL)
        
        # create controls
        txtPrompt = wx.StaticText(panel, -1, prompt)
        self.cmb = wx.ComboBox(panel, -1, size=(200, -1), choices=arChoices)
        self.cmb.SetValue(sDefault)
        self.btnOK = wx.Button(panel, wx.ID_OK) 
        self.btnOK.SetDefault()
        self.btnCancel = wx.Button(panel, wx.ID_CANCEL)

        # add controls to dialog
        hbox1.Add(txtPrompt, 1, wx.ALIGN_CENTRE)
        hbox2.Add(self.cmb, 1, wx.ALIGN_CENTRE)
        hbox3.Add(self.btnOK, 1, wx.RIGHT, 10)
        hbox3.Add(self.btnCancel, 1)
        vbox.Add((0, 1), 0)
        vbox.Add(hbox1, 0, wx.ALIGN_CENTRE)
        vbox.Add((0, 1), 0)
        vbox.Add(hbox2, 1, wx.ALIGN_CENTRE)
        vbox.Add(hbox3, 1, wx.ALIGN_CENTRE)
        panel.SetSizer(vbox)
        self.Centre()

def comboboxAsk(parent, id, title, prompt, arChoices, sDefault=None):
    dlg = CellRenameComboBoxAsk(parent, id, title, prompt, arChoices, sDefault)
    evt = dlg.ShowModal()
    val = dlg.cmb.GetValue()
    wasOk = evt == wx.ID_OK
    dlg.Destroy()
    return val if (wasOk and val is not None) else None

if __name__ == '__main__':
    class MyApp(wx.App):
        def OnInit(self):
            choices = ['aaa', 'bbb', 'ccc']
            val = comboboxAsk(None, -1, 'Test Choice Dialog', 'Please choose a name:', choices, 'deftext')
            print('you chose ' + str(val))
            return True

    app = MyApp(0)
    app.MainLoop()
