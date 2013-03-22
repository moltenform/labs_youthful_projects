
a=5.0;
b=9.0;
% a:b

f1s = [440:0.1:880];
f2s = f1s.*0 + 440;

roughness = getroughness_holtz(f1s*a, f2s*b, 1, 1); 


hold off
plot(f1s, roughness)
axis([400 900 0 5])
