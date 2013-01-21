

import os
import exceptions
dir = './unittesttemp'
join = os.path.join
exists = os.path.exists


def expectEqual(v, vExpected):
    if v != vExpected:
        print 'Fail: Expected '+str(vExpected)+' but got '+str(v)
        raise exceptions.RuntimeError, 'stop'
    else:
        print 'Pass: '+str(vExpected)+' == '+str(v)
        
def expectStringContains(s, sExpected):
    if not isinstance(s, basestring): raise exceptions.RuntimeError, 'not even a string.'
    if sExpected.lower() not in s.lower():
        print 'Fail: Expected '+str(sExpected)+' within string '+str(s)
        raise exceptions.RuntimeError, 'stop'
    else:
        print 'Pass: the string "'+str(s)+'" contains "'+str(sExpected)+'"'

def expectThrow(fn, sExpectedError, exceptionClass=exceptions.RuntimeError):
    try:
        fn()
    except exceptionClass,e:
        sError = str(e).split('\n')[-1]
        if sExpectedError.lower() in sError.lower():
            print 'Pass:', sExpectedError, ' within ', sError
        else:
            print 'Fail: expected msg', sExpectedError, 'got', sError
    else:
        print 'Fail: expected to throw! '+sExpectedError


def unittestsetup():
    # nuke the unittest directory
    if not os.path.exists(dir): os.mkdir(dir)
    for file in os.listdir(dir):
        if os.path.isdir(join(dir,file)): os.rmdir(join(dir,file))
        else: os.unlink(join(dir,file))
    
    # populate it with new files
    names = '''.testhidden,1,2.gif,3__3_,_4__4.gif,a picture.JPG,first,second.gif,noext another with no ext,noext1,picture 2.jpg,picture and picture.jpG,test.doc,the PiCture with caps.jpg'''.split(',')
    for name in names:
        fout=open(os.path.join(dir, name),'w')
        fout.write('some data some data some data some data')
        fout.close()
    return names

    
def engineunittest():
    from cellrename_engine import renameFiles, marker
    names = unittestsetup()
    
    # test same names
    expectEqual( renameFiles(dir,[names[1],names[2]], [names[1],names[2]]), True)
    
    # test name swap
    fout=open(join(dir,names[1]),'w'); fout.write('testswap'); fout.close()
    expectEqual( renameFiles(dir,[names[1],names[2]], [names[2],names[1]]), True)
    fout=open(join(dir,names[2]),'r'); txt=fout.read(); fout.close()
    assert 'testswap' in txt
    
    # test simple prefix
    namestochange = [names[3],names[4],names[5],names[6]]
    expectEqual( renameFiles(dir,namestochange, ['pref'+name for name in namestochange]), True)
    for name in namestochange: assert exists(join(dir, 'pref'+name))
    expectEqual( renameFiles(dir, ['pref'+name for name in namestochange], namestochange), True)
    for name in namestochange: assert not exists(join(dir, 'pref'+name))
    
    # expect failure, different lengths
    expectStringContains( renameFiles(dir,['a','b','c'], ['a','b']), 'Lengths')
    # expect failure, marker
    expectStringContains( renameFiles(dir,[names[0],names[1]], [names[0],names[1]+marker]), 'its name')
    expectStringContains( renameFiles(dir,[names[0],names[1]+marker], [names[0],names[1]]), 'its name')
    # expect failure, bad filenames
    expectStringContains( renameFiles(dir,['notexist1','notexist2'], ['a','']), 'invalid filename')
    expectStringContains( renameFiles(dir,['notexist1','notexist2'], ['a','ok:one']), 'invalid filename')
    expectStringContains( renameFiles(dir,['notexist1','notexist2'], ['a','some/other']), 'invalid filename')
    expectStringContains( renameFiles(dir,['notexist1','notexist2'], ['a','some\\other']), 'invalid filename')
    # expect failure, duplicates
    expectStringContains( renameFiles(dir,['notexist1','notexist2'], ['a','a']), 'Duplicate')
    expectStringContains( renameFiles(dir,['6','7','8','9'], ['y','x','z','x']), 'Duplicate')
    if os.name=='nt':
        expectStringContains( renameFiles(dir,['6','7','8','9'], ['y','X','z','x']), 'Duplicate')
    
    # expect failure, file exists
    expectStringContains( renameFiles(dir,[names[0],names[1]], [names[0],names[8]]), 'exists')
    os.mkdir(join(dir,'existdir'))
    expectStringContains( renameFiles(dir,[names[0],names[1]], ['existdir',names[1]]), 'exists')
    expectEqual( renameFiles(dir,['existdir'], ['existdirrenamed']), True)
    os.rmdir(join(dir,'existdirrenamed'))
    if os.name=='nt':
        assert names[8].upper()!=names[8]
        expectStringContains( renameFiles(dir,[names[0],names[1]], [names[0],names[8].upper()]), 'exists')
        
    # expect failure, cannot access
    expectStringContains( renameFiles(dir,['notexist1','notexist2'], ['a','b']), 'longer exists')
    expectStringContains( renameFiles(dir,[names[0],'notexist1','notexist2'], [names[0],'a','b']), 'longer exists')
    if os.name=='nt':
        # expectStringContains( renameFiles('c:\\windows',['DirectX.log'], ['DirectX2.log']), 'could not be')
        expectStringContains( renameFiles('c:\\',['Windows'], ['Windows2']), 'could not be')

def dataunittest_files():
    from cellrename_data import CellRenameData
    names = unittestsetup()
    
    def containsName(data, name):
        return any((item.filename==name for item in data.data))
    
    os.mkdir(os.path.join(dir, 'directory1'))
    os.mkdir(os.path.join(dir, 'directory.jpg'))
    os.mkdir(os.path.join(dir, '.hiddendirectory'))
    test1 = CellRenameData(dir, '*', True)
    expectEqual( len(test1.data), len(names)+3-2) #2 begin with . and shouldn't be included
    expectEqual( containsName(test1, 'directory1'), True)
    expectEqual( containsName(test1, 'directory.jpg'), True)
    expectEqual( containsName(test1, '1'), True)
    expectEqual( containsName(test1, 'a picture.JPG'), True)
    expectEqual( containsName(test1, '.hiddendirectory'), False)
    expectEqual( containsName(test1, '.testhidden'), False)
    expectEqual(any((item.filename.startswith('.') for item in test1.data)), False)
    
    test2 = CellRenameData(dir, '*', False)
    expectEqual( containsName(test2, 'directory1'), False)
    expectEqual( containsName(test2, 'directory.jpg'), False)
    expectEqual( containsName(test2, '.testhidden'), False)
    expectEqual( containsName(test2, '.hiddendirectory'), False)
    expectEqual( containsName(test2, '1'), True)
    
    testFilter1 = CellRenameData(dir, '*.*', False)
    expectEqual( containsName(testFilter1, '1'), False)
    expectEqual( containsName(testFilter1, 'a picture.JPG'), True)
    expectEqual( containsName(testFilter1, '2.gif'), True)
    expectEqual( containsName(testFilter1, 'directory.jpg'), False)
    
    testFilter2 = CellRenameData(dir, '*.jp', False)
    expectEqual( containsName(testFilter2, 'a picture.JPG'), False)
    expectEqual( len(testFilter2.data), 0)
    
    testFilter3 = CellRenameData(dir, '*.jpg', False)
    testFilter4 = CellRenameData(dir, '*.JpG', False)
    testFilter5 = CellRenameData(dir, '*.jp*', False)
    testFilter6 = CellRenameData(dir, '*.jp?', False)
    for filter in (testFilter3, testFilter4, testFilter5, testFilter6):
        expectEqual( containsName(filter, 'a picture.JPG'), True)
        expectEqual( containsName(filter, 'picture 2.jpg'), True)
        expectEqual( containsName(filter, 'picture and picture.jpG'), True)
        expectEqual( containsName(filter, 'directory.jpg'), False)
    
    #test sorting
    testSort1 = CellRenameData(dir, '*', True)
    testSort1.sort('filename')
    expectEqual( testSort1.data[0].filename , '1')
    expectEqual( testSort1.data[1].filename , '2.gif')
    expectEqual( testSort1.data[-1].filename , 'the PiCture with caps.jpg')
    testSort1.sort('filename',True) #reverse order
    expectEqual( testSort1.data[-1].filename , '1')
    expectEqual( testSort1.data[-2].filename , '2.gif')
    expectEqual( testSort1.data[0].filename , 'the PiCture with caps.jpg')
    
    fout=open(join(dir, 'a picture.JPG'),'w'); fout.write('a'); fout.close()
    testSort1.refresh()
    testSort1.sort('size')
    expectEqual( testSort1.data[0].filename in ('directory1', 'directory.jpg'), True) #directories always at top
    expectEqual( testSort1.data[1].filename in ('directory1', 'directory.jpg'), True) #directories always at top
    expectEqual( testSort1.data[2].filename , 'a picture.JPG')
    
    # time.sleep(7) # in case the OS is imprecise in stamping file times
    # fout=open(join(dir, 'newfile'),'w'); fout.write('new'); fout.close()
    # testSort2 = CellRenameData(dir, '*', True)
    # testSort2.sort('creationTime', True)
    # expectEqual( testSort2.data[0].filename in ('directory1', 'directory.jpg','newfile'), True)
    # expectEqual( testSort2.data[1].filename in ('directory1', 'directory.jpg','newfile'), True)
    # expectEqual( testSort2.data[2].filename in ('directory1', 'directory.jpg','newfile'), True)
    

def dataunittest_transforms():
    from cellrename_data import CellRenameData, CellRenameItem
    
    def useFakeData(data, a):
        newdata = [CellRenameItem() for s in a]
        for i in range(len(a)): 
            newdata[i].filename=newdata[i].newname=a[i]
        data.data = newdata
    def tostring(data):
        return '|'.join((item.newname for item in data.data))
    
    testNames1 = 'aa|bb.jpg|cc.Ok.jpg|dd.a'.split('|')
    test1 = CellRenameData('.', '*', False)
    expectEqual( len(test1.data)>0, True)
    
    useFakeData(test1, testNames1)
    expectEqual(any((item.filename.endswith('.py') for item in test1.data)), False)
    test1.transformSuffixOrPrefix(False, 'suf')
    expectEqual( tostring(test1), 'aasuf|bbsuf.jpg|cc.Oksuf.jpg|ddsuf.a')
    useFakeData(test1, testNames1)
    test1.transformSuffixOrPrefix(True, 'pref')
    expectEqual( tostring(test1), 'prefaa|prefbb.jpg|prefcc.Ok.jpg|prefdd.a')
    
    useFakeData(test1, testNames1)
    test1.transformAppendNumber('1')
    expectEqual( tostring(test1), 'aa 1|bb 2.jpg|cc.Ok 3.jpg|dd 4.a')
    test1.transformAppendNumber('5')
    expectEqual( tostring(test1), 'aa 1 5|bb 2 6.jpg|cc.Ok 3 7.jpg|dd 4 8.a')
    useFakeData(test1, testNames1)
    test1.transformAppendNumber('000')
    expectEqual( tostring(test1), 'aa 000|bb 001.jpg|cc.Ok 002.jpg|dd 003.a')
    useFakeData(test1, testNames1)
    test1.transformAppendNumber('0098')
    expectEqual( tostring(test1), 'aa 0098|bb 0099.jpg|cc.Ok 0100.jpg|dd 0101.a')
    
    useFakeData(test1, testNames1)
    test1.transformWithPattern('s')
    expectEqual( tostring(test1), 's|s.jpg|s.jpg|s.a')
    useFakeData(test1, testNames1)
    test1.transformWithPattern('a%na%n %u%u%f')
    expectEqual( tostring(test1), 'a1a1 aaaaaa|a2a2 bbbbbb.jpg|a3a3 cc.okcc.okcc.Ok.jpg|a4a4 dddddd.a')
    useFakeData(test1, testNames1)
    test1.transformWithPattern('%N(%U)%N')
    expectEqual( tostring(test1), '1(AA)1|2(BB)2.jpg|3(CC.OK)3.jpg|4(DD)4.a')
    
    useFakeData(test1, testNames1)
    test1.transformReplace('A','C')
    test1.transformReplace('.jpg','.png') #unlike the rest, this *should* be able to rename extensions
    expectEqual( tostring(test1),'aa|bb.png|cc.Ok.png|dd.a')
    
    test2 = CellRenameData('.', '*', False)
    testNames2 = 'a(a|test(Athat|first,second|xx,yy,zz|cc.Ok.jpg|ccnOk.jpg|dd.a'.split('|')
    useFakeData(test2, testNames2)
    test2.transformRegexReplace('(a', '_a', False, False) #not regex, case insensitive
    expectEqual( tostring(test2), 'a_a|test_athat|first,second|xx,yy,zz|cc.Ok.jpg|ccnOk.jpg|dd.a')
    useFakeData(test2, testNames2)
    test2.transformRegexReplace('(a', '_a', False, True) #not regex, case sensitive
    expectEqual( tostring(test2), 'a_a|test(Athat|first,second|xx,yy,zz|cc.Ok.jpg|ccnOk.jpg|dd.a')
    useFakeData(test2, testNames2)
    test2.transformRegexReplace('cc.Ok', 'match', False, False) #not regex, case sensitive
    expectEqual( tostring(test2), 'a(a|test(Athat|first,second|xx,yy,zz|match.jpg|ccnOk.jpg|dd.a')
    useFakeData(test2, testNames2)
    test2.transformRegexReplace('cc.Ok', 'match', True, False) #is regex, case sensitive
    expectEqual( tostring(test2), 'a(a|test(Athat|first,second|xx,yy,zz|match.jpg|match.jpg|dd.a')
    useFakeData(test2, testNames2)
    test2.transformRegexReplace(r'(\w+),(\w+)', r'\2-\1', True, False) #is regex, case sensitive
    expectEqual( tostring(test2), 'a(a|test(Athat|second-first|yy-xx,zz|cc.Ok.jpg|ccnOk.jpg|dd.a')

def runall():
    engineunittest()
    dataunittest_transforms()
    dataunittest_files()
    
if __name__=='__main__':
    runall()
    