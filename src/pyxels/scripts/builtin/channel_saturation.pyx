#Color saturation effect, similar to increasing s in HSL 
eh = ImageEnhance.Color(imgInput)
imgOutput = eh.enhance(1.4)

#Or, to make it grayscale,
#imgOutput = ImageOps.grayscale(imgInput)