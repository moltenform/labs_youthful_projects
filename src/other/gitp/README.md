
```
gitp
(c) 2020 moltenform(Ben Fisher)
released under the MIT license

a tool to store & back up the changes you're not ready to push to a server.

when using gitp, you make your changes as unstaged modifications
in your git repo, periodically using gitp pack to save the changes to a zip file.
when ready to commit, you commit as usual, then edit gitp_util.py
to type in the new basis commit id. you can optionally connect to
an alt directory, which is another copy on your hard drive of the same
repo, which we'll use for some temporary actions like verifying
that a patch will apply perfectly. we will also use it to store a 'prev'
commit, which is a partial version of your current changes on the main
directory, this is used when you run gitp diffprev.

features:
gitp diffdev (shows diff against main branch, aka ddd)
gitp diffprev (shows diff against prev commit on branch)
gitp copytoaltrepo (copy files to the other repo)
gitp slowcopytoaltrepo
gitp pack
gitp apply
gitp recent
gitp runprettier
gitp updatebasis
gitp seepacketdiff
gitp seepacketinfo
gitp seepacketinfoverbose

___________________

how to install:

1) install gitk

2) copy the gitp directory to /example/path
The file /example/path/gitp/gitp_util.py should exist.

3) add
gitp() {
    saveddir=$(pwd)
    gitpdir=/example/path
    gitptmp=/example/path/changedir.txt
    
    rm -f $gitptmp
    (cd $gitpdir && python3 -m gitp $saveddir $@)
    if [ -f $gitptmp ]; then
        saveddir=`cat $gitptmp`
        cd $saveddir
    fi
}

alias ddd='gitp diffdev'

to your zshrc or bashrc file.

4) open gitp_util.py and, based on the examples,
type in paths for outDir, workingRepos, tempRepos, and basisCommits

5) cd to the first entry in workingRepos, make some changes, and
run ddd in your shell. gitk should show up, displaying the changes you made.

6) use gitp pack to save your changes to a zip file,
and gitp apply to apply them. type gitp with no arguments to see more features.

```

