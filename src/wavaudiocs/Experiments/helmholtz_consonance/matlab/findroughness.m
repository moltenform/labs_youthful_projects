
f1s = [440:0.1:880];
f2s = f1s.*0 + 440;

roughness = getroughness_holtz(f1s, f2s, 1, 1); % 1:1

roughness2 = getroughness_holtz(f1s, f2s*2, 1, 1); % 1:2

roughness3 = getroughness_holtz(f1s*2, f2s*3, 1, 1); % 2:3

roughness4 = getroughness_holtz(f1s*3, f2s*2, 1, 1); % 3:2


hold off
plot(f1s, roughness)
hold on
%plot(f1s, roughness2)
plot(f1s, roughness4)
%plot(f1s, roughness+ roughness2)

