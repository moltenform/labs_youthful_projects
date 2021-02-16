# BenPythonCommon,
# 2015 Ben Fisher, released under the GPLv3 license.

import os
from .common_util import *

def getRandomString(max=100 * 1000):
    import random
    return '%s' % random.randrange(max)

def genGuid(asBase64=False):
    import uuid
    u = uuid.uuid4()
    if asBase64:
        import base64
        b = base64.urlsafe_b64encode(u.bytes_le)
        return b.decode('utf8')
    else:
        return str(u)

def getNowAsMillisTime():
    import time
    t = time.time()
    return int(t * 1000)

def DBG(obj=None):
    import pprint
    if obj is None:
        import inspect
        fback = inspect.currentframe().f_back
        framelocals = fback.f_locals
        newDict = {}
        for key in framelocals:
            if not callable(framelocals[key]) and not \
                    inspect.isclass(framelocals[key]) and not \
                    inspect.ismodule(framelocals[key]):
                newDict[key] = framelocals[key]
        pprint.pprint(newDict)
    else:
        pprint.pprint(obj)

def getClipboardTextTk():
    try:
        from tkinter import Tk
    except ImportError:
        from Tkinter import Tk
    try:
        r = Tk()
        r.withdraw()
        s = r.clipboard_get()
    except BaseException as e:
        if 'selection doesn\'t exist' in str(e):
            s = ''
        else:
            raise
    finally:
        r.destroy()
    return s

def setClipboardTextTk(s):
    try:
        from tkinter import Tk
    except ImportError:
        from Tkinter import Tk
    if not isPy3OrNewer:
        text = unicode(s)
    try:
        r = Tk()
        r.withdraw()
        r.clipboard_clear()
        r.clipboard_append(text)
    finally:
        r.destroy()

def getClipboardTextPyperclip():
    import pyperclip
    return pyperclip.paste()

def setClipboardTextPyperclip(s):
    import pyperclip
    pyperclip.copy(s)

def getClipboardText():
    try:
        return getClipboardTextPyperclip()
    except ImportError:
        return getClipboardTextTk()

def setClipboardText(s):
    try:
        setClipboardTextPyperclip(s)
    except ImportError:
        setClipboardTextTk(s)

class PersistedDict(object):
    data = None
    handle = None
    counter = 0
    persistEveryNWrites = 1

    def __init__(self, filename, warnIfCreatingNew=True,
            keepHandle=False, persistEveryNWrites=5):
        from .files import exists, writeall
        from .common_ui import alert
        self.filename = filename
        self.persistEveryNWrites = persistEveryNWrites
        if not exists(filename):
            if warnIfCreatingNew:
                alert("creating new cache at " + filename)
            writeall(filename, '{}')
        self.load()
        if keepHandle:
            self.handle = open(filename, 'w')
            self.persist()

    def load(self):
        import json
        from .files import readall
        txt = readall(self.filename, encoding='utf-8')
        self.data = json.loads(txt)

    def close(self):
        if self.handle:
            self.handle.close()
            self.handle = None

    def persist(self):
        import json
        from .files import writeall
        txt = json.dumps(self.data)
        if self.handle:
            self.handle.seek(0, os.SEEK_SET)
            self.handle.write(txt)
            self.handle.truncate()
        else:
            writeall(self.filename, txt, encoding='utf-8')

    def afterUpdate(self):
        self.counter += 1
        if self.counter % self.persistEveryNWrites == 0:
            self.persist()

    def set(self, key, value):
        self.data[key] = value
        self.afterUpdate()

    def setSubdict(self, subdictname, key, value):
        if subdictname not in self.data:
            self.data[subdictname] = {}
        self.data[subdictname][key] = value
        self.afterUpdate()

    def setSubsubdict(self, subdictname, key1, key2, value):
        if subdictname not in self.data:
            self.data[subdictname] = {}
        self.data[subdictname][key1][key2] = value
        self.afterUpdate()

def downloadUrl(url, toFile=None, timeout=30, asText=False):
    import requests
    resp = requests.get(url, timeout=timeout)
    if toFile:
        with open(toFile, 'wb') as fout:
            fout.write(resp.content)
    if asText:
        return resp.text
    else:
        return resp.content

def startThread(fn, args=None):
    import threading
    if args is None:
        args = tuple()
    t = threading.Thread(target=fn, args=args)
    t.start()

class ParsePlus(object):
    '''
    ParsePlus, by Ben Fisher 2019

    Adds the following features to the "parse" module:
        {s:NoNewlines} field type
        {s:NoSpaces} works like {s:S}
        remember that "{s} and {s}" matches "a and a" but not "a and b",
            use "{s1} and {s2}" or "{} and {}" if the contents can differ
        escapeSequences such as backslash-escapes (see examples in tests)
        replaceFieldWithText (see examples in tests)
        getTotalSpan
    '''
    def __init__(self, pattern, extra_types=None, escapeSequences=None,
            case_sensitive=True):
        try:
            import parse
        except:
            raise ImportError('needs "parse", https://pypi.org/project/parse/')
        self.pattern = pattern
        self.case_sensitive = case_sensitive
        self.extra_types = extra_types if extra_types else {}
        self.escapeSequences = escapeSequences if escapeSequences else []
        if 'NoNewlines' in pattern:
            @parse.with_pattern(r'[^\r\n]+')
            def parse_NoNewlines(s):
                return str(s)
            self.extra_types['NoNewlines'] = parse_NoNewlines
        if 'NoSpaces' in pattern:
            @parse.with_pattern(r'[^\r\n\t ]+')
            def parse_NoSpaces(s):
                return str(s)
            self.extra_types['NoSpaces'] = parse_NoSpaces

    def _createEscapeSequencesMap(self, s):
        self._escapeSequencesMap = {}
        if len(self.escapeSequences) > 5:
            raise ValueError('we support a max of 5 escape sequences')

        sTransformed = s
        for i, seq in enumerate(self.escapeSequences):
            assertTrue(len(seq) > 1, "an escape-sequence only makes sense if " +
                "it is at least two characters")

            # use rarely-occurring ascii chars like
            # \x01 (start of heading)
            rareChar = chr(i + 1)
            # raise error if there's any occurance of rareChar, not repl,
            # otherwise we would have incorrect expansions
            if rareChar in s:
                raise RuntimeError("we don't yet support escape sequences " +
                "if the input string contains rare ascii characters. the " +
                "input string contains " + rareChar + ' (ascii ' +
                str(ord(rareChar)) + ')')
            # replacement string is the same length, so offsets aren't affected
            repl = rareChar * len(seq)
            self._escapeSequencesMap[repl] = seq
            sTransformed = sTransformed.replace(seq, repl)

        assertEq(len(s), len(sTransformed), 'internal error: len(s) changed.')
        return sTransformed

    def _unreplaceEscapeSequences(self, s):
        for key in self._escapeSequencesMap:
            s = s.replace(key, self._escapeSequencesMap[key])
        return s

    def _resultToMyResult(self, parseResult, s):
        if not parseResult:
            return parseResult
        ret = Bucket()
        lenS = len(s)
        for name in parseResult.named:
            val = self._unreplaceEscapeSequences(parseResult.named[name])
            setattr(ret, name, val)
        ret.spans = parseResult.spans
        ret.getTotalSpan = lambda: self._getTotalSpan(parseResult, lenS)
        return ret

    def _getTotalSpan(self, parseResult, lenS):
        if '{{' in self.pattern or '}}' in self.pattern:
            raise RuntimeError("for simplicity, we don't yet support getTotalSpan " +
                "if the pattern contains {{ or }}")
        locationOfFirstOpen = self.pattern.find('{')
        locationOfLastClose = self.pattern.rfind('}')
        if locationOfFirstOpen == -1 or locationOfLastClose == -1:
            # pattern contained no fields?
            return None

        if not len(parseResult.spans):
            # pattern contained no fields?
            return None
        smallestSpanStart = float('inf')
        largestSpanEnd = -1
        for key in parseResult.spans:
            lower, upper = parseResult.spans[key]
            smallestSpanStart = min(smallestSpanStart, lower)
            largestSpanEnd = max(largestSpanEnd, upper)

        # ex.: for the pattern aaa{field}bbb, widen by len('aaa') and len('bbb')
        smallestSpanStart -= locationOfFirstOpen
        largestSpanEnd += len(self.pattern) - (locationOfLastClose + len('}'))

        # sanity check that the bounds make sense
        assertTrue(0 <= smallestSpanStart <= lenS,
            'internal error: span outside bounds')
        assertTrue(0 <= largestSpanEnd <= lenS,
            'internal error: span outside bounds')
        assertTrue(largestSpanEnd >= smallestSpanStart,
            'internal error: invalid span')
        return (smallestSpanStart, largestSpanEnd)

    def match(self, s):
        # entire string must match
        import parse
        sTransformed = self._createEscapeSequencesMap(s)
        parseResult = parse.parse(self.pattern, sTransformed,
            extra_types=self.extra_types, case_sensitive=self.case_sensitive)
        return self._resultToMyResult(parseResult, s)

    def search(self, s):
        import parse
        sTransformed = self._createEscapeSequencesMap(s)
        parseResult = parse.search(self.pattern, sTransformed,
            extra_types=self.extra_types, case_sensitive=self.case_sensitive)
        return self._resultToMyResult(parseResult, s)

    def findall(self, s):
        import parse
        sTransformed = self._createEscapeSequencesMap(s)
        parseResults = parse.findall(self.pattern, sTransformed,
            extra_types=self.extra_types, case_sensitive=self.case_sensitive)
        for parseResult in parseResults:
            yield self._resultToMyResult(parseResult, s)

    def replaceFieldWithText(self, s, key, newValue,
            appendIfNotFound=None, allowOnlyOnce=False):
        from . import jslike
        # example: <title>{title}</title>
        results = list(self.findall(s))
        if allowOnlyOnce and len(results) > 1:
            raise RuntimeError('we were told to allow pattern only once.')
        if len(results):
            span = results[0].spans[key]
            return jslike.spliceSpan(s, span, newValue)
        else:
            if appendIfNotFound is None:
                raise RuntimeError("pattern not found.")
            else:
                return s + appendIfNotFound

    def replaceFieldWithTextIntoFile(self, path, key, newValue,
            appendIfNotFound=None, allowOnlyOnce=False, encoding=None):
        from .files import readall, writeall
        s = readall(path, encoding=encoding)

        newS = self.replaceFieldWithText(s, key, newValue,
            appendIfNotFound=appendIfNotFound,
            allowOnlyOnce=allowOnlyOnce)

        writeall(path, newS, 'w', encoding=encoding, skipIfSameContent=True)


'''
Example:
schema = CheckedConfigParserSchema(
    [
        CheckedConfigParserSchemaSection('main', [
            CheckedConfigParserSchemaEntry(re.compile('n[0-9]'), type=int, min=1, max=5, fallback=4),
            CheckedConfigParserSchemaEntry('path', type=str),
            CheckedConfigParserSchemaEntry('custom', type=myConverter),
            CheckedConfigParserSchemaEntry('isEnabled', type=bool, fallback=True),
        ],
        optional=False)
    ]
)

Example input:
key=value
spaces in keys=allowed
spaces in values=allowed as well
emptystring=
example=a multi-line
    string that continues across strings
# comment
; comment

Potential future additions:
Writing
Regular-expression identifiers

'''


class CheckedConfigParserException(Exception):
    pass

class _CheckedConfigParserMarkSeen(object):
    def __init__(self, count):
        self.wasSeen = [False] * count

    def idMatches(self, s):
        if isinstance(self.identifier, anystringtype):
            return self.identifier == s
        else:
            match = self.identifier.match(s)
            return match and match.end() == len(s)

class CheckedConfigParserSchemaEntry(object):
    def __init__(self, identifier, type=str, fallback=None, min=None, max=None):
        assertTrue(identifier is not None, 'identifier cannot be None')
        self.identifier = identifier
        self.type = type
        self.fallback = fallback
        self.min = min
        self.max = max
        self.knownTypes=dict(
            str='Expected a string',
            int='Expected an integer number',
            float='Expected a floating point number',
            bool='Expected a floating point number',
        )

    def parseInput(self, s, sContext):
        if self.type == bool:
            if s.lower() == 'true':
                return True
            elif s.lower() == 'false':
                return False
            else:
                msg = '%s%s' % (sContext, self.knownTypes[self.type.__name__])
                raise CheckedConfigParserException(msg)
        elif '__name__' in dir(self.type) and self.type.__name__ in self.knownTypes:
            try:
                return self.type(s)
            except:
                msg = '%s%s' % (sContext, self.knownTypes[self.type.__name__])
                raise CheckedConfigParserException(msg)
        else:
            return self.type(s, sContext)

class _CheckedConfigParserSchemaResults(object):
    pass

class CheckedConfigParserSchemaSection(object):
    def __init__(self, identifier, entries, optional=True):
        assertTrue(all(isinstance(o, CheckedConfigParserSchemaEntry) for o in entries))
        self.identifier = identifier
        self.entries = entries
        self.optional = optional

class CheckedConfigParserSchema(object):
    def __init__(self, sections):
        assertTrue(all(isinstance(o, CheckedConfigParserSchemaSection) for o in sections))
        self.sections = sections

def checkedConfigParserPath(path, **kwargs):
    with open(path, 'r', encoding='utf-8') as f:
        return checkedConfigParser(f.read(), **kwargs)

# wrapper around ConfigParser that 1) doesn't need main section 2) validates schema 3) has better defaults.
def checkedConfigParser(text, schema=None, defaultSectionName='main', autoInsertDefaultSection=True,
    interpolation=None):
    assertTrue(False, 'feature is still under development')
    assertTrue(isPy3OrNewer, 'Py2 not supported, it might have different behavior in ConfigParser')
    from configparser import ConfigParser
    expectSection = '[' + defaultSectionName + ']\n'
    if not (text.startswith(expectSection) or ('\n' + expectSection) in text):
        text = expectSection + text
    
    ret = Bucket()
    p = ConfigParser(strict=True, empty_lines_in_values=True, interpolation=interpolation)
    p.read_string(text)
    
    # check that sections exist
    p.has_section()
    
    return ret
