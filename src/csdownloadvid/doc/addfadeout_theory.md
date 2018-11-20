
[Back](../addfadeout.md)

## Lossless fade-out for a mp4/m4a song

<i>By moltenjs / Ben Fisher</i>

Sometimes you'll have a song that goes on for longer than you like. It's an mp4 audio file (aka .m4a or "aac"). You could open the song in Audacity, delete the last part of the song, and add a fade-out. This would work, but then you to have to re-encode the audio again, and since m4a is a lossy format, some audio data will be lost.

Let's say the song is 6:00 long, and we want to keep only the first 4:00, with an 8 second fade-out. We'll re-encode these last 8 seconds, but we can leave the rest of the song untouched.

* Losslessly split the input file into 3 audio files with `ffmpeg`.

    * (0:00 - 4:00), save as "part1.m4a"
    * (4:00 - 4:08), save as "part2.m4a"
    * (4:08 - 6:00), save as "part3.m4a"

* Use `ffmpeg`'s `afade` to create an 8-second fade out to "part2", save to "part2_fade.wav"

* Use `qaac` to encode `part2_fade.wav` to `part2_fade.m4a`

* Use `ffmpeg` to extract the raw .aac data from `part1.m4a` to `part1raw.aac`

* Use `ffmpeg` to extract the raw .aac data from `part2_fade.m4a` to `part2_faderaw.aac`

* Strip the priming frames at the beginning of `part2_faderaw.aac` (see "details" section below)

* Concatenate the contents of `part1raw.aac` and `part2_faderaw.aac` to `output.aac`

* Use `ffmpeg` to add an mp4 header, `output.aac` to `output.m4a`

## Code

I've written a C# implementation [here](../src/ClassAacFadeout.cs).

## Details

    The raw data aac file is built from frames. The first frames contain priming data,
    mentioned in https://developer.apple.com/library/mac/technotes/tn2258/_index.html
    The encoder delay for Nero aac is ~2600 samples
    The encoder delay for qaac (Apple) is ~2112 samples
    We can't break apart a frame without needing to encode, but 2048 is quite close to 2112.
    So, for `qaac`, deleting the first two 1024-sample frames yields good results.

    Alternatives that aren't sufficient:
    If the -af filter for ffmpeg is used, it re-encodes the entire file.
    Joining the two m4a files with ffmpeg -concat leaves a gap of silence
    Gluing the two pieces of aac with ffmpeg -concat leaves a gap of silence
    Gluing the two pieces of aac by concat'ing aac files leaves a gap of silence

The result is all lossless except for the final seconds, the fade-out itself. There will sometimes be a quiet sound artifact heard right at the transition. In my example, if there is a quiet sound at 4:00 in the output, I'll try the process again using 4:01 instead of 4:00.

