# Ben Fisher

from spline import *

targetfile = 'gradcoollinear.ugr'
outputfile = 'colortable_1024.h'


NPOINTS  = 1024
bComponents = False
def main():
    f=open(targetfile,'r')
    alltxt = f.read()
    thelist = alltxt.split()
    f.close()
    
    parts = len(alltxt.split('{'))
    if parts==1:
        print 'Probably not a valid gradient, no { found'
        return
    elif parts > 2:
        print 'More than one gradient in file... we don\'t support this'
        return
    
    i=0
    for i in xrange(len(thelist)):
        if thelist[i].startswith('index='): break
        i+=1
    
    xpoints = []; rpoints = []; gpoints=[]; bpoints=[]
    while True:
        if thelist[i].startswith('index='):
            
            xpoints.append(parsevalue(thelist[i]))
            print '@', xpoints[-1]
        elif thelist[i].startswith('color='):
            r,g,b = parsecolor(parsevalue(thelist[i]))
            rpoints.append(r); gpoints.append(g); bpoints.append(b); 
            print '@@@r=%d\tg=%d\tb=%d' % (rpoints[-1],gpoints[-1],bpoints[-1])
        else:
            break
        i+=1
        
    #the gradients wrap around. we can mimick this by making a copy of the last point that is negative, and a copy of the first point that is beyond 400.
    xpoints.insert(0 , xpoints[-1]-400); xpoints.append(xpoints[1]+400)
    rpoints.insert(0 , rpoints[-1]); rpoints.append(rpoints[1])
    gpoints.insert(0 , gpoints[-1]); gpoints.append(gpoints[1])
    bpoints.insert(0 , bpoints[-1]); bpoints.append(bpoints[1])
    
    #make interpolation objects
    R = LinInt(xpoints, rpoints) #use Spline or LinInt for linear interp
    G = LinInt(xpoints, gpoints)
    B = LinInt(xpoints, bpoints)
    
    print xpoints
    print rpoints
    print gpoints
    print bpoints

    #write results
    f= open(outputfile,'w')
    if bComponents:
        output(f, R, 'cmapr')
        output(f, G, 'cmapg')
        output(f, B, 'cmapb')
    else:
        output32bits(f, R,G,B)
    
    f.close()

def packinto32bits(r,g,b):
    # note that windows sometimes uses BGR
    assert r>=0 and r<=255 and g>=0 and g<=255 and b>=0 and b<=255 
    rbits = r
    gbits = g<<8
    bbits = b<<16
    return bbits + gbits  + rbits
    

def output32bits(f, R,G,B):
    f.write('unsigned int color32bit[] = {')	
    for i in range(NPOINTS):
        x = (float(i)/NPOINTS)*400
        s = '0x%08x, ' % packinto32bits(boundcheck(int(R(x))),boundcheck(int(G(x))),boundcheck(int(B(x))))
        f.write(s)
        if i%20==0: f.write('\n')
        
    f.write( '0};\n')

def parsevalue(str):
    s = str.split("=")
    return int(s[1])

def parsecolor(n):
    r = 0xff & n
    g = (0x00ff00 & n) >> 8
    b = (0xff0000 & n) >> 16
    return ((float(r),float(g),float(b)))

def boundcheck(n):
    if n<0: return 0
    if n>=255: return 255
    return n

def output(f,  data,varname):
    f.write('unsigned char '+varname+'[] = {')
    for i in range(NPOINTS):
        x = (float(i)/NPOINTS)*400
        f.write(str( boundcheck( int(data(x)) )))
        f.write(',')
    f.write( '0};\n')

def packinto16bits(r,g,b):
        #5 bits R, 6 bits G, 5 bits B
        rbits = (r & 0xf8) >> 3
        gbits = (g & 0xfc) >> 2
        bbits = (b & 0xf8) >> 3
        print 'r',bstr_pos(rbits),'g',bstr_pos(gbits),'b',bstr_pos(bbits)
        return bbits + (gbits << 5) + (rbits << 11)

if __name__ == '__main__':
    bstr_pos = lambda n: n>0 and bstr_pos(n>>1)+str(n&1) or ''
    
    main()
    #~ print '%x' % (int('0000011111100000',2))
    
    
    
    #~ r=int('11111000',2)
    #~ g=int('01001000',2)
    #~ b=int('11111000',2)
    
    
        # rrrrrggggggbbbbb
    #~ print bstr_pos(packinto16bits(r,g,b))
    
    #~ print '%x' % (int('11111',2))
    #~ a = int('11001001',2)
    
    #~ top5a = (a & 0xf8) >> 3  #want 11001
    #~ top6a = (a & 0xfc) >> 2  #want 11001
    
    #~ bstr_pos = lambda n: n>0 and bstr_pos(n>>1)+str(n&1) or ''
    #~ print bstr_pos(top5a)
    #~ print bstr_pos(top6a)
    



