
#define HENON x_ = 1 - c1*x*x + y; y_ = c2*x;
#define HENONSUF "_henon"
#define BURGER x_ = c1*x - y*y; y_ = c2*y + x*y;
#define BURGERSUF "_burger"
//#define CHSTD x_ = fmod((x+c1*sin(y)),1.0); y_ =  fmod((y+ x_),1.0);
//#define CHSTDSUF "_burger"

#define MAPEXPRESSION HENON
#define MAPSUFFIX HENONSUF


#define SAVESFOLDER ("saves" MAPSUFFIX  )
#define MAPDEFAULTFILE ("saves" MAPSUFFIX "/default.cfg" )

#define STRINGIFY2( x) #x
#define STRINGIFY(x) STRINGIFY2(x)
#define MAPEXPRESSIONTEXT STRINGIFY(MAPEXPRESSION)

