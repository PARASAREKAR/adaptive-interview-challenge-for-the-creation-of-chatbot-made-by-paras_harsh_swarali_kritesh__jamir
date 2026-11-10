import os
import sys
import urllib.parse
import json

# Ensure the project root directory is in the sys.path
root_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
if root_dir not in sys.path:
    sys.path.insert(0, root_dir)

from app import app as flask_app
from werkzeug.middleware.proxy_fix import ProxyFix


class VercelWSGIMiddleware:
    def __init__(self, wsgi_app):
        self.wsgi_app = wsgi_app

    def __call__(self, environ, start_response):
        qs = environ.get("QUERY_STRING", "")
        path_info = environ.get("PATH_INFO", "")

        # Immediate debug inspector
        if "debug=1" in qs or "debug" in path_info:
            headers = [("Content-Type", "application/json")]
            start_response("200 OK", headers)
            debug_data = {
                "PATH_INFO": environ.get("PATH_INFO"),
                "SCRIPT_NAME": environ.get("SCRIPT_NAME"),
                "QUERY_STRING": environ.get("QUERY_STRING"),
                "RAW_URI": environ.get("RAW_URI"),
                "REQUEST_URI": environ.get("REQUEST_URI"),
                "HEADERS": {k: v for k, v in environ.items() if k.startswith("HTTP_")},
            }
            return [json.dumps(debug_data, indent=2).encode("utf-8")]

        if "__path=" in qs:
            parsed = urllib.parse.parse_qs(qs, keep_blank_values=True)
            path = parsed.get("__path", [""])[0]
            remaining = {k: v for k, v in parsed.items() if k != "__path"}
            environ["QUERY_STRING"] = urllib.parse.urlencode(remaining, doseq=True)
            if not path or path == "/":
                environ["PATH_INFO"] = "/"
            else:
                environ["PATH_INFO"] = "/" + path.lstrip("/")
        else:
            real_uri = (
                environ.get("HTTP_X_FORWARDED_URI")
                or environ.get("HTTP_X_VERCEL_FORWARDED_FOR_PATH")
                or environ.get("HTTP_X_MATCHED_PATH")
                or environ.get("HTTP_X_NOW_ROUTE_MATCHES")
            )
            if real_uri:
                clean_path = real_uri.split("?")[0]
                if clean_path:
                    environ["PATH_INFO"] = clean_path
            else:
                script_name = environ.get("SCRIPT_NAME", "")
                if script_name and path_info.startswith(script_name):
                    environ["PATH_INFO"] = path_info[len(script_name):] or "/"
                    environ["SCRIPT_NAME"] = ""
                elif path_info in ("/api/index", "/api/index.py", "/api", "/api/"):
                    environ["PATH_INFO"] = "/"
                elif path_info.startswith("/api/index/"):
                    environ["PATH_INFO"] = path_info[len("/api/index"):] or "/"
                elif path_info.startswith("/api/index.py/"):
                    environ["PATH_INFO"] = path_info[len("/api/index.py"):] or "/"

        return self.wsgi_app(environ, start_response)


# Wrap with ProxyFix (for https/host forwarding) and VercelWSGIMiddleware
proxied_app = ProxyFix(flask_app.wsgi_app, x_for=1, x_proto=1, x_host=1, x_prefix=1)
flask_app.wsgi_app = VercelWSGIMiddleware(proxied_app)

app = flask_app
