
Trilling
A proof-of-concept for music transcription
Ben Fisher, 2009
GPLv3.



To start, run main.py. Requires Python 2.5.

Excerpt from a blog entry:
Many musicians spend time transcribing music, using computer software such as Sibelius or Finale to enter notes into sheet music.  This process, though, could use improvement, particularly when a midi keyboard is not available. (In some programs the computer keyboard mapping is not intuitive; in Finale 2006, one enters notes by typing their letter names. The "g" key produces a g note and so on - but on a qwerty layout, this doesn't work well, because of the non-consequtive placement of a,b, and c. Also, I have yet to see a good way to select note duration.)  Both Finale and Sibelius have recently released new transcription features, such as Sibelius' keyboard window introduced in May 2009. Here I will introduce one of my ideas for faster music transcription.

Instead of following an arbitrary metronome when recording, my program lets the performer tap the pulse themselves. This lets the performer play more naturally, and also to slow down for parts of the music that are more complex. I recently finished a proof-of-concept of this system that needs only a standard computer keyboard. The keys are played like a piano, and the Tab key is tapped for every quarter note. 


    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
    
    