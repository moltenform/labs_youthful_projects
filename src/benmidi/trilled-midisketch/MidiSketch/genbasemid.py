#see readme.txt, bbuilder.py, and the online documentation for more examples.

from bmidilib import bbuilder
b = bbuilder.BMidiBuilder()
b.rest(4999)
b.note('a0', 1)

b.save('out.mid')

