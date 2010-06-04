

REM ~ menag.exe out\out.dat d 128 64 -3.4 1.1 1.45 2.05  (a good burger's map)
REM ~ menag.exe out\out.dat d 128 128 -1 1.2 -1.1 1.1
menag.exe out\out.dat d 128 128 1.2 3 -1.1 0.7


REM ~ menag.exe out\out.dat d 128 128 -3.5 1.5 1.5 2
REM ~ menag.exe out\out.dat d 64 64 0 1.5 0.5 2
tofig.exe out\out.dat out\out.imdat
out\simpletobmp.exe o out\out.imdat out.bmp

