
bool effect_checksame(CAudioData* w1, CAudioData* w2, bool lengthmatters);
double effect_interpolate(double* data, int length, double sampleIndex);

errormsg effect_mix(CAudioData**out, CAudioData* w1, CAudioData* w2, double s1, double s2);
errormsg effect_modulate(CAudioData**out, CAudioData* w1, CAudioData* w2, double a1, double a2);
errormsg effect_append(CAudioData**out, CAudioData* w1, CAudioData* w2);
errormsg effect_scale_pitch_duration(CAudioData**out, CAudioData* w1, double factor);
errormsg effect_vibrato(CAudioData**out, CAudioData* w1, double freq, double width);

//not exported:
//void effect_mix_impl(int length, double* out, double* d1,double* d2, double s1, double s2);
//void effect_modulate_impl(int length, double* out, double* d1,double* d2, double a1, double a2)
//void effect_scale_pitch_duration_impl(int newLength, double* out, int oldLength, double* d1, double factor)
//void effect_vibrato_impl(int length, double* out, double* d1, double vibratoFreqScale, double width)




