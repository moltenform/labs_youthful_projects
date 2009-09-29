size = (3,3)

myMatrix = (
-1, -1, -1,
-1,  3, -1,
-1, -1, -1
)

imgOutput = imgInput.filter(ImageFilter.Kernel(size,myMatrix))

# 5x5 can also be used.
# http://www.pythonware.com/library/pil/handbook/imagefilter.htm