
class Callable:
	def __init__(self, func, *args, **kwds):
		self.func = func
		self.args = args
		self.kwds = kwds
	def __call__(self, event=None):
		return apply(self.func, self.args, self.kwds)
	def __str__(self):
		return self.func.__name__

class NoteRenderInfo():
	def __init__(self, posy, sharpflat):
		self.posy = posy
		self.sharpflat = sharpflats
		
