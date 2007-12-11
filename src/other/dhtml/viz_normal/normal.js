// From surfstat, 

function Norm_p(z) { //Forward
// Returns the two-tailed standard normal probability of z
    z = Math.abs(z);
    var a1 = 0.0000053830, a2 = 0.0000488906, a3 = 0.0000380036,
        a4 = 0.0032776263, a5 = 0.0211410061, a6 = 0.0498673470;
    var p = (((((a1*z+a2)*z+a3)*z+a4)*z+a5)*z+a6)*z+1;
    p = Math.pow(p, -16);
    return p
}
 
function Norm_z(p) { //Inverse
// Returns z given a half-middle tail type p.
 
    var a0= 2.5066282,  a1=-18.6150006,  a2= 41.3911977,   a3=-25.4410605,
        b1=-8.4735109,  b2= 23.0833674,  b3=-21.0622410,   b4=  3.1308291,
        c0=-2.7871893,  c1= -2.2979648,  c2=  4.8501413,   c3=  2.3212128,
        d1= 3.5438892,  d2=  1.6370678, r, z;
 
    if (p>0.42) {
        r=Math.sqrt(-Math.log(0.5-p));
        z=(((c3*r+c2)*r+c1)*r+c0)/((d2*r+d1)*r+1)
    }
    else {
        r=p*p;
        z=p*(((a3*r+a2)*r+a1)*r+a0)/((((b4*r+b3)*r+b2)*r+b1)*r+1)
    }
    return z
}
 
 
function RoundDP(x, dp) {
// Rounds x to dp decimal places.
    var powten = Math.pow(10, dp);
    return (Math.round(x*powten)/powten)
}

function RoundSF(x, sf) {
// Rounds x to sf significant figures, but max 6 decimal places.
    if (x==0) return 0;
    with (Math) {
        var magnitude = ceil(log(abs(x))/LN10); // digits before decimal point
        var dp = max(0, min(6, sf-magnitude)); // no rounding left of the .
        return RoundDP(x, dp)
    }
}

function checkneg(p, tailType) {
// determines whether this p implies a negative deviate
    if (tailType=='left' & p<.5) return true;
    if (tailType=='right' & p>.5) return true;
    return false
}


function any2left(p, from, negative) {
// converts any tailType to 'left'

   if (from=='left') return p;
   if (from=='right') return (1-p); // no negative check for these

   if (from=='twotail') p = 1 - p/2
   else if (from=='middle') p = p/2 + 0.5
   else if (from=='half') p = p + 0.5;

   if (negative) p = 1-p;
   return p
}

function diddle(p, fromType, toType, negative) {
// converts p's tailType from fromType to toType via 'left'
// negative is logical, indicating a negative deviate: see checkneg
    var newp = any2left(p, fromType, negative);
    newp = left2any(newp, toType, negative);
    return newp
}

function left2any(p, to, negative) {
// converts p from tailType 'left' to any other

    if (to=='left') return p;
    if (to=='right') return (1-p);

    if (negative) p = 1-p; // corrects to p>0.5;

    if (to=='twotail') return 2*(1-p);
    if (to=='middle') return (2*p-1);
    if (to=='half') return (p-0.5)
}

//--------------END OF COMMON FUNCTIONS----------------------------------------

function forwardNormal(form) {
// Calculates p given z and tailtype.
    var normType = getTail(),
    z = form.z.value;
    if (!z | z=="NaN") form.p.value = ""
    else {
    z = eval(z); // make numeric
    var p = Norm_p(z); // returns twotail p
    p = diddle(p, 'twotail', normType, z<0);
    form.p.value = RoundDP(p, 4)
    }
}

function forwardNormal2(z)
{
    var normType = 'right';
    var p = Norm_p(z); // returns twotail p
    p = diddle(p, 'twotail', normType, z<0);
    return p;//RoundDP(p, 4)
}


function backwardNormal(form) {
// Beasley Springer approx. to inverse norm, Applied Stats. 26, 118-121.
// See: J.H.Maindonald "Statistical Computation" p.295.
    var p = eval(form.p.value);
    if (p<0 || p>1) form.z.value=""
    else {
        var normType = getTail();
        var negative = checkneg(p, normType);
        p = diddle(p, normType, 'half', negative);
        var z = Norm_z(p);
        if (negative) z=-z;
        form.z.value=RoundSF(z, 4)
    }
}

function backwardNormal2(p)
{
    var normType = 'right';
    var negative = checkneg(p, normType);
    p = diddle(p, normType, 'half', negative);
    var z = Norm_z(p);
    if (negative) z=-z;
    return z;
}