function R = getroughness_holtz(f1, f2, a1, a2)

fdiff = abs(f1-f2);
x = fdiff;
R = (10000.0 .*x.^2)./((33.^2+x.^2).^2);

end

