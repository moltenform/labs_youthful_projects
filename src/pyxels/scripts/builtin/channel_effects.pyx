eh = ImageEnhance.Brightness(imgInput)
imgOutput = eh.enhance(1.4)

#eh = ImageEnhance.Contrast(imgInput)
#eh = ImageEnhance.Sharpness(imgInput)

# http://www.pythonware.com/library/pil/handbook/imageenhance.htm

#imgOutput = ImageOps.solarize(imgInput, threshold=128)