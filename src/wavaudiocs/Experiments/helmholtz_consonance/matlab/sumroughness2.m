basefreq = 400;
noctaves = 2; %the other octave displays the same pattern, given enough frequencies added

f1s = [basefreq-10:0.1:basefreq*noctaves+10];%[440:0.1:880];
f2s = f1s.*0 + basefreq;

roughness = f1s .* 0;

hold off
plot(0,0)
hold on
nharmonics = 6;
for uppernoteharmonic = 1:nharmonics
   for lowernoteharmonic =  1:nharmonics*2
       
        roughness = roughness + (1/uppernoteharmonic) * getroughness(f1s*uppernoteharmonic, f2s*lowernoteharmonic, 1, 1); 
   end
   plot(f1s, roughness)
end

hold on
plot(f1s, roughness)
axis([basefreq-10 basefreq*noctaves+10 0 10])
