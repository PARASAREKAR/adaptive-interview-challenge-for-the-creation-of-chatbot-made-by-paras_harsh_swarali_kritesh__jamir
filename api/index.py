import os
import sys

# Ensure the project root directory is in the sys.path
root_dir = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
if root_dir not in sys.path:
    sys.path.insert(0, root_dir)

from app import app as flask_app
from werkzeug.middleware.proxy_fix import ProxyFix


class VercelWSGIMiddleware:
    """
    WSGI Middleware to normalize PATH_INFO on Vercel Serverless deployments.
    Vercel rewrites routes to /api/index, which causes Flask to receive PATH_INFO = '/api/index'
    instead of the real requested path (e.g. '/' or '/api/config').
    This middleware extracts the true requested path from Vercel's proxy headers or cleans
    the script prefix so Flask's route matching works seamlessly.
    """
    def __init__(self, wsgi_app):
        self.wsgi_app = wsgi_app

    def __call__(self, environ, start_response):
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
