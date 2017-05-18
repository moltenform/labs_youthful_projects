%module cPMIDI
%include typemaps.i

%{
  //#define _WIN32_WINNT 0x400
  #include "windows.h"
  
  //PyInterpreterState *save_interp = NULL;
%}

%init %{
  //save_interp = PyThreadState_Get()->interp;
%}

%typemap(python, in) PyObject *pytuple {
  if (!PyTuple_Check($input)) {
    PyErr_SetString(PyExc_TypeError, "need a tuple object");
    return NULL;
  }
  $1 = $input;
}

%wrapper %{
/*
  void CALLBACK midiCallback(HMIDIOUT stream, UINT uMsg, DWORD dwInstance, DWORD dwParam1, DWORD dwParam2) {
    LPMIDIHDR header = (LPMIDIHDR)dwParam1;
    unsigned long err;
    
    switch (uMsg) {
    case MOM_DONE:
      printf("done\n");
      // get the header of the stream that finished
      header = (LPMIDIHDR)dwParam1;
    
      // unprepare the header
      err = midiOutUnprepareHeader(stream, header, sizeof(MIDIHDR));
            
      if (err != MMSYSERR_NOERROR) {
        PyErr_SetString(PyExc_WindowsError, "could not unprepare header");
        break;
      }
      
      // free the data memory
      free(header->lpData);  
      printf("freed");
      
      break;
          
    case MOM_POSITIONCB:
      printf("position\n");
      printf("%d %d", MHDR_INQUEUE, header->dwFlags);
      break;
    case MOM_CLOSE:
      printf("close\n");
      break;
    case MOM_OPEN:
      printf("open\n");
      break;
    }
  }
*/

  static MIDIHDR *header = NULL;

  void RestartMIDIStream(int stream) {
    unsigned long err;
    
    // try to restart the stream
    Py_BEGIN_ALLOW_THREADS
    err = midiStreamRestart((HMIDISTRM) stream);
    Py_END_ALLOW_THREADS
    
    if (err != MMSYSERR_NOERROR) {
      PyErr_SetString(PyExc_WindowsError, "could not (re)start stream");
    }
  }
  
  void PauseMIDIStream(int stream) {
    unsigned long err;
    
    // try to pause the stream
    Py_BEGIN_ALLOW_THREADS
    err = midiStreamPause((HMIDISTRM) stream);
    Py_END_ALLOW_THREADS
    
    if (err != MMSYSERR_NOERROR) {
      PyErr_SetString(PyExc_WindowsError, "could not pause stream");
    }
  }
  
  void StopMIDIStream(int stream) {
    unsigned long err;
    
    // try to stop the stream
    Py_BEGIN_ALLOW_THREADS
    err = midiStreamStop((HMIDISTRM) stream);
    Py_END_ALLOW_THREADS
    
    if (err != MMSYSERR_NOERROR) {
      PyErr_SetString(PyExc_WindowsError, "could not stop stream");
    }
  }
  
  int prepareMIDIStream(int stream, MIDIHDR *header) {
    unsigned long err;
    
    // prepare the header
    Py_BEGIN_ALLOW_THREADS
    err = midiOutPrepareHeader((HMIDIOUT) stream, header, sizeof(MIDIHDR));
    Py_END_ALLOW_THREADS
    
    if (err != MMSYSERR_NOERROR) {
      PyErr_SetString(PyExc_WindowsError, "could not prepare stream header");
      return 0;
    }
    
    return 1;
  }
  
  int unprepareMIDIStream(int stream, MIDIHDR *header) {
    unsigned long err;
  
    // prepare the header
    Py_BEGIN_ALLOW_THREADS
    err = midiOutUnprepareHeader((HMIDIOUT) stream, header, sizeof(MIDIHDR));
    Py_END_ALLOW_THREADS
    
    if (err != MMSYSERR_NOERROR) {
      PyErr_SetString(PyExc_WindowsError, "could not unprepare stream header");
      return 0;
    }
    
    return 1;
  }
  
  int outMIDIStream(int stream, MIDIHDR *header) {
    unsigned long err;
  
    // put data into the stream
    Py_BEGIN_ALLOW_THREADS
    err = midiStreamOut((HMIDISTRM) stream, header, sizeof(MIDIHDR));
    Py_END_ALLOW_THREADS

    if (err != MMSYSERR_NOERROR) {
      PyErr_SetString(PyExc_WindowsError, "could not place data in stream");
      return 0;
    }
    
    return 1;
  }
  
  void FillMIDIStream(int stream, PyObject *pytuple) {
    PyObject *iterator;
    PyObject *item;
    unsigned long *events;
    int i, size;
     
    // try to get an iterator over the sequence
    iterator = PySeqIter_New(pytuple);

    // make sure we have got a valid iterator
    if (iterator == NULL) {
      PyErr_SetString(PyExc_ValueError, "could not get a valid iterator for data sequence");
      return;
    }
    
    // if the header already exists, get rid of its contents; otherwise, make a new one
    if(header != NULL) {
      StopMIDIStream(stream);
      if(unprepareMIDIStream(stream, header))
        free(header->lpData);       
    } else {
      header = (MIDIHDR *) malloc(sizeof(MIDIHDR));
    }
    
    // construct a new MIDIEVENT array
    size = PyTuple_Size(pytuple) * sizeof(unsigned long);
    events = (unsigned long *) malloc(size);
  
    // iterate over all sequence items, packing them into the data array
    i = 0;
    while (item = PyIter_Next(iterator)) {
      events[i] = (unsigned long) PyInt_AsLong(item);
      Py_DECREF(item);
      i++;
    }
    Py_DECREF(iterator);
    
    // store the data in the header
    header->lpData = (LPBYTE)events;
    header->dwBufferLength = size;
    header->dwBytesRecorded = size;
    header->dwFlags = 0;
    
    // prepare the header and put it in the stream if possible
    if(prepareMIDIStream(stream, header))
      outMIDIStream(stream, header);
  }
  
  int OpenMIDIStream(unsigned long device) {
    HMIDISTRM stream;
    MIDIPROPTIMEDIV prop;
    unsigned long err;
  
    // try to open the stream
    Py_BEGIN_ALLOW_THREADS
    err = midiStreamOpen(&stream, &device, 1, 0, 0, CALLBACK_NULL); //(DWORD) midiCallback, 0, CALLBACK_FUNCTION);
    Py_END_ALLOW_THREADS
    
    if (err != MMSYSERR_NOERROR) {
      PyErr_SetString(PyExc_WindowsError, "could not open stream");
      return 0;
    }
    
    // also set up the proper time division (96 PPQN)
    prop.cbStruct = sizeof(MIDIPROPTIMEDIV);
    prop.dwTimeDiv = 96;
  		err = midiStreamProperty(stream, (LPBYTE)&prop, MIDIPROP_SET|MIDIPROP_TIMEDIV);
    if (err != MMSYSERR_NOERROR) {
      PyErr_SetString(PyExc_WindowsError, "could not set time division");
      return 0;
    }
    
    // return the stream handle if successful
    return (int) stream;
  }

  void CloseMIDIStream(int stream) {
    unsigned long err;

    // if the header exists, get rid of its contents
    if(header != NULL) {
      StopMIDIStream(stream);
      if(unprepareMIDIStream(stream, header))
        free(header->lpData);
    }
    header = NULL;
    
    // try to close the stream
    Py_BEGIN_ALLOW_THREADS
    err = midiStreamClose((HMIDISTRM) stream);
    Py_END_ALLOW_THREADS
    
    if (err != MMSYSERR_NOERROR) {
      PyErr_SetString(PyExc_WindowsError, "could not close stream");
    }    
  }  
%}

int OpenMIDIStream(unsigned long device);
void CloseMIDIStream(int stream);
void FillMIDIStream(int stream, PyObject *data);
void StopMIDIStream(int stream);
void PauseMIDIStream(int stream);
void RestartMIDIStream(int stream);

/*
WINMMAPI MMRESULT WINAPI midiStreamOpen( OUT LPHMIDISTRM phms, IN LPUINT puDeviceID, IN DWORD cMidi, IN DWORD_PTR dwCallback, IN DWORD_PTR dwInstance, IN DWORD fdwOpen);
WINMMAPI MMRESULT WINAPI midiStreamClose( IN HMIDISTRM hms);

WINMMAPI MMRESULT WINAPI midiStreamProperty( IN HMIDISTRM hms, OUT LPBYTE lppropdata, IN DWORD dwProperty);
WINMMAPI MMRESULT WINAPI midiStreamPosition( IN HMIDISTRM hms, OUT LPMMTIME lpmmt, IN UINT cbmmt);

WINMMAPI MMRESULT WINAPI midiStreamOut( IN HMIDISTRM hms, IN LPMIDIHDR pmh, IN UINT cbmh);
WINMMAPI MMRESULT WINAPI midiStreamPause( IN HMIDISTRM hms);
WINMMAPI MMRESULT WINAPI midiStreamRestart( IN HMIDISTRM hms);
WINMMAPI MMRESULT WINAPI midiStreamStop( IN HMIDISTRM hms);
*/