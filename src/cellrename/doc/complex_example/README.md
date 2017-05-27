
[CellRename Home](https://github.com/downpoured/labs_youthful_projects/tree/master/cellrename/README.md) | [Simple Example](https://github.com/downpoured/labs_youthful_projects/tree/master/cellrename/doc/simple_example/README.md) | Complex Example

CellRename documentation: A complex example

I just bought a great CD by the Swedish rock group "Dungen". I was copying the CD to mp3s on my computer,
but it looks like the software choked on the Swedish letters because the filenames are all truncated.
Do I have to slowly type in the correct name for each file?

<img style="border:none" border="0" src="https://downpoured.github.io/pages/cellrename2/figure/example001.png" />

The answer is no -- I can use CellRename to quickly add the right filenames.

First, I'll sort by file-creation time, so that the files are sorted in the right order:

<img style="border:none" border="0" src="https://downpoured.github.io/pages/cellrename2/figure/example002a.png" />

<img style="border:none" border="0" src="https://downpoured.github.io/pages/cellrename2/figure/example002b.png" />

Now, I'll look up the album, Ta Det Lugnt, on amazon.com.

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/amazon.png" />

I copy the text from this website, go back to CellRename, click on the topmost cell, and hit Ctrl+V to paste. (Cool!)

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/example003.png" />

CellRename supports renaming files by Regular Expressions, which can be very useful. I can go from "Song Title - Artist.mp3" to "Artist - Song Title.mp3", by hitting Replace regexp in filenames, searching for the pattern "(.*?) - (.*?).mp3", and replacing with "\\2 - \\1.mp3".

In this case, I'll use CellRename to go from the format "1. Panda" to the form that I prefer, "01 Panda". I'll use a regular expression as an example.

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/example004a.png" />

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/example004b.png" />

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/example004c.png" />

In the regular expression, [0-9]+\\. means to search for a string of 0-9 digits followed by a period.

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/example005a.png" />

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/example005b.png" />

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/example005c.png" />

It worked! Now I'll press Ctrl+Enter to rename the files.

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/example006.png" />

Now I'm done!


<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/018_dungen_re.jpg" />
