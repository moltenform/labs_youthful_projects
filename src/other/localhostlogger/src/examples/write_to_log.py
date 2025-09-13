# write_to_log.py
# 2020 Ben Fisher, released under the MIT license.

import requests
import random
import string


def writeToLog(*listText):
    endpoint = 'http://localhost:9123/log'
    text = '; '.join(map(str, listText))
    text = clientId + ': ' + text
    _sendPost(endpoint, text)


def askSetting(settingName):
    endpoint = 'http://localhost:9123/asksetting'
    data = '$$$' + settingName + '$$$'
    r = _sendPost(endpoint, data)
    response = r.text
    parts = response.split('$$$')
    if len(parts) == 3:
        return parts[1]
    else:
        raise Exception('Server did not reply to the setting ' + settingName)


def _sendPost(endpoint, s):
    return requests.post(
        url=endpoint,
        data=s.encode('utf-8'),
        headers={
            'Content-Type': 'text/plain; charset=utf-8'
        }
    )


def getUniqueId():
    return ''.join(random.choice(string.ascii_letters) for i in range(3))


clientId = getUniqueId()
