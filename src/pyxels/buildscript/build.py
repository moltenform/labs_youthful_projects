import sys

sys.path.append('..')
import os

os.chdir('..')

sys.path.append(r'C:\Python25\Lib\site-packages\PIL')

from distutils.core import setup
import py2exe

# enter the filename of your wxPython code file to compile ...
filename = "main.py"

# ... this creates the filename of your .exe file in the dist folder
distribution = 'Pythonpixels'

# if run without args, build executables
if len(sys.argv) == 1:
    sys.argv.append("py2exe")


class Target:
    def __init__(self, **kw):
        self.__dict__.update(kw)
        # for the versioninfo resources, edit to your needs
        self.version = "0.1"
        self.company_name = "Ben Fisher"
        self.copyright = "GPL"
        self.name = "Pythonpixels"


################################################################
# The manifest will be inserted as resource into your .exe.  This
# gives the controls the Windows XP appearance (if run on XP ;-)
#
# Another option would be to store it in a file named
# test_wx.exe.manifest, and copy it with the data_files option into
# the dist-dir.
#
manifest_template = '''
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<assembly xmlns="urn:schemas-microsoft-com:asm.v1" manifestVersion="1.0">
<assemblyIdentity
\tversion="5.0.0.0"
\tprocessorArchitecture="x86"
\tname="%(prog)s"
\ttype="win32"
/>
<description>%(prog)s Program</description>
<dependency>
\t<dependentAssembly>
\t\t<assemblyIdentity
\t\t\ttype="win32"
\t\t\tname="Microsoft.Windows.Common-Controls"
\t\t\tversion="6.0.0.0"
\t\t\tprocessorArchitecture="X86"
\t\t\tpublicKeyToken="6595b64144ccf1df"
\t\t\tlanguage="*"
\t\t/>
\t</dependentAssembly>
</dependency>
</assembly>
'''.replace("\r\n", "\n").replace("\n", "\r\n")

RT_MANIFEST = 24

# description is the versioninfo resource
# script is the wxPython code file
# manifest_template is the above XML code
# distribution will be the exe filename
# icon_resource is optional, remove any comment and give it an iconfile you have
#   otherwise a default icon is used
# dest_base will be the exe filename
test_wx = Target(
    description="Pythonpixels",
    script=filename,
    other_resources=[(RT_MANIFEST, 1, manifest_template % dict(prog=distribution))],
    dest_base=distribution
)

################################################################

setup(
    options={
        "py2exe": {
            "compressed": 1,
            "optimize": 2,
            "ascii": 1,
        }
    },
    zipfile=None,
    windows=[test_wx],
    py_modules=[
        'link_djoser',
        'pix_minitimer',
        'pix_parser',
        'pix_preview',
        # 'Image',
        # 'ImageTk',
    ],
    data_files=[
        #(directory, [filepath])
        ('im', ['im/test.png']),
        ('', ['readme.txt']),
    ],

    #Optional ones here
    author='Ben Fisher'
)
