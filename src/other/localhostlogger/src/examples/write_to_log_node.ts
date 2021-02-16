// write_to_log_node.ts
// 2020 Ben Fisher, released under the MIT license.

const http = require("http");
const process = require("process");

const writeToLog = async (...strings:Array<string>) => {
    const hostname = 'localhost';
    const path = '/log';
    const port = 9123;
    const joined = strings.join('; ');
    const text = clientId + ': ' + joined;
    return _sendPost(text, hostname, path, port);
}

const askSetting = async (settingName: string) => {
    const hostname = 'localhost';
    const path = '/asksetting';
    const port = 9123;
    const text = '$$$' + settingName + '$$$';
    const ret = await _sendPost(s, hostname, path, port);
    const parts = ret.split(/\$\$\$/g);
    if (parts.length == 3) {
        return parts[1];
    } else {
        throw new Error('Server did not reply to the setting ' + settingName);
    }
}

const _sendPost = async (s: string, hostname: string, path: string, port: number): Promise<string> => {
    return new Promise((resolve, reject) => {
        var options = {
            hostname: hostname,
            port: port,
            path: path,
            method: "POST",
            headers: {
                "Content-Type": "text/plain; charset=utf-8"
            },
        };

        var req = http.request(options, (res) => {
            const statusCode = res.statusCode;
            const headers = res.headers;
            let gotData = "";
            res.on("data", (d) => {
                gotData += d;
            });

            res.on("end", () => {
                if (statusCode < 200 || statusCode >= 300) {
                    return reject(new Error("failed with status " + statusCode));
                } else {
                    resolve(gotData);
                }
            });
        });

        req.on("error", (e) => {
            reject(e);
        });

        const toSend = Buffer.from(s, "utf-8");
        req.write(toSend);
        req.end();
    });
}

const clientId = Math.random().toString().slice(2, 5);
