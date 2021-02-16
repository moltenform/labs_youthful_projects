# BenPythonCommon,
# 2020 Ben Fisher, released under the GPLv3 license.

import pytest
from .. import common_jslike as jslike
from .. import Bucket

class TestJslikeArrayMethods(object):
    def test_concatBasic(self):
        assert jslike.concat([], []) == []
        assert jslike.concat([1], []) == [1]
        assert jslike.concat([], [1]) == [1]
        assert jslike.concat([1, 2], []) == [1, 2]
        assert jslike.concat([], [1, 2]) == [1, 2]
        assert jslike.concat([1, 2], [3, 4]) == [1, 2, 3, 4]
        assert jslike.concat([1, 2], [3, 4, 5]) == [1, 2, 3, 4, 5]

    def test_concatShouldNotModify(self):
        a = [1, 2]
        b = [3, 4]
        assert jslike.concat(a, b) == [1, 2, 3, 4]
        assert a == [1, 2]
        assert b == [3, 4]

    def test_every(self):
        fn = lambda x: x > 0
        assert jslike.every([], fn) is True
        assert jslike.every([1], fn) is True
        assert jslike.every([1, 2], fn) is True
        assert jslike.every([1, 2, 0], fn) is False
        assert jslike.every([1, 0, 2], fn) is False
        assert jslike.every([0], fn) is False
        assert jslike.every([0, 0], fn) is False
        assert jslike.every([0, 0, 1], fn) is False

    def test_some(self):
        fn = lambda x: x > 0
        assert jslike.some([], fn) is False
        assert jslike.some([1], fn) is True
        assert jslike.some([1, 2], fn) is True
        assert jslike.some([1, 2, 0], fn) is True
        assert jslike.some([1, 0, 2], fn) is True
        assert jslike.some([0], fn) is False
        assert jslike.some([0, 0], fn) is False
        assert jslike.some([0, 0, 1], fn) is True

    def test_filter(self):
        fn = lambda x: x > 0
        assert jslike.filter([], fn) == []
        assert jslike.filter([1], fn) == [1]
        assert jslike.filter([1, 2], fn) == [1, 2]
        assert jslike.filter([1, 2, 0], fn) == [1, 2]
        assert jslike.filter([1, 0, 2], fn) == [1, 2]
        assert jslike.filter([0], fn) == []
        assert jslike.filter([0, 0], fn) == []
        assert jslike.filter([0, 0, 1], fn) == [1]

    def test_find(self):
        fn = lambda x: x > 0
        assert jslike.find([], fn) is None
        assert jslike.find([1], fn) == 1
        assert jslike.find([1, 2], fn) == 1
        assert jslike.find([1, 2, 0], fn) == 1
        assert jslike.find([1, 0, 2], fn) == 1
        assert jslike.find([2, 0, 1], fn) == 2
        assert jslike.find([0], fn) is None
        assert jslike.find([0, 0], fn) is None
        assert jslike.find([0, 0, 1], fn) == 1

    def test_findIndex(self):
        fn = lambda x: x > 0
        assert jslike.findIndex([], fn) == -1
        assert jslike.findIndex([1], fn) == 0
        assert jslike.findIndex([1, 2], fn) == 0
        assert jslike.findIndex([1, 2, 0], fn) == 0
        assert jslike.findIndex([1, 0, 2], fn) == 0
        assert jslike.findIndex([2, 0, 1], fn) == 0
        assert jslike.findIndex([0], fn) == -1
        assert jslike.findIndex([0, 0], fn) == -1
        assert jslike.findIndex([0, 0, 1], fn) == 2

    def test_indexOf(self):
        assert jslike.indexOf([], 1) == -1
        assert jslike.indexOf([1], 1) == 0
        assert jslike.indexOf([1, 2], 1) == 0
        assert jslike.indexOf([1, 2, 0], 1) == 0
        assert jslike.indexOf([1, 0, 2], 1) == 0
        assert jslike.indexOf([1, 0, 1], 1) == 0
        assert jslike.indexOf([2, 0, 1], 1) == 2
        assert jslike.indexOf([0], 1) == -1
        assert jslike.indexOf([0, 0], 1) == -1
        assert jslike.indexOf([0, 0, 1], 1) == 2

    def test_lastIndexOf(self):
        assert jslike.lastIndexOf([], 1) == -1
        assert jslike.lastIndexOf([1], 1) == 0
        assert jslike.lastIndexOf([1, 2], 1) == 0
        assert jslike.lastIndexOf([1, 2, 0], 1) == 0
        assert jslike.lastIndexOf([1, 0, 2], 1) == 0
        assert jslike.lastIndexOf([1, 0, 1], 1) == 2
        assert jslike.lastIndexOf([2, 0, 1], 1) == 2
        assert jslike.lastIndexOf([0], 1) == -1
        assert jslike.lastIndexOf([0, 0], 1) == -1
        assert jslike.lastIndexOf([0, 0, 1], 1) == 2

    def test_map(self):
        fn = lambda x: x + 10
        assert jslike.map([], fn) == []
        assert jslike.map([1], fn) == [11]
        assert jslike.map([1, 2], fn) == [11, 12]

    def test_mapShouldReturnList(self):
        fn = lambda x: x + 10
        input = [1, 2]
        out = jslike.map(input, fn)
        assert isinstance(out, list)
        assert input == [1, 2]
        assert out == [11, 12]

    def test_times(self):
        fn = lambda: 1
        assert jslike.times(0, fn) == []
        assert jslike.times(1, fn) == [1]
        assert jslike.times(5, fn) == [1, 1, 1, 1, 1]

        context = Bucket(counter=0)

        def incCounter():
            context.counter += 1
            return context.counter
        assert jslike.times(0, incCounter) == []
        assert jslike.times(1, incCounter) == [1]
        assert jslike.times(5, incCounter) == [2, 3, 4, 5, 6]

    def test_reduceBasic(self):
        fn = lambda initialVal, head: initialVal + head
        assert jslike.reduce([2], fn) == 2
        assert jslike.reduce([2, 3], fn) == 5
        assert jslike.reduce([2, 3, 4], fn) == 9
        with pytest.raises(Exception):
            jslike.reduce([], fn)

    def test_reduceWithInitialVal(self):
        fn = lambda initialVal, head: initialVal + head
        assert jslike.reduce([], fn, 1) == 1
        assert jslike.reduce([2], fn, 1) == 3
        assert jslike.reduce([2, 3], fn, 1) == 6
        assert jslike.reduce([2, 3, 4], fn, 1) == 10

    def test_reduceMultiply(self):
        fn = lambda initialVal, head: initialVal * head
        assert jslike.reduce([2], fn) == 2
        assert jslike.reduce([2, 3], fn) == 6
        assert jslike.reduce([2, 3, 4], fn) == 24
        with pytest.raises(Exception):
            jslike.reduce([], fn)

    def test_reduceMultiplyWithInitialVal(self):
        fn = lambda initialVal, head: initialVal * head
        assert jslike.reduce([2], fn, 2) == 4
        assert jslike.reduce([2, 3], fn, 2) == 12
        assert jslike.reduce([2, 3, 4], fn, 2) == 48
        assert jslike.reduce([], fn, 2) == 2

    def test_reduceMultiplyWithZero(self):
        fn = lambda initialVal, head: initialVal * head
        assert jslike.reduce([2], fn, 0) == 0
        assert jslike.reduce([2, 3], fn, 0) == 0
        assert jslike.reduce([2, 3, 4], fn, 0) == 0
        assert jslike.reduce([], fn, 0) == 0

    def test_reduceRecorded(self):
        def fn(initialVal, head):
            return 'initialVal=%s;head=%s;' % \
                (initialVal, head)
        assert jslike.reduce([2], fn) == 2
        assert jslike.reduce([2, 3], fn) == 'initialVal=2;head=3;'
        assert jslike.reduce([2, 3, 4], fn) == 'initialVal=initialVal=2;head=3;;head=4;'
        with pytest.raises(Exception):
            jslike.reduce([], fn)

class TestJslikeStringMethods(object):
    def test_splice(self):
        assert 'abef' == jslike.splice('abcdef', 2, 2, '')
        assert 'ab1ef' == jslike.splice('abcdef', 2, 2, '1')
        assert 'ab12ef' == jslike.splice('abcdef', 2, 2, '12')
        assert 'ab123ef' == jslike.splice('abcdef', 2, 2, '123')
        assert 'ab123def' == jslike.splice('abcdef', 2, 1, '123')
        assert 'ab123cdef' == jslike.splice('abcdef', 2, 0, '123')

class TestJslikeDictMethods(object):
    def test_compareDict(self):
        assert dict() == dict()
        assert dict(a=1, b=2) == dict(a=1, b=2)
        assert dict(b=2, a=1) == dict(a=1, b=2)
        assert dict(a=1, b=2) != dict(a=1, b=3)
        assert dict(a=1, b=2) != dict(a=1, aa=2)
        assert dict(a=1, b=2) != dict(a=1, b=2, c=3)
        assert dict(a=1, b=2) != dict(a=1)

    def test_mergedBasic(self):
        assert jslike.merged(dict(), dict()) == dict()
        assert jslike.merged(dict(a=1), dict()) == dict(a=1)
        assert jslike.merged(dict(), dict(a=1)) == dict(a=1)
        assert jslike.merged(dict(a=1), dict(a=2)) == dict(a=2)
        assert jslike.merged(dict(a=2), dict(a=1)) == dict(a=1)
        assert jslike.merged(dict(a=1), dict(a=2, b=3)) == dict(a=2, b=3)
        assert jslike.merged(dict(a=2), dict(a=1, b=3)) == dict(a=1, b=3)
        assert jslike.merged(dict(a=1, b=3), dict(a=1, b=3)) == dict(a=1, b=3)
        assert jslike.merged(dict(a=1, b=3), dict(a=4, b=5)) == dict(a=4, b=5)
        assert jslike.merged(dict(a=1, b=3), dict(a=1)) == dict(a=1, b=3)
        assert jslike.merged(dict(a=1, b=3), dict(a=2)) == dict(a=2, b=3)
        assert jslike.merged(dict(a=1, b=3), dict()) == dict(a=1, b=3)
        assert jslike.merged(dict(a=1), dict(b=3)) == dict(a=1, b=3)

    def test_mergedShouldNotModify(self):
        a = dict(a=1, b=2, c=1)
        b = dict(a=3, b=2)
        out = jslike.merged(a, b)
        assert a == dict(a=1, b=2, c=1)
        assert b == dict(a=3, b=2)
        assert out == dict(a=3, b=2, c=1)
