angle=25

imgOutput = imgInput.rotate(angle, Image.BICUBIC, expand=0)
#Other methods are NEAREST, BILINEAR, BICUBIC 
# set expand to 1: output image should be made large enough to hold the rotated image.

#To flip,
#imgOutput = imgInput.transpose(Image.FLIP_LEFT_RIGHT)
#other options: FLIP_LEFT_RIGHT,FLIP_TOP_BOTTOM,ROTATE_90,ROTATE_180,ROTATE_270
