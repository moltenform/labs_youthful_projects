
a=5.0;
b=9.0;
% a:b

f1s = [420:0.1:1300];%[440:0.1:880];
f2s = f1s.*0 + 440;

roughness = f1s .* 0;

hold off
plot(0,0)
hold on
for uppernoteharmonic = 1:6
   for lowernoteharmonic =  1:12; %uppernoteharmonic:uppernoteharmonic*2
       
        roughness = roughness + (1/uppernoteharmonic) * getroughness(f1s*uppernoteharmonic, f2s*lowernoteharmonic, 1, 1); 
   end
   plot(f1s, roughness)
end

hold on
plot(f1s, roughness)
axis([400 1300 0 10])
