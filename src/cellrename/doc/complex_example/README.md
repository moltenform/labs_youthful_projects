
[CellRename Home](https://github.com/downpoured/labs_youthful_projects/tree/master/cellrename/README.md) | [Simple Example](https://github.com/downpoured/labs_youthful_projects/tree/master/cellrename/doc/simple_example/README.md) | Complex Example

CellRename documentation: A complex example

I have a CD by Dungen, one of my favorite Swedish indie rock bands. When copying the CD to mp4s on my computer,
it looks like the audio software choked on the Swedish characters, and the resulting mp4 files have truncated names.
Is there a way I can avoid having to type all of the song names in manually?

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/008_t1.png" />

First, I'll sort by file-creation time, to sort in album order:

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/009_t2.png" />

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/010_t3.png" />

Now, I'll look up the album, Ta Det Lugnt, on amazon.com.

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/011_t4.png" />

I copy the text from this website, go back to CellRename, click on the first cell under "New name", and use Ctrl+V to paste. (Cool!)

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/012_t5.png" />

There are other ways to accomplish this, but let's use regular expressions to remove the track numbers.
I select "Replace in Filename" from the Edit menu, and type:

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/013_t6.png" />

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/014_t7.png" />
(The r: means to use a regular expression. The [0-9] means to match any number from 0 to 9. The + means that the [0-9] can occur one or more times).

Looks like this worked:

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/015_t8.png" />
Now, I'll add back the track numbers, and leading zeros, and the .mp4 extension. After selecting Pattern... from the Edit menu, I'll type:

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/016_t9.png" />
(For each file, the %n will be replaced by a new number, and the %f will be replaced by the previous filename.)
 Now I'm done!

<img style="border:none" border="0" src="https://downpoured.github.io/pages/cellrename2/figure/017_t10.png" />

<img style="border:none"  border="0" src="https://downpoured.github.io/pages/cellrename2/figure/018_dungen_re.jpg" />
