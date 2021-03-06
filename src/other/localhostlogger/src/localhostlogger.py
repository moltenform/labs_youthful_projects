#!/usr/bin/env python3

# localhostlogger.py
# 2020 Ben Fisher, released under the MIT license.

# Some code based on server.py,
# https://gist.github.com/mdonkers/63e115cc0c79b4f6b8b3a6b797e485c7

PORT = 9123
LOGFILE = './localhostlog.log'
ENCODING = 'utf-8'

# provide a value in response if certain text appears in the post.
# can be useful for toggling behavior and not needing to compile your app.

from http.server import BaseHTTPRequestHandler, HTTPServer
import logging
import json
from datetime import datetime

class LocalhostLoggerServer(BaseHTTPRequestHandler):
    # Overriding parent: called when an options request arrives
    def do_OPTIONS(self):           
        self.send_response(200)
        self.send_header('Access-Control-Allow-Origin', '*')                
        self.send_header('Access-Control-Allow-Methods', 'GET, POST, OPTIONS')
        self.send_header("Access-Control-Allow-Headers", "X-Requested-With")
        self._complete_response()
        self.wfile.write(" ".encode('utf-8'))

    # Overriding parent: called when a get request arrives
    def do_GET(self):
        self.send_response(200)
        self._complete_response()
        self.wfile.write("Welcome to localhostlogger. To use, send a post to '/log'.".encode('utf-8'))

    # Overriding parent: called when a post request arrives
    def do_POST(self):
        content_length = int(self.headers['Content-Length'])
        post_data = self.rfile.read(content_length) # hangs if content_length is not given
        post_data = post_data.decode('utf-8')
        response = ''
        if self.path.endswith('asksetting'):
            self.send_response(200)
            response = self.onAskSetting(post_data)
        elif self.path.endswith('log'):
            self.send_response(200)
            response = self.onLog(post_data)
        else:
            self.send_response(404)
        self._complete_response()
        self.wfile.write(response.encode('utf-8'))

    # call this to indicate success
    def _complete_response(self):
        self.send_header('Content-type', 'text/plain; charset=utf-8')
        self.end_headers()

    # Overriding parent: finish sending headers
    def end_headers(self):
        self.send_header('Access-Control-Allow-Origin', '*')
        BaseHTTPRequestHandler.end_headers(self)

    # caller is asking for a setting
    def onAskSetting(self, post_data):
        with open(ASKSETTINGSFILE, encoding='utf-8') as f:
           allSettings = f.read()
        allSettings = json.loads(allSettings)
        val = ''
        parts = post_data.split('$$$')
        if len(parts) == 3:
            key = parts[1]
            val = allSettings.get(key, '')

        return '$$$' + val + '$$$'

    # caller is asking to log text
    def onLog(self, text):
        text = text.replace('\n', '\n                ')
        current_time = datetime.now().strftime("%H:%M:%S")
        logging.info(f"[{current_time}] {text}")
        return 'Logged.'

def run(port):
    logging.basicConfig(
        filename=LOGFILE,
        filemode='a',
        level=logging.INFO)
    server_address = ('', port)
    httpd = HTTPServer(server_address, LocalhostLoggerServer)
    current_time = datetime.now().strftime("%H:%M:%S")
    logging.info(f'[{current_time}] Starting httpd on port {port}...\n')
    try:
        httpd.serve_forever()
    except KeyboardInterrupt:
        pass
    httpd.server_close()
    logging.info('Stopping httpd...\n')

if __name__ == '__main__':
    run(port=PORT)


