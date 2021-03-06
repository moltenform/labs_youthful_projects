// write_to_log_node.ts
// 2020 Ben Fisher, released under the MIT license.

const http = require("http");
const process = require("process");

export const writeToLog = async (...strings:Array<any>) => {
    const hostname = 'localhost';
    const path = '/log';
    const port = 9123;
    const joined = strings.map(o => o.toString()).join('; ');
    const text = clientId + ': ' + joined;
    return _sendPost(text, hostname, path, port);
}

export const askSetting = async (settingName: string) => {
    const hostname = 'localhost';
    const path = '/asksetting';
    const port = 9123;
    const text = '$$$' + settingName + '$$$';
    const ret = await _sendPost(text, hostname, path, port);
    const parts = ret.split(/\$\$\$/g);
    if (parts.length == 3) {
        return parts[1];
    } else {
        throw new Error('Server did not reply to the setting ' + settingName);
    }
}

const _sendPost = async (toSend: string, hostname: string, path: string, port: number): Promise<string> => {
    return new Promise((resolve, reject) => {
        var options = {
            hostname: hostname,
            path: path,
            port: port,
            method: "POST",
            headers: {
                "Content-Type": "text/plain; charset=utf-8",
                "Content-Length":  Buffer.byteLength(toSend)
            },
        };

        var req = http.request(options, (res) => {
            res.setEncoding('utf8');
            const statusCode = res.statusCode;
            const headers = res.headers;
            let gotData = "";
            res.on("data", (d) => {
                gotData += d;
            });

            res.on("end", () => {
                if (res.statusCode < 200 || res.statusCode >= 300) {
                    reject(new Error("failed with status " + res.statusCode));
                } else {
                    resolve(gotData);
                }
            });
        });

        req.on("error", (e) => {
            reject(e);
        });

        req.write(toSend);
        req.end();
    });
}

const clientId = Math.random().toString().slice(2, 5);
