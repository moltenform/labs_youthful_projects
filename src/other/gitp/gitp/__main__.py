# Ben Fisher
# Copyright (c) 2021, MIT License

# See README.md for more information

from typing import OrderedDict
from .gitp_impl import *

def main(argv):
    sawGitpException = False
    try:
        checkOtherInstancesWhenStarting()
        cmds = OrderedDict(
            diffdev=gitpTop_DiffMain,
            diffprev=gitpTop_DiffPrev,
            copytoaltrepo=gitpTop_CopyToAltRepo,
            slowcopytoaltrepo=gitpTop_SlowCopyToAltRepo,
            pack=gitpTop_Pack,
            apply=gitpTop_Apply_ApplyAndCheck,
            cdalt=gitpTop_CdAltRepo,
            recent=gitpTop_ViewRecent,
            runprettier=gitpTop_RunCommitHook,
            updatebasis=gitpTop_UpdateBasis,
            resethardall=gitpTop_ResetHardAll,
            seepacketdiff=gitpTop_ShowPacketDiff,
            seepacketinfo=lambda: gitpTop_ShowDescription(False),
            seepacketinfoverbose=lambda: gitpTop_ShowDescription(True),
        )
        
        infoText = OrderedDict(
            diffdev=' (shows diff against main branch, aka "ddd")',
            diffprev=' (shows diff against prev commit on branch)',
            copytoaltrepo=' (copy files to the other repo)',
        )

        root = argv[1] if len(argv) > 1 else ''
        cmd = argv[2] if len(argv) > 2 else ''
        os.chdir(root)
        fn = cmds.get(cmd)
        if fn:
            fn()
        else:
            help = "gitp, a tool to store & back up the changes you're not ready to push to a server.\n"
            help += "Ben Fisher, 2021\n\n"
            help += 'Syntax:\n'
            for k in cmds:
                info = infoText.get(k, '')
                help += f'gitp {k}{info}\n'
            trace(help)
    except GitPacketException as e:
        # we'll raise GitPacketException when stopping the script intentionally.
        sys.stderr.write(str(e) + '\n')
        sawGitpException = True
        
    # having a GitPacketException currently still counts as a clean exit.
    markCleanExitWhenEnding()
    if sawGitpException:
        sys.exit(1)

if __name__ == '__main__':
    main(sys.argv)

