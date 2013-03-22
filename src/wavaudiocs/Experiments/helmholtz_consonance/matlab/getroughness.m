function R = getroughness(f1, f2, a1, a2)
%http://www.acousticslab.org/learnmoresra/moremodel.html
%a1=1.0;
%a2=1.0;

scale = 26; %normalize

fmin=min(f1,f2);
fmax=max(f1,f2);
amin=min(a1,a2);
amax=max(a1,a2);

b1 = 3.5;  b2 = 5.75;  s1 = 0.0207;  s2 = 18.96; 
s = 0.24./(s1.*fmin + s2);

X = amin.*amax;
Y = 2*amin./(amin+amax);
Z = exp(-b1.*s.*(fmax-fmin)) - exp(-b2.*s.*(fmax-fmin));
R = X.^(0.1) .* 0.5 .* Y.^(3.11) .* Z .* scale;
%fn = @(x) (lambda.*x.*x)./((30+x.^2).^2);


end

