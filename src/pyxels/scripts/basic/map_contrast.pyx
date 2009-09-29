#Simple contrast effect.
strength = 1.25
map:
	if r>128: R=int(r*strength)
	else: R=int(r/strength)
	
	if g>128: G=int(g*strength)
	else: G=int(g/strength)
	
	if b>128: B=int(b*strength)
	else: B=int(b/strength)
	


