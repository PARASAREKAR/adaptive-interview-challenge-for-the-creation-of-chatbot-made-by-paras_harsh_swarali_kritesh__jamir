import os
import sys
import urllib.parse

# Ensure the project root directory is in the sys.path
root_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
if root_dir not in sys.path:
    sys.path.insert(0, root_dir)

from app import app as flask_app
from werkzeug.middleware.proxy_fix import ProxyFix


class VercelWSGIMiddleware:
    """
    WSGI Middleware to normalize PATH_INFO and QUERY_STRING on Vercel Serverless deployments.
    Extracts the true client route from '__path' passed by vercel.json rewrite or proxy headers.
    """
    def __init__(self, wsgi_app):
        self.wsgi_app = wsgi_app

    def __call__(self, environ, start_response):
        qs = environ.get("QUERY_STRING", "")
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
                path_info = environ.get("PATH_INFO", "")

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
