scaleFactor = 0.5
newWidth = int(width*scaleFactor)
newHeight = int(height*scaleFactor)

imgOutput = imgInput.resize((newWidth,newHeight), Image.ANTIALIAS)
#Other methods are NEAREST, BILINEAR, BICUBIC, or ANTIALIAS 
