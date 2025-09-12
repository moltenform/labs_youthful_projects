"""
Copyright 2005 Allen B. Downey

	This file contains wrapper classes.  It is
	mostly for my own use; I don't support it, and it is not very
	well documented.

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

class Callable(object):
	def __init__(self, func, *args, **kwds):
		self.func = func
		self.args = args
		self.kwds = kwds
	def __call__(self, event=None):
		return self.func(*self.args, **self.kwds)
	def __str__(self):
		return self.func.__name__

def makeThread(fn, args):
	import threading
	class MyThread(threading.Thread):
		"""this is a wrapper for threading.Thread that improves
		the syntax for creating and starting threads.
		"""
		def __init__(self, target, *args):
			threading.Thread.__init__(self, target=target, args=args)
			self.start()
	MyThread(fn, args)
