
function $(s)
{
    return document.getElementById(s)
}

errmsg = function(s)
{
    alert(s)
}
alerdbg = function(s)
{
    alert(debugprint(s))
}

//found online
function debugprint(obj, maxDepth, prefix){
   var result = '';
   if (!prefix) prefix='';
   for(var key in obj){
       if (typeof obj[key] == 'object'){
           if (maxDepth !== undefined && maxDepth <= 1){
               result += (prefix + key + '=object [max depth reached]\n');
           } else {
               result += debugprint(obj[key], (maxDepth) ? maxDepth - 1: maxDepth, prefix + key + '.');
           }
       } else {
           result += (prefix + key + '=' + obj[key] + '\n');
       }
   }
   return result;
}

String.prototype.startsWith = function(s)
{
return (this.indexOf(s)===0);
}

//consider making a pool object?


Time = {};
Time.createTimer = function() 
{
    return new _Timer();
}
_Timer = function() //intended to be constructed with new.
{
    this.dt = new Date();
}
_Timer.prototype.check = function()
{
    var n = new Date();
    var nDiff = n.getTime() - this.dt.getTime();
    return nDiff;
}