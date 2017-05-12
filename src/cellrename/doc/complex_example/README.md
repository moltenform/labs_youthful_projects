
[CellRename Home](https://github.com/downpoured/labs_youthful_projects/tree/master/cellrename/README.md) | [Simple Example](https://github.com/downpoured/labs_youthful_projects/tree/master/cellrename/doc/simple_example/README.md) | Complex Example

CellRename documentation: A complex example

I have a CD by Dungen, one of my favorite Swedish indie rock bands. When copying the CD to mp4s on my computer,
it looks like the audio software choked on the Swedish characters, and the resulting mp4 files have truncated names.
Is there a way I can avoid having to type all of the song names in manually?

<img style="border:none"  border="0" src="http://3.bp.blogspot.com/-xgeqHqKq39A/UP9vzUsEv7I/AAAAAAAAApw/Q5hnRxDVFTM/s320/t1.png" />

First, I'll sort by file-creation time, to sort in album order:

<img style="border:none"  border="0" src="http://1.bp.blogspot.com/-5Bi2a1HIn70/UP9vzv7RNbI/AAAAAAAAAp8/iCZYuQJBIT4/s320/t2.png" />

<img style="border:none"  border="0" src="http://2.bp.blogspot.com/-hI_Jw3xhTs8/UP9v0oAmkeI/AAAAAAAAAqI/-2688Bt1iTc/s320/t3.png" />

Now, I'll look up the album, Ta Det Lugnt, on amazon.com.

<img style="border:none"  border="0" src="http://4.bp.blogspot.com/-jfi4lp9Ts7g/UP9v1OaW4fI/AAAAAAAAAqU/WjHizCZ0yoc/s320/t4.png" />

I copy the text from this website, go back to CellRename, click on the first cell under "New name", and use Ctrl+V to paste. (Cool!)

<img style="border:none"  border="0" src="http://3.bp.blogspot.com/-gCkRtsqqQak/UP9v1hzPPoI/AAAAAAAAAqg/J5akuihF1zc/s320/t5.png" />

There are other ways to accomplish this, but let's use regular expressions to remove the track numbers.
I select "Replace in Filename" from the Edit menu, and type:

<img style="border:none"  border="0" src="http://3.bp.blogspot.com/-7cUQVQDp6Vs/UP9wEtAfFmI/AAAAAAAAAqs/zK17iWraCwU/s320/t6.png" />

<img style="border:none"  border="0" src="http://2.bp.blogspot.com/-FqDUFVvXbvA/UP9wFGPUHSI/AAAAAAAAAq4/YxJqT5txKvg/s320/t7.png" />
(The r: means to use a regular expression. The [0-9] means to match any number from 0 to 9. The + means that the [0-9] can occur one or more times).

Looks like this worked:

<img style="border:none"  border="0" src="http://3.bp.blogspot.com/-ZztewK0gD_4/UP9wFYhQwaI/AAAAAAAAArE/TQKZcSrEoWE/s320/t8.png" />
Now, I'll add back the track numbers, and leading zeros, and the .mp4 extension. After selecting Pattern... from the Edit menu, I'll type:

<img style="border:none"  border="0" src="http://4.bp.blogspot.com/-eNGWsBCtC0U/UP9wGFcWd6I/AAAAAAAAArQ/ZZKijO9lBbI/s320/t9.png" />
(For each file, the %n will be replaced by a new number, and the %f will be replaced by the previous filename.)
 Now I'm done!

<img style="border:none" border="0" src="http://3.bp.blogspot.com/-VQD2uhJSUTU/UP9zKEpBeGI/AAAAAAAAArw/jDbhYTaPSbI/s320/t10.png" />

<img style="border:none"  border="0" src="http://1.bp.blogspot.com/-JtQrgOeBdXQ/UP9wGdny-RI/AAAAAAAAArc/FCIFQjPlGVQ/s320/dungen.jpg" />

