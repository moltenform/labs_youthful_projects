tmplate = open('plottmplate.html').read()
for i in range(4):
	si = str(i)
	s = tmplate.replace('%%NUM%%', si)
	assert s!=tmplate
	fout = open('plot'+si+'.html')
	fout.write(s)
	fout.close()
	