# Copyright (c) Ben Fisher, 2018.
# Licensed under GPLv3.

# a script for csdownloadvid, 
# wraps pytube and makes it look/act like ytdl does

import os
import sys
try:
    from shinerainsevenlib.standard import *
    from shinerainsevenlib.core import *
except ImportError:
    print('Please install shinerainsevenlib; python -m pip install shinerainsevenlib')


myPath = os.path.split(os.path.realpath(__file__))[0]
pytPath = os.path.join(myPath, 'tools', 'pytubemasterdir', 'pytube-master')
assertTrue(files.isDir(pytPath), 'cannot find tools/pytubemasterdir')
sys.path.append(pytPath)

def goMain():
    opts = {}
    for item in sys.argv:
        if item == "--list-formats":
            opts['listFormatsOnly'] = True
        if item.lower().endswith('ffmpeg') or \
            item.lower().endswith('ffmpeg.exe'):
            opts['ffmpegPath'] = item
        if item.startswith('http://') or item.startswith('https://'):
            opts['url'] = item
        if item.startswith('<Stream:'):
            opts['format'] = item
        if item.startswith('--outputdir='):
            opts['outPath'] = item.replace('--outputdir=', '')
    
    goImpl(opts)

def goImpl(opts):
    listFormatsOnly = opts.get('listFormatsOnly', False)
    url  = opts.get('url', None)
    assertTrue(url, 'you forgot to specify url!!')
    
    from pytube import YouTube
    assertTrue(url, 'you forgot to specify url!!')
    yt = YouTube(url)
    choices = [item for item in yt.streams.all()]
    choicesStrings = [str(item) for item in choices]
    
    if listFormatsOnly:
        # print this to make the output look similar to ytdl's output
        trace('[info] Available formats for ', url)
        for choice in choicesStrings:
            trace(shortenStreamName(choice))
        trace('done.')
    else:
        ffmpegPath = opts.get('ffmpegPath', None)
        assertTrue(ffmpegPath, 'forgot to specify ffmpegPath!!')
        outPath = opts.get('outPath', None)
        assertTrue(outPath, 'forgot to specify outPath!!')
        assertTrue(files.isDir(outPath), 'outPath not a dir!!')
        format = opts.get('format', None)
        assertTrue(format, 'forgot to specify format!!')
        chosenObj = None
        for choiceObj in choices:
            s = shortenStreamName(choiceObj)
            if s == format:
                assertTrue(chosenObj is None, 'ambiguous format??', 
                    format, choicesStrings)

                chosenObj = choiceObj
        assertTrue(chosenObj, 'none matches format', format, choicesStrings)
        downloadIt(chosenObj, outPath, ffmpegPath, url, format)

def shortenStreamName(s):
    s = str(s)
    assertTrue(s.startswith('<Stream:'), s)
    s = s.replace(' mime_type="audio/mp4"', ' "m4a"')
    s = s.replace(' mime_type="audio/"', ' "')
    s = s.replace(' mime_type="video/', ' "')
    s = s.replace(' res="', ' "')
    s = s.replace(' vcodec="', ' "')
    s = s.replace(' acodec="', ' "')
    s = s.replace(' fps="30fps" ', ' ')
    return s

def downloadIt(chosenObj, outdir, ffmpegPath, url, format):
    isAnAudioFile = 'abr="' in format
    ytId = getYtIdFromUrl(url)
    writtenFile = chosenObj.download(outdir)
    trace(f'wrote to {writtenFile}')
    newName = os.path.splitext(files.getName(writtenFile))[0]

    # make it title case
    newName = ChangeCasing().title(newName)
    newName = newName + f' [{ytId}]'
    if isAnAudioFile:
        newName += '.m4a'
    else:
        newName += os.path.splitext(writtenFile)[1]

    newName = toValidFilename(getPrintable(newName))
    newName = files.join(outdir, newName)
    trace(f'wrote to {writtenFile}, moving to \r\n {newName}')
    files.move(writtenFile, newName, False)
    if isAnAudioFile:
        correctHeader(newName, ffmpegPath)

def correctHeader(path, ffmpeg):
    trace('[ffmpeg] Correcting container in "{path}"')
    assertTrue(files.exists(path), path)
    assertTrue(path.endswith('.m4a'), path)
    tmpPath = path + '---tempforheader---.m4a'
    files.move(path, tmpPath, False)
    args = [ffmpeg, '-i', tmpPath, '-c', 'copy', '-f', 'mp4', path]
    files.run(args)
    assertTrue(files.exists(path), path)
    softDeleteFile(tmpPath)

def getYtIdFromUrl(ytUrl):
    import urllib.parse as urlparse
    if ytUrl.startswith('https://youtu.be/'):
        ytId = ytUrl.split('/')[-1]
    else:
        parsed = urlparse.urlparse(ytUrl)
        ytId = urlparse.parse_qs(parsed.query).get('v', 'couldNotGetVidId')
        ytId = str(ytId).replace("['", '').replace("']", '')
    return ytId

def tests():
    assertEq('gdgX1KpQEgY', getYtIdFromUrl(
        'https://www.youtube.com/watch?v=gdgX1KpQEgY'))
    assertEq('gdgX1KpQEgY', getYtIdFromUrl(
        'https://www.youtube.com/watch?v=gdgX1KpQEgY&h=abcd'))
    assertEq('gdgX1KpQEgY', getYtIdFromUrl(
        'https://www.youtube.com/watch?y=qwert&v=gdgX1KpQEgY&h=abcd'))
    assertEq('gdgX1KpQEgY', getYtIdFromUrl(
        'https://youtu.be/gdgX1KpQEgY'))
    assertEq('abc', getYtIdFromUrl(
        'https://youtu.be/abc'))
    assertEq('couldNotGetVidId', getYtIdFromUrl(
        'https://www.youtube.com/watch?y=qwert'))

class ChangeCasing(object):
    def upper(self, s):
        return s.upper()
        
    def lower(self, s):
        return s.lower()

    def title(self, s):
        result = ''
        for i, c in enumerate(s):
            # if it's not adjacent to a letter, make it capitalized.
            if i == 0 or not s[i - 1].isalpha():
                result += c.upper()
            else:
                result += c.lower()
                
        return result

if __name__=='__main__':
    tests()
    goMain()
