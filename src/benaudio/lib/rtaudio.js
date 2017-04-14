// htmlfreq, Ben Fisher, 2011

// This no longer works as of 2012 - use audiolet instead which works in chrome and firefox (albeit w different buffer sizes)

//based off of example at https://wiki.mozilla.org/Audio_Data_API
 function AudioDataDestination(sampleRate, readFn)
 {
	// Initialize the audio output.
	 if (!Audio) { errmsg('This program unfortunately requires using Firefox, because it uses Firefox\'s new real-time audio abilities.'); return false; }
	var audio = new Audio();
	 if (!audio.mozSetup) { errmsg('This program unfortunately requires using Firefox, because it uses Firefox\'s new real-time audio abilities.'); return false; }
	audio.mozSetup(1, sampleRate);

	var currentWritePosition = 0;
	var prebufferSize = sampleRate / 2; // buffer 500ms
	var tail = null, tailPosition;

	// The function called with regular interval to populate 
	// the audio output buffer.
	setInterval(function() {
	  var written;
	  // Check if some data was not written in previous attempts.
	  if(tail) {
	    written = audio.mozWriteAudio(tail.subarray(tailPosition));
	    currentWritePosition += written;
	    tailPosition += written;
	    if(tailPosition < tail.length) {
	      // Not all the data was written, saving the tail...
	      return; // ... and exit the function.
	    }
	    tail = null;
	  }

	  // Check if we need add some data to the audio output.
	  var currentPosition = audio.mozCurrentSampleOffset();
	  var available = currentPosition + prebufferSize - currentWritePosition;
	  if(available > 0) {
	    // Request some sound data from the callback function.
	    var soundData = new Float32Array(available);
	    readFn(soundData);

	    // Writting the data.
	    written = audio.mozWriteAudio(soundData);
	    if(written < soundData.length) {
	      // Not all the data was written, saving the tail.
	      tail = soundData;
	      tailPosition = written;
	    }
	    currentWritePosition += written;
	  }
	}, 100);
	return true;
}
