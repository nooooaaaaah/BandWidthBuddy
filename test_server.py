import http.server
import socketserver
import socket

PORT = 8000

class Handler(http.server.SimpleHTTPRequestHandler):
    def _set_headers(self, content_type="text/html"):
        self.send_response(200)
        self.send_header("Content-type", content_type)
        self.end_headers()

    def do_GET(self):
        self._set_headers()
        path = self.path.strip("/")  # Remove leading slashes for correct path handling
        if path.startswith("bytes/"):
            try:
                size = int(path.split("/")[1])
                self.wfile.write(b"a" * size)
            except ValueError:
                self.wfile.write(b"Invalid size")
        else:
            self.wfile.write(b"Hello, world!")

    def do_POST(self):
        content_length = int(self.headers['Content-Length'])  # Get the size of data
        post_data = self.rfile.read(content_length)  # Read the data
        self._set_headers()
        self.wfile.write(b"Received POST data")

with socketserver.TCPServer(("", PORT), Handler) as httpd:
    hostname = socket.gethostname()
    local_ip = socket.gethostbyname(hostname)
    print("Server's IP:", local_ip)
    print("serving at port", PORT)
    httpd.serve_forever()
