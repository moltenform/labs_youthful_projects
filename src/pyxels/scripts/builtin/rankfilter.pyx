# The following are defined.
# MinFilter, MedianFilter, MaxFilter, ModeFilter
# RankFilter (more general)

imgOutput = imgInput.filter(ImageFilter.MinFilter(size=3))

# See http://www.pythonware.com/library/pil/handbook/imagefilter.htm