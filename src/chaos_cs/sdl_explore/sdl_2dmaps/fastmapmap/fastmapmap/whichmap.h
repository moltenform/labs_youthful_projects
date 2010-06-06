//fmod can be negative, so use this instead.
#define MMod(a,b) ((a>0)? fmod(a,b) : fmod(a,b)+b)

#define HENON x_ = 1 - c1*x*x + y; y_ = c2*x; 
#define HENONSUF "_henon"
#define BURGER x_ = c1*x - y*y; y_ = c2*y + x*y;
#define BURGERSUF "_b"
#define TKBELL x_ =  x*x - y*y + c1*x+c2*y; y_ = 2*x*y+2.0*x+0.5*y;
#define TKBELLALT x_ =  x*x - y*y + 0.9*x+-0.6013*y; y_ = 2*x*y+c1*x+c2*y;
#define TKBELLSUF "_tinkerbell"
#define IKEDA {double t=0.4-(6/(1+x*x+y*y)); x_ = 1+c1*(x*cos(t)-y*sin(t)); y_ = c2*(x*sin(t)+y*cos(t));};
#define IKEDASUF "_ikeda"
#define DUFFING x_ = y; y_ = -c2*x+c1*y - y*y*y;
#define DUFFINGSUF "_duffing"
#define GINGER x_ = 1 - c1*y + fabs(x); y_ = c2*x;
#define GINGERSUF "_ginger"

#define CHSTD x_ = MMod((x+c1*sin(y)),6.2831853); y_ =  MMod((y+ x_),6.2831853);
#define CHSTDSUF "_chstandard"
#define BEN1 x_ = MMod((x+c1*sin(y)),6.2831853); y_ =  MMod((c2*y+ x),6.2831853);
#define BEN1SUF "_ben1"
#define BEN2 x_ = MMod((x+c1*sin(y)),6.2831853); y_ =  MMod((y+ c2*x),6.2831853);
#define BEN2SUF "_ben2"

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
