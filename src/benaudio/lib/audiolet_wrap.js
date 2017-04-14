
// the code I wrote in 2011 expects to be filling a buffer,
// but audiolet apparently expects one sample at a time (see Sine class)
// this wrapper makes audiolet happy.

var WrapperNode = function(audiolet, fncallback) {
	AudioletNode.call(this, audiolet, 1, 1);
	this.nopparam = new AudioletParameter(this, 0, 5);
	this.fakebuffer = [0.0]; // allocate once and reuse
	this.fncallback = fncallback;
};
extend(WrapperNode, AudioletNode);

WrapperNode.prototype.generate = function() {
	var output = this.outputs[0];
	this.fncallback(this.fakebuffer);
	output.samples[0] = this.fakebuffer[0];
};

WrapperNode.prototype.toString = function() {
	return 'WrapperNode';
};

