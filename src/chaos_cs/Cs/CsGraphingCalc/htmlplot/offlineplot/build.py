tmplate = open('plottmplate.html').read()
widthsmall = str(256*1.5)
heightsmall = str(128*1.5)

for i in range(4):
	si = str(i+1)
	s = tmplate.replace('%%NUM%%', si)
	s = s.replace('%%WIDTH%%', widthsmall)
	s = s.replace('%%HEIGHT%%', heightsmall)
	assert s!=tmplate
	fout = open('plot'+si+'.html','w')
	fout.write(s)
	fout.close()
	
fout = open('plot_big.html','w')
s = tmplate.replace('%%NUM%%', '1')
s = s.replace('%%WIDTH%%',str(256*2.5))
s = s.replace('%%HEIGHT%%', str(128*2.5))
fout.write(s)
fout.close()
