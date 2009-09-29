stat = ImageStat.Stat(rim)

print 'Stats for red channel:'
stats={'# of pixels':stat.count, 
'Average pixel level':stat.mean,
'Median pixel level': stat.median,
'Variance':stat.var}
for k,v in stats.iteritems():
	print k +' '+str(v)

#http://www.pythonware.com/library/pil/handbook/imagestat.htm