# Ben Fisher
# Copyright (c) 2021, MIT License

import random
import os
from .gitp_impl import *
from shinerainsoftsevenutil.standard import * # auto-transitioned
from shinerainsoftsevenutil.core import *

def createManyRandomFiles(dir, seed):
    random.seed(seed)
    files.ensureEmptyDirectory(dir)
    makeTestFiles(dir + '/src', 5, ['.ts', '.js', '.json'], binary=False)
    makeTestFiles(dir + '/src', 5, ['.png', '.gif'], binary=True)
    for subdir in ['src/a1/a1a', 'src/a1/a1b', 'src/a2/a2a', 'src/a2/a2b', 'src/a2/a2c/a2c2']:
        makeTestFiles(dir + '/' + subdir, 3, ['.ts', '.js', '.json'], binary=False)
        makeTestFiles(dir + '/' + subdir, 1, ['.png', '.gif'], binary=True)

def makeTestFiles(dir, nFiles, exts, binary):
    for _ in range(nFiles):
        makeTestFile(dir, exts, binary)

def makeTestFile(dir, exts, binary, append=False):
    files.makeDirs(dir)
    path = f"{dir}/{random.choice(wordlist)}-{random.choice(wordlist)}{random.choice(exts)}".lower()
    def getWord():
        s = random.choice(wordlist)
        if random.randint(1, 100) > 95:
            s += random.choice(unicodeChars)
        return s
    if binary:
        filesize = random.choice([random.randint(1100, 1800), random.randint(2100, 3100)])
        with open(path, 'ab' if append else 'wb') as fout:
            for n in range(filesize):
                fout.write(bytes([n % 256]))
                if random.randint(1, 100) >= 98:
                    fout.write(bytes([random.randint(0, 255)]))
    else:
        makeLines = random.choice([random.randint(100, 300), random.randint(1000, 2000)])
        with open(path, 'a' if append else 'w', encoding='utf-8') as fout:
            for _ in range(makeLines):
                linelength = 3 # if random.randint(1, 100) < 99 else random.randint(50, 150)
                text = ' '.join(jslike.times(linelength, getWord))
                fout.write(text)
                fout.write('\n')
    return path

def changeFile(path, tmpdir, binary=False):
    # because we're operating on full lines, we won't interfere with utf8 sequences
    split = lambda data: splitBytesBySize(data, random.choice((50, 500))) if binary else data.split(b'\n')
    join = lambda lst: b''.join(lst) if binary else b'\n'.join(lst)
    tempSrcDataPath = makeTestFile(tmpdir, [files.getExt(path)], binary=binary)
    tempSrcData = files.readAll(tempSrcDataPath, 'rb')
    srcLines = split(tempSrcData)
    destBytes = files.readAll(path, 'rb')
    destLines = split(destBytes)
    hasDoneOne = False
    for _ in range(random.randint(1, 5)):
        op = random.choice(['add', 'delete', 'modify'])
        if len(destLines) <= 1:
            break
        hasDoneOne = True
        destTarget = random.randint(1, len(destLines) - 1)
        srcTarget = random.randint(1, len(srcLines) - 1)
        newText = join(srcLines[srcTarget:srcTarget+random.randint(1,3)])
        if op == 'add':
            destLines.insert(destTarget, newText)
        elif op == 'modify':
            destLines[destTarget:destTarget+random.randint(1,3)] = [newText]
        elif op == 'delete':
            destLines.pop(destTarget)
    files.writeAll(path, join(destLines), 'wb')

def makeChanges(dir, tmpdir, nAdds=2, nModifies=3, nDeletes=1):
    with ChangeCurrentDirectory(dir) as cd:
        for isBinary in [True, False]:
            allTs = [f for (f,short) in files.recurseFiles(dir) if f.endswith('.ts')]
            allPng = [f for (f,short) in files.recurseFiles(dir) if f.endswith('.png')]
            exts = ['.png', '.gif'] if isBinary else ['.ts', '.js']
            arFiles = allPng if isBinary else allTs
            for i in range(nAdds):
                path = 'src/a2/a2a' if i > 0 else 'src/new'
                makeTestFile(path, exts, binary=isBinary)
            for i in range(nModifies):
                changeFile(random.choice(arFiles), tmpdir, binary=isBinary)
            for i in range(nDeletes):
                files.deleteSure(random.choice(arFiles))

def getTestSetup(testDir, forceRebuild=False, seed=123):
    os.chdir(testDir)
    outDir, tmpDir = getOutDirs()
    model = os.path.abspath('testtmpdata/modeltestrepo1')
    tmpmodel = os.path.abspath('testtmpdata/modeltestrepo1b')
    root = os.path.abspath('testtmpdata/testrepo1')
    tmproot = os.path.abspath('testtmpdata/testrepo1b')
    if not forceRebuild and files.exists('testtmpdata/done.txt'):
        files.ensureEmptyDirectory(root)
        files.ensureEmptyDirectory(tmproot)
        files.runRsync(model, root, deleteExisting=True)
        files.runRsync(tmpmodel, tmproot, deleteExisting=True)
        workingRepos.append(root)
        tempRepos[root] = tmproot
        with ChangeCurrentDirectory(root) as cd:
            basisCommits[root] = currentCommitId()
        return
    elif forceRebuild:
        files.deleteSure('testtmpdata/done.txt')
    
    createManyRandomFiles(root, seed=seed)
    with ChangeCurrentDirectory(root) as cd:
        getGitResults('git_init')
        getGitResults('git_co_-b', [mainBranch(root)])
        getGitResults('git_add_-A')
        getGitResults('git_commit_-m_initialcommit')
        for i in range(5):
            makeChanges(root, tmpDir)
            getGitResults('git_add_-A')
            getGitResults(f'git_commit_-m', [f'c{i}\nmore info c{i}'])
        basis = currentCommitId()
        for i in range(2):
            makeChanges(root, tmpDir)
            getGitResults('git_add_-A')
            getGitResults(f'git_commit_-m_othermainchanges{i}')
    files.makeDirs(tmproot)
    files.runRsync(root, tmproot, deleteExisting=True)
    workingRepos.append(root)
    tempRepos[root] = tmproot
    basisCommits[root] = basis
    # set up tmproot
    with ChangeCurrentDirectory(tmproot) as cd:
        getGitResults('git_co', [basis])
        getGitResults('git_co_-b_cleanworkonmain')

    # set up root
    with ChangeCurrentDirectory(root) as cd:
        # main has one set of changes that are unique
        getGitResults('git_co', [basis])
        getGitResults('git_co_-b_gpworking')
        makeChanges(root, tmpDir)
        with ChangeCurrentDirectory(tmproot) as cd:
            files.runRsync(root + '/src', tmproot + '/src', deleteExisting=True)
            assertGitPacket(not isPendingMerge(), f'pending merge or rebase in {tmproot}?')
            assertGitPacket(not areThereStagedFiles(), f'should not have staged files in {tmproot}')
            getGitResults('git_add_-A')
            getGitResults('git_commit_-m_cleanworkonmain1')
        makeChanges(root, tmpDir)
    
    files.ensureEmptyDirectory(model)
    files.ensureEmptyDirectory(tmpmodel)
    files.runRsync(root, model, deleteExisting=True)
    files.runRsync(tmproot, tmpmodel, deleteExisting=True)
    files.writeAll('testtmpdata/done.txt', 'done')

def getDirSummary(d):
    fls = sorted(f for f, short in files.recurseFiles(d))
    hashes = jslike.map(fls, lambda f: files.getName(f) + ': ' + getHashNormalized(f))
    return fls, hashes

def getHashNormalized(path):
    import zlib
    crc = zlib.crc32(bytes(), 0)
    with open(path, 'rb') as f:
        data = f.read().replace(b'\r\n', b'\n').strip()
    crc = zlib.crc32(data, crc) & 0xffffffff
    return '%08x' % crc

def compareTwoDirs(d1, d2):
    fls1, hashes1 = getDirSummary(d1)
    fls2, hashes2 = getDirSummary(d2)
    if fls1 != fls2:
        warn('Different files: ', fls1, fls2)
    if hashes1 != hashes2:
        warn('Different hashes: ', hashes1, hashes2)

def testPackAndApply(testDir):
    outDir, tmpDir = getOutDirs()
    for f, short in list(files.listFiles(outDir)):
        if 'ztestgitptest' in short:
            files.delete(f)
    for i, seed in enumerate([123, 124, 125, 126, 127]):
        getTestSetup(testDir, True, seed=seed)
        root = testDir + '/testtmpdata/testrepo1'
        with ChangeCurrentDirectory(root) as cd:
            gitpTop_Pack('ztestgitptest', 'short_desc:long_desc:more')
            n = '%03d' % (i + 1)
            expectZip = outDir + f'/ztestgitptest_{n}_short_desc.gitp.zip'
            assertTrue(files.exists(expectZip), expectZip)
            # check metadata
            fullpathtozip, zip = askUserForPathToGitp(fullpathtozip)
            with zip:
                manifest = getManifestJsonFromZip(zip, 'manifest.txt')
                assertEq(1, manifest['gitp'])
                assertEq('0,1', manifest['gitpversion'])
                assertEq('ztestgitptest:short_desc:long_desc:more', manifest['description'])
                assertTrue(shasMatch(currentCommitId(), manifest['basis']))
                assertEq(expectZip, manifest['packname'])
                assertEq('gpworking', manifest['branch1'])
                assertEq(root, manifest['fullpath1'])
                assertEq('cleanworkonmain', manifest['branch2'])
                assertEq(root + 'b', manifest['fullpath2'])
                trace('time=', manifest['time'])
                trace('humantime=', manifest['humantime'])
                assertEq('c4\nmore info c4', manifest['adjacentCommitsMsg1'])
                assertEq('c3\nmore info c3', manifest['adjacentCommitsMsg2'])
                assertEq('c2\nmore info c2', manifest['adjacentCommitsMsg3'])
                changes = getUnstagedFiles()
                assertTrue(len(changes.modified) > 1)
                assertTrue(len(changes.added) > 1)
                assertTrue(len(changes.deleted) > 1)
                assertEq(sorted(list(changes.modified.keys())), manifest['modified'])
                assertEq(sorted(list(changes.added.keys())), manifest['added'])
                assertEq(sorted(list(changes.deleted.keys())), manifest['deleted'])
            # try applying it
            fls1, hashes1 = getDirSummary(root + '/src')
            getGitResults('git_reset_--hard')
            getGitResults('git_clean_-fdx')
            assertTrue(not areThereStagedFiles())
            assertTrue(not areThereUnstagedFiles())
            flsClean, hashesClean = getDirSummary(root + '/src')
            assertTrue(flsClean != fls1)
            assertTrue(hashesClean != hashes1)
            gitpTop_Apply_ApplyAndCheck(expectZip, alwaysToRoot=True)
            fls2, hashes2 = getDirSummary(root + '/src')
            assertEq(fls1, fls2)
            assertEq(hashes1, hashes2)
            trace('Applied and confirmed equal.')
            

def testGitpTop_ShowDescription():
    gitpTop_ShowDescription(False)

def testGitpTop_ShowDescriptionVerbose():
    gitpTop_ShowDescription(True)

wordlist = "a;ability;able;about;above;accept;according;account;across;act;action;activity;actually;add;address;administration;admit;adult;affect;after;again;against;age;agency;agent;ago;agree;agreement;ahead;air;all;allow;almost;alone;along;already;also;although;always;American;among;amount;analysis;and;animal;another;answer;any;anyone;anything;appear;apply;approach;area;argue;arm;around;arrive;art;article;artist;as;ask;assume;at;attack;attention;attorney;audience;author;authority;available;avoid;away;baby;back;bad;bag;ball;bank;bar;base;be;beat;beautiful;because;become;bed;before;begin;behavior;behind;believe;benefit;best;better;between;beyond;big;bill;billion;bit;black;blood;blue;board;body;book;born;both;box;boy;break;bring;brother;budget;build;building;business;but;buy;by;call;camera;campaign;can;cancer;candidate;capital;car;card;care;career;carry;case;catch;cause;cell;center;central;century;certain;certainly;chair;challenge;chance;change;character;charge;check;child;choice;choose;church;citizen;city;civil;claim;class;clear;clearly;close;coach;cold;collection;college;color;come;commercial;common;community;company;compare;computer;concern;condition;conference;Congress;consider;consumer;contain;continue;control;cost;could;country;couple;course;court;cover;create;crime;cultural;culture;cup;current;customer;cut;dark;data;daughter;day;dead;deal;death;debate;decade;decide;decision;deep;defense;degree;Democrat;democratic;describe;design;despite;detail;determine;develop;development;die;difference;different;difficult;dinner;direction;director;discover;discuss;discussion;disease;do;doctor;dog;door;down;draw;dream;drive;drop;drug;during;each;early;east;easy;eat;economic;economy;edge;education;effect;effort;eight;either;election;else;employee;end;energy;enjoy;enough;enter;entire;environment;environmental;especially;establish;even;evening;event;ever;every;everybody;everyone;everything;evidence;exactly;example;executive;exist;expect;experience;expert;explain;eye;face;fact;factor;fail;fall;family;far;fast;father;fear;federal;feel;feeling;few;field;fight;figure;fill;film;final;finally;financial;find;fine;finger;finish;fire;firm;first;fish;five;floor;fly;focus;follow;food;foot;for;force;foreign;forget;form;former;forward;four;free;friend;from;front;full;fund;future;game;garden;gas;general;generation;get;girl;give;glass;go;goal;good;government;great;green;ground;group;grow;growth;guess;gun;guy;hair;half;hand;hang;happen;happy;hard;have;he;head;health;hear;heart;heat;heavy;help;her;here;herself;high;him;himself;his;history;hit;hold;home;hope;hospital;hot;hotel;hour;house;how;however;huge;human;hundred;husband;I;idea;identify;if;image;imagine;impact;important;improve;in;include;including;increase;indeed;indicate;individual;industry;information;inside;instead;institution;interest;interesting;international;interview;into;investment;involve;issue;it;item;its;itself;job;join;just;keep;key;kid;kill;kind;kitchen;know;knowledge;land;language;large;last;late;later;laugh;law;lawyer;lay;lead;leader;learn;least;leave;left;leg;legal;less;let;letter;level;lie;life;light;like;likely;line;list;listen;little;live;local;long;look;lose;loss;lot;love;low;machine;magazine;main;maintain;major;majority;make;man;manage;management;manager;many;market;marriage;material;matter;may;maybe;me;mean;measure;media;medical;meet;meeting;member;memory;mention;message;method;middle;might;military;million;mind;minute;miss;mission;model;modern;moment;money;month;more;morning;most;mother;mouth;move;movement;movie;Mr;Mrs;much;music;must;my;myself;name;nation;national;natural;nature;near;nearly;necessary;need;network;never;new;news;newspaper;next;nice;night;no;none;nor;north;not;note;nothing;notice;now;n't;number;occur;of;off;offer;office;officer;official;often;oh;oil;ok;old;on;once;one;only;onto;open;operation;opportunity;option;or;order;organization;other;others;our;out;outside;over;own;owner;page;pain;painting;paper;parent;part;participant;particular;particularly;partner;party;pass;past;patient;pattern;pay;peace;people;per;perform;performance;perhaps;period;person;personal;phone;physical;pick;picture;piece;place;plan;plant;play;player;PM;point;police;policy;political;politics;poor;popular;population;position;positive;possible;power;practice;prepare;present;president;pressure;pretty;prevent;price;private;probably;problem;process;produce;product;production;professional;professor;program;project;property;protect;prove;provide;public;pull;purpose;push;put;quality;question;quickly;quite;race;radio;raise;range;rate;rather;reach;read;ready;real;reality;realize;really;reason;receive;recent;recently;recognize;record;red;reduce;reflect;region;relate;relationship;religious;remain;remember;remove;report;represent;Republican;require;research;resource;respond;response;responsibility;rest;result;return;reveal;rich;right;rise;risk;road;rock;role;room;rule;run;safe;same;save;say;scene;school;science;scientist;score;sea;season;seat;second;section;security;see;seek;seem;sell;send;senior;sense;series;serious;serve;service;set;seven;several;sex;sexual;shake;share;she;shoot;short;shot;should;shoulder;show;side;sign;significant;similar;simple;simply;since;sing;single;sister;sit;site;situation;six;size;skill;skin;small;smile;so;social;society;soldier;some;somebody;someone;something;sometimes;son;song;soon;sort;sound;source;south;southern;space;speak;special;specific;speech;spend;sport;spring;staff;stage;stand;standard;star;start;state;statement;station;stay;step;still;stock;stop;store;story;strategy;street;strong;structure;student;study;stuff;style;subject;success;successful;such;suddenly;suffer;suggest;summer;support;sure;surface;system;table;take;talk;task;tax;teach;teacher;team;technology;television;tell;ten;tend;term;test;than;thank;that;the;their;them;themselves;then;theory;there;these;they;thing;think;third;this;those;though;thought;thousand;threat;three;through;throughout;throw;thus;time;to;today;together;tonight;too;top;total;tough;toward;town;trade;traditional;training;travel;treat;treatment;tree;trial;trip;trouble;true;truth;try;turn;TV;two;type;under;understand;unit;until;up;upon;us;use;usually;value;various;very;victim;view;violence;visit;voice;vote;wait;walk;wall;want;war;watch;water;way;we;weapon;wear;week;weight;well;west;western;what;whatever;when;where;whether;which;while;white;who;whole;whom;whose;why;wide;wife;will;win;wind;window;wish;with;within;without;woman;wonder;word;work;worker;world;worry;would;write;writer;wrong;yard;yeah;year;yes;yet;you;young;your;yourself".split(';')
unicodeChars = []
unicodeChars += list(range(0xa2, 0xd4)) # latin symbols
unicodeChars += list(range(0x13A8, 0x13f4)) # cherokee alphabet
unicodeChars += list(range(0x01D49E, 0x01D4AC)) # math characters, outside BMP
unicodeChars = jslike.map(unicodeChars, lambda n: chr(n))

