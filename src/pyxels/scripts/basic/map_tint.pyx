#Add tint to image. map uses a precomputed table, so complicated expressions
#can be used with virtually no slow-down

targetColor = (255,128,0)
strength = 0.5
map:
	R=int( r*(1-strength) +targetColor[0]*(strength))
	G=int( g*(1-strength) +targetColor[1]*(strength))
	B=int( b*(1-strength) +targetColor[2]*(strength))


