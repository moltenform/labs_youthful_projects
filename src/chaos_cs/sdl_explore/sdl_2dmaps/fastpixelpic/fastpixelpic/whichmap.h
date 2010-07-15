//fmod can be negative, so use this instead.
#define MMod(a,b) ((a>0)? fmod(a,b) : fmod(a,b)+b)

#define HENON x_ = 1 - c1*x*x + y; y_ = c2*x; 
#define HENONSUF "_henon"
#define BURGER x_ = c1*x - y*y; y_ = c2*y + x*y;
#define BURGERSUF "_b"

#define MAPEXPRESSION BURGER
#define MAPSUFFIX BURGERSUF
#define DRAWPERIODIC 0

#define SAVESFOLDER ("saves" MAPSUFFIX  )
#define MAPDEFAULTFILE ("saves" MAPSUFFIX "/default.cfg" )

#define STRINGIFY2( x) #x
#define STRINGIFY(x) STRINGIFY2(x)
#define MAPEXPRESSIONTEXT STRINGIFY(MAPEXPRESSION)

#define CountPixelsSettle 160
#define CountPixelsDraw 80
#define CountPixelsSeedsPerAxis 10
