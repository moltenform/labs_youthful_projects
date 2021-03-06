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
import recursiveshapes
class Interpreter(object):
    """this object encapsulates the environment where user-provided
    code will execute
    """
    def __init__(self, app):
        self.locals = recursiveshapes.getglobals()
        # make sure the environment contains a reference to the app
        self.locals['app'] = app

    def run_code_thread(self, *args):
        """run the given code in a new thread"""
        MyThread(self.run_code, *args)
        
    def run_code(self, source, filename):
        """run the given code in the saved environment"""
        code = compile(source, filename, 'exec')
        try:
            exec(code, self.locals)
        except KeyboardInterrupt:
            self.world.quit()

class Callable(object):
    def __init__(self, func, *args, **kwds):
        self.func = func
        self.args = args
        self.kwds = kwds
        self.__name__ = func.__name__
    def __call__(self, event=None):
        return self.func(*self.args, **self.kwds)
    def __str__(self):
        return self.func.__name__

