
#Small timer. Yes, I do know about timeit.

class Minitimer(object):
    def getTime(self):
        import time, sys
        if sys.platform=='win32':
            return time.time() # not ideal...
        else:
            return time.clock()
    def __init__(self):
        self.startedAt = self.getTime()
    def check(self):
        return self.getTime() - self.startedAt
