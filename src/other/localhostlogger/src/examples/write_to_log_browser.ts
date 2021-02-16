// write_to_log_browser.ts
// 2020 Ben Fisher, released under the MIT license.

const writeToLog = async (...strings:Array<string>) => {
    const endpoint = 'http://localhost:9123/log';
    const joined = strings.join('; ');
    const text = clientId + ': ' + joined;
    return _sendPost(text, endpoint);
}

const askSetting = async (settingName: string) => {
    const endpoint = 'http://localhost:9123/asksetting';
    const text = '$$$' + settingName + '$$$';
    const ret = await _sendPost(s, endpoint);
    const parts = ret.split(/\$\$\$/g);
    if (parts.length == 3) {
        return parts[1];
    } else {
        throw new Error('Server did not reply to the setting ' + settingName);
    }
}

const _sendPost = async (s: string, endpoint: string) => {
    const headers = new Headers({
        'Content-Type': 'text/plain; charset=utf-8',
    });

    const encoder = new TextEncoder();
    const response = await fetch(endpoint, {  
        method: 'POST',
        headers: headers,
        cache: 'no-cache',
        body: encoder.encode(s)
    })

    if (response.status >= 200 && response.status < 300) {
        return response.text();
    } else {
        throw new Error('status code ' + response.status);
    }
}

const clientId = Math.random().toString().slice(2, 5);
