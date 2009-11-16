#region License

/* Copyright (c) 2006 Leslie Sanford
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Multimedia.Midi
{
    /// <summary>
    /// Represents a collection of Tracks.
    /// </summary>
    public sealed class Sequence : ICollection<Track>
    {
        #region Sequence Members

        #region Fields

        // The collection of Tracks for the Sequence.
        public List<Track> tracks = new List<Track>();

        // The Sequence's MIDI file properties.
        private MidiFileProperties properties = new MidiFileProperties();

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the Sequence class.
        /// </summary>
        public Sequence()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Sequence class with the specified division.
        /// </summary>
        /// <param name="division">
        /// The Sequence's division value.
        /// </param>
        public Sequence(int division)
        {
            properties.Division = division;
            properties.Format = 1;
        }

        /// <summary>
        /// Initializes a new instance of the Sequence class with the specified
        /// file name of the MIDI file to load.
        /// </summary>
        /// <param name="fileName">
        /// The name of the MIDI file to load.
        /// </param>
        public Sequence(string fileName)
        {
            Load(fileName);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads a MIDI file into the Sequence.
        /// </summary>
        /// <param name="fileName">
        /// The MIDI file's name.
        /// </param>
        public void Load(string fileName)
        {
            #region Require

            if(fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            #endregion

            FileStream stream = new FileStream(fileName, FileMode.Open,
                FileAccess.Read, FileShare.Read);

            using(stream)
            {
                MidiFileProperties newProperties = new MidiFileProperties();
                TrackReader reader = new TrackReader();
                List<Track> newTracks = new List<Track>();

                newProperties.Read(stream);

                for(int i = 0; i < newProperties.TrackCount; i++)
                {
                    reader.Read(stream);
                    newTracks.Add(reader.Track);
                }

                properties = newProperties;
                tracks = newTracks;
            }

            #region Ensure

            Debug.Assert(Count == properties.TrackCount);

            #endregion
        }

        /// <summary>
        /// Saves the Sequence as a MIDI file.
        /// </summary>
        /// <param name="fileName">
        /// The name to use for saving the MIDI file.
        /// </param>
        public void Save(string fileName)
        {
            #region Require

            if(fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            #endregion

            FileStream stream = new FileStream(fileName, FileMode.Create,
                FileAccess.Write, FileShare.None);

            using(stream)
            {
                properties.Write(stream);

                TrackWriter writer = new TrackWriter();

                foreach(Track trk in tracks)
                {
                    writer.Track = trk;
                    writer.Write(stream);
                }
            }
        }

        /// <summary>
        /// Gets the length in ticks of the Sequence.
        /// </summary>
        /// <returns>
        /// The length in ticks of the Sequence.
        /// </returns>
        /// <remarks>
        /// The length in ticks of the Sequence is represented by the Track 
        /// with the longest length.
        /// </remarks>
        public int GetLength()
        {
            int length = 0;

            foreach(Track t in this)
            {
                if(t.Length > length)
                {
                    length = t.Length;
                }
            }

            return length;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Track at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index of the Track to get.
        /// </param>
        /// <returns>
        /// The Track at the specified index.
        /// </returns>
        public Track this[int index]
        {
            get
            {
                #region Require

                if(index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException("index", index,
                        "Sequence index out of range.");
                }

                #endregion

                return tracks[index];
            }
        }

        /// <summary>
        /// Gets the Sequence's division value.
        /// </summary>
        public int Division
        {
            get
            {
                return properties.Division;
            }
        }

        /// <summary>
        /// Gets or sets the Sequence's format value.
        /// </summary>
        public int Format
        {
            get
            {
                return properties.Format;
            }
            set
            {
                properties.Format = value;
            }
        }

        /// <summary>
        /// Gets the Sequence's type.
        /// </summary>
        public SequenceType SequenceType
        {
            get
            {
                return properties.SequenceType;
            }
        }

        #endregion

        #endregion

        #region ICollection<Track> Members

        public void Add(Track item)
        {
            #region Require

            if(item == null)
            {
                throw new ArgumentNullException("item");
            }

            #endregion

            tracks.Add(item);

            properties.TrackCount = tracks.Count;
        }

        public void Clear()
        {            
            tracks.Clear();

            properties.TrackCount = 0;
        }

        public bool Contains(Track item)
        {
            return tracks.Contains(item);
        }

        public void CopyTo(Track[] array, int arrayIndex)
        {
            tracks.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return tracks.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(Track item)
        {
            bool result = tracks.Remove(item);

            if(result)
            {
                properties.TrackCount = tracks.Count;
            }

            return result;
        }

        #endregion

        #region IEnumerable<Track> Members

        public IEnumerator<Track> GetEnumerator()
        {
            return tracks.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return tracks.GetEnumerator();
        }

        #endregion
    }
}
