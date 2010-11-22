


errormsg synth_sin(CAudioData**out, double freq, double length, double amp);
errormsg synth_square(CAudioData**out, double freq, double length, double amp);
errormsg synth_sawtooth(CAudioData**out, double freq, double length, double amp);
errormsg synth_triangle(CAudioData**out, double freq, double length, double amp);
errormsg synth_circle(CAudioData**out, double freq, double length, double amp);
errormsg synth_square_change(CAudioData**out, double freq, double length, double amp);
errormsg synth_whitenoise(CAudioData**out, double length, double amp);

errormsg synth_pinknoise(CAudioData**out, double length, double amp);
errormsg synth_rednoise(CAudioData**out, double length, double amp);

errormsg synth_redglitch(CAudioData**out,double freq, double lengthSeconds, double amp, double chunkLength, double rednoisefactor);


