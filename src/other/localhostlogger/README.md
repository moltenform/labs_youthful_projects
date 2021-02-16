
```
localhostlogger
(c) 2020 moltenform(Ben Fisher)
released under the MIT license

You're testing a complex app that has several moving parts. There's
client code and server code interacting, and they might be written in
different languages or running on different machines. You'd like to
use simple logging like console.log, but to be able to write to the same
log file from any app.

localhostlogger is a simple solution to this problem - start an instance
and it begins to run on port 9123. Now any program can POST some
text to http://localhost:9123/ or to http://<ipaddress>:9123 and it 
will be written, along with the current time, to a single log file.

It's now much easier to, say, trace interactions between server and
client code, because they can both write to the same log.

_____________________________________________________

Usage:
1) python3 localhostlogger.py
2) copy the contents from examples/call_logger.js into your app
3) add a line like await localhostLog("a test statement") to your code

You can also optionally edit localhostlogger.py to change a few settings.

Support for additional languages is easy to add, just send text to
localhost:9123 with POST. If your language can't send web requests
you might still be able to shell out to curl.

Enjoy!
```

