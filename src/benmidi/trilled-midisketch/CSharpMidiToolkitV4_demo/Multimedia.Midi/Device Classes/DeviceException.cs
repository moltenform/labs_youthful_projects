#region License

/* Copyright (c) 2005 Leslie Sanford
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Leslie Sanford
 * Email: jabberdabber@hotmail.com
 */

#endregion

using System;

namespace Multimedia.Midi
{
	/// <summary>
	/// The base class for all MIDI device exception classes.
	/// </summary>
	public class DeviceException : ApplicationException
	{
        #region DeviceException Members

        #region Error Codes

        public const int MMSYSERR_NOERROR      = 0;  /* no error */
        public const int MMSYSERR_ERROR        = 1;  /* unspecified error */
        public const int MMSYSERR_BADDEVICEID  = 2;  /* device ID out of range */
        public const int MMSYSERR_NOTENABLED   = 3;  /* driver failed enable */
        public const int MMSYSERR_ALLOCATED    = 4;  /* device already allocated */
        public const int MMSYSERR_INVALHANDLE  = 5;  /* device handle is invalid */
        public const int MMSYSERR_NODRIVER     = 6;  /* no device driver present */
        public const int MMSYSERR_NOMEM        = 7;  /* memory allocation error */
        public const int MMSYSERR_NOTSUPPORTED = 8;  /* function isn't supported */
        public const int MMSYSERR_BADERRNUM    = 9;  /* error value out of range */
        public const int MMSYSERR_INVALFLAG    = 10; /* invalid flag passed */
        public const int MMSYSERR_INVALPARAM   = 11; /* invalid parameter passed */
        public const int MMSYSERR_HANDLEBUSY   = 12; /* handle being used */
                                                     /* simultaneously on another */
                                                     /* thread (eg callback) */
        public const int MMSYSERR_INVALIDALIAS = 13; /* specified alias not found */
        public const int MMSYSERR_BADDB        = 14; /* bad registry database */
        public const int MMSYSERR_KEYNOTFOUND  = 15; /* registry key not found */
        public const int MMSYSERR_READERROR    = 16; /* registry read error */
        public const int MMSYSERR_WRITEERROR   = 17; /* registry write error */
        public const int MMSYSERR_DELETEERROR  = 18; /* registry delete error */
        public const int MMSYSERR_VALNOTFOUND  = 19; /* registry value not found */
        public const int MMSYSERR_NODRIVERCB   = 20; /* driver does not call DriverCallback */
        public const int MMSYSERR_LASTERROR    = 20; 

        public const int MIDIERR_UNPREPARED    = 64; /* header not prepared */
        public const int MIDIERR_STILLPLAYING  = 65; /* still something playing */
        public const int MIDIERR_NOMAP         = 66; /* no configured instruments */
        public const int MIDIERR_NOTREADY      = 67; /* hardware is still busy */
        public const int MIDIERR_NODEVICE      = 68; /* port no longer connected */
        public const int MIDIERR_INVALIDSETUP  = 69; /* invalid MIF */
        public const int MIDIERR_BADOPENMODE   = 70; /* operation unsupported w/ open mode */
        public const int MIDIERR_DONT_CONTINUE = 71; /* thru device 'eating' a message */
        public const int MIDIERR_LASTERROR     = 71; /* last error in range */

        #endregion

        #region Fields

        // The error code.
        private int errCode;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the DeviceException class with the
        /// specified error code.
        /// </summary>
        /// <param name="errCode">
        /// The error code.
        /// </param>
		public DeviceException(int errCode)
		{
            this.errCode = errCode;
		}

        #endregion

        #region Properties

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public int ErrorCode
        {
            get
            {
                return errCode;
            }
        }

        #endregion

        #endregion
	}
}
