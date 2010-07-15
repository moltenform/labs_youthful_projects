//fmod can be negative, so use this instead.
#define MMod(a,b) ((a>0)? fmod(a,b) : fmod(a,b)+b)

//henon 1 - c1*x*x + y; y_ = c2*x; 
//burger x_ = c1*x - y*y; y_ = c2*y + x*y;

#define MAPSUFFIX "_gen"

#define SAVESFOLDER ("saves" MAPSUFFIX  )
#define MAPDEFAULTFILE ("saves" MAPSUFFIX "/default.cfg" )

#define STRINGIFY2( x) #x
#define STRINGIFY(x) STRINGIFY2(x)
#define MAPEXPRESSIONTEXT STRINGIFY(MAPEXPRESSION)

