try:
    from Tkinter import *
    from Tkinter import _cnfmerge
except ImportError:
    from tkinter import *
    from tkinter import _cnfmerge


class ListViewWindow(object):
    def __init__(self, top, tracknumber, trackdata, opts):
        top.title('Track %d Events' % tracknumber)
        frameTop = Frame(top, padx='0m')
        frameTop.pack(expand=YES, fill=BOTH)

        #perhaps use a monospace font
        self.lb = ScrolledListbox(frameTop, selectmode=SINGLE, width=60, height=30)
        self.lb.pack(expand=YES, fill=BOTH)

        #Insert the data. Actually, just use Notelist, a lot easier
        superlist = [
            evt for evt in trackdata.events if evt.type != 'NOTE_ON' and evt.type != 'NOTE_OFF'
        ]
        superlist.extend(trackdata.notelist)
        superlist.sort(key=lambda item: item.time)

        for item in superlist:
            self.lb.insert(
                END,
                item.__repr__().replace('\r', '').replace('\n', '').replace('\t', '    ')
            )

        top.bind('<MouseWheel>', self.scroll) #binding for Windows
        top.bind('<Button-4>', self.scroll) #binding for Linux
        top.bind('<Button-5>', self.scroll)

    def scroll(self, event):
        if event.num == 5 or event.delta == -120:
            self.lb.yview_scroll(5, 'units')
        if event.num == 4 or event.delta == 120:
            self.lb.yview_scroll(-5, 'units')


class ScrolledListbox(Listbox): #an imitation of ScrolledText
    def __init__(self, master=None, cnf=None, **kw):
        if cnf is None:
            cnf = {}
        if kw:
            cnf = _cnfmerge((cnf, kw))
        fcnf = {}
        for k in list(cnf.keys()):
            if type(k) == ClassType or k == 'name':
                fcnf[k] = cnf[k]
                del cnf[k]
        self.frame = Frame(master, **fcnf)
        self.vbar = Scrollbar(self.frame, name='vbar')
        self.vbar.pack(side=RIGHT, fill=Y)
        cnf['name'] = 'lbox'
        Listbox.__init__(self, self.frame, **cnf)
        self.pack(side=LEFT, fill=BOTH, expand=1)
        self['yscrollcommand'] = self.vbar.set
        self.vbar['command'] = self.yview

        # Copy geometry methods of self.frame -- hack!
        methods = list(Pack.__dict__.keys())
        methods = methods + list(Grid.__dict__.keys())
        methods = methods + list(Place.__dict__.keys())

        for m in methods:
            if m[0] != '_' and m != 'config' and m != 'configure':
                setattr(self, m, getattr(self.frame, m))


if __name__ == '__main__':
    import sys
    sys.path.append('..')
    from bmidilib import bmidilib

    class TestApp(object):
        def __init__(self, root):
            root.title('Testing list view')
            Button(root, text='open', command=self.openit).pack()

        def openit(self):
            file = bmidilib.BMidiFile()
            file.open('..\\midis\\bossa.mid')
            file.read()
            file.close()
            trackfile = file.tracks[3]

            opts = {}
            #~ opts['showonlynotes'] = False

            top = Toplevel()
            window = ListViewWindow(top, 5, trackfile, opts)

    root = Tk()
    app = TestApp(root)
    root.mainloop()
