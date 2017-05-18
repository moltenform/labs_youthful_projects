'''PMIDI: Python MIDI library for Windows

The PMIDI library allows the generation of short MIDI sequences in Python code. The interface allows
a programmer to specify songs, instruments, measures, and notes. Playback is handled by the Windows 
MIDI stream API so proper playback timing is handled by the OS rather than by client code. The 
library is especially useful for generating earcons.
'''

classifiers = """\
Development Status :: 5 - Production/Stable
Intended Audience :: Developers
License :: OSI Approved :: MIT License
Programming Language :: Python
Topic :: Multimedia :: Sound/Audio :: MIDI
Topic :: Software Development :: Libraries :: Python Modules
Operating System :: Microsoft :: Windows
"""

from distutils.core import setup, Extension

doclines = __doc__.split('\n')
libs = ['winmm']

setup(name="PMIDI",
      version='1.1',
      author='Peter Parente',
      author_email='parente@cs.unc.edu',
      url='http://www.cs.unc.edu/~parente',
      download_url='http://www.sourceforge.net/projects/uncassist',
      license='http://www.opensource.org/licenses/mit-license.php',
      platforms=['Win32'],
      description = doclines[0],
      classifiers = filter(None, classifiers.split('\n')),
      long_description = ' '.join(doclines[2:]),
      packages = ['PMIDI'],
      package_dir = {'PMIDI' : ""},
      ext_modules = [Extension('PMIDI._cPMIDI', ['cPMIDI.i'], libraries=libs)],
      data_files = [('Lib/site-packages/PMIDI', ['LICENSE.txt']), 
                    ('Lib/site-packages/PMIDI', ['README.txt'])]
      )