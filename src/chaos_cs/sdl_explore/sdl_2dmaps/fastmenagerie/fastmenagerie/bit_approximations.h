//from http://www.codemaestro.com/reviews/9
//famous invSqrt hack
__inline float invSqrt(float xxx)
{
        float xhalf = 0.5f*xxx;
        union
        {
  	        float xxx;
                int i;
        } u;
        u.xxx = xxx;
        u.i = 0x5f3759df - (u.i >> 1);
        xxx = u.xxx * (1.5f - xhalf * u.xxx * u.xxx);
        return xxx;
}

//more fast square roots
//http://ilab.usc.edu/wiki/index.php/Fast_Square_Root#Method_using_Log_Base_2_Approximation


//http://en.wikipedia.org/wiki/Methods_of_computing_square_roots
//basically like a linear approx. between powers. 
__inline float wikipediaQuickLog(float v)
{
	int x = *(const int *) &v;
	float f = x;
	return f/(1<<23) - 127.0f;
}

//from http://graphics.stanford.edu/~seander/bithacks.html#IntegerLogFloat
static const char LogTable256[256] = 
{
#define LT(n) n, n, n, n, n, n, n, n, n, n, n, n, n, n, n, n
    -1, 0, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3,
    LT(4), LT(5), LT(5), LT(6), LT(6), LT(6), LT(6),
    LT(7), LT(7), LT(7), LT(7), LT(7), LT(7), LT(7), LT(7)
};

__inline int logBase2OfFloat(float v)
{
 // find int(log2(v)), where v > 0.0 && finite(v)
int c;                      // 32-bit int c gets the result;
int x = *(const int *) &v;  // OR, for portability:  memcpy(&x, &v, sizeof x);

c = x >> 23;          

if (c)
{
  c -= 127;
}
else
{ // subnormal, so recompute using mantissa: c = intlog2(x) - 149;
  register unsigned int t; // temporary
  // Note that LogTable256 was defined earlier
  if (t = x >> 16)
  {
    c = LogTable256[t] - 133;
  }
  else
  {
    c = (t = x >> 8) ? LogTable256[t] - 141 : LogTable256[x] - 149;
  }
}
return c;
}

//words here
//http://www.dspguru.com/dsp/tricks/quick-and-dirty-logarithms



//http://www.flipcode.com/archives/Fast_log_Function.shtml
//Submitted by Laurent de Soras
__inline float flipcodefast_log2 (float val)
{
   int * const    exp_ptr = reinterpret_cast <int *> (&val);
   int            x = *exp_ptr;
   const int      log_2 = ((x >> /* a guess that this is >> */ 23) & 255) - 128;
   x &= ~(255 << 23);
   x += 127 << 23;
   *exp_ptr = x;

   val = ((-1.0f/3) * val + 2) * val - 2.0f/3;   // (1)

   return (val + log_2);
} 


//http://www.icsi.berkeley.edu/cgi-bin/pubs/publication.pl?ID=002209
//published version
#define NBits 9

/* Creates the ICSILog lookup table. Must be called
once before any call to icsi_log().
n is the number of bits to be taken from the mantissa
(0<=n<=23)
lookup_table is a pointer to a floating point array
(memory has to be allocated by the user) of 2^n positions.
*/
void fill_icsi_log_table(const int n, float *lookup_table)
{
float numlog;
int *const exp_ptr = ((int*)&numlog);
int x = *exp_ptr; //x is the float treated as an integer
x = 0x3F800000; //set the exponent to 0 so numlog=1.0
*exp_ptr = x;
int incr = 1 << (23-n); //amount to increase the mantissa
int p=pow(2.0,n);
for(int i=0;i<p;++i)
{
lookup_table[i] = log(numlog)/log(2.0); //save the log value
x += incr;
*exp_ptr = x; //update the float value
}
}
/* Computes an approximation of log(val) quickly.
val is a IEEE 754 float value, must be >0.
lookup_table and n must be the same values as
provided to fill_icsi_table.
returns: log(val). No input checking performed.
*/
inline float icsi_log(register float val,
register const float *lookup_table, register const int n)
{
register int *const exp_ptr = ((int*)&val);
register int x = *exp_ptr; //x is treated as integer
register const int log_2 = ((x >> 23) & 255) - 127;//exponent
x &= 0x7FFFFF; //mantissa
x = x >> (23-n); //quantize mantissa
val = lookup_table[x]; //lookup precomputed value
return ((val + log_2)* 0.69314718); //natural logarithm
}


//fast pow() here
//http://www.hxa.name/articles/content/fast-pow-adjustable_hxa7241_2007.html
//http://www.stereopsis.com/sree/fpu2006.html

//fast int
//http://www.stereopsis.com/sree/fpu2006.html


//http://www.devmaster.net/forums/showthread.php?t=10153
//only valid if float is >=0 && < 256
unsigned char flt_to_byte (float a)
{
  float x = a + 256.0f;
  return ((*(int*)&x)&0x7fffff)>>15;
}

