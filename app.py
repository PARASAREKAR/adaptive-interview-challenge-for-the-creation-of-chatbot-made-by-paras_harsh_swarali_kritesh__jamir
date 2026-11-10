"""
OOP Technical Interview Chatbot - Adaptive Challenge Simulation Server (v2)
Features:
  - Adaptive Interview Simulation Engine
  - Dynamic difficulty progression & Elo rating tracking
  - In-flight interviewer curveballs & staff-level debriefs
"""

import os
import sys
import webbrowser
import threading
import socket
from flask import Flask, request, jsonify, render_template, send_from_directory
from knowledge_base import TOPICS, DIFFICULTIES, QUESTIONS
from interview_session import AdaptiveInterviewSession
from evaluator import generate_ai_curveball

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
app = Flask(
    __name__,
    static_folder=os.path.join(BASE_DIR, "static"),
    template_folder=os.path.join(BASE_DIR, "templates")
)

# In-memory session store
sessions = {}
active_session_id = None


def find_available_port(start_port=5000, max_attempts=20):
    for port in range(start_port, start_port + max_attempts):
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
            if s.connect_ex(("localhost", port)) != 0:
                return port
    return start_port


@app.route("/")
@app.route("/api/index")
@app.route("/api/index.py")
@app.route("/api")
@app.route("/api/")
def index():
    return render_template("index.html")


@app.route("/static/<path:filename>")
def serve_static(filename):
    return send_from_directory(os.path.join(BASE_DIR, "static"), filename)


@app.route("/favicon.ico")
def favicon():
    return ("", 204)


@app.route("/api/config", methods=["GET"])
def get_config():
    return jsonify({
        "topics": TOPICS,
        "difficulties": DIFFICULTIES,
        "total_questions_in_kb": len(QUESTIONS),
        "has_env_gemini_key": bool(os.environ.get("GEMINI_API_KEY")),
    })


@app.route("/api/start", methods=["POST"])
def start_interview():
    global active_session_id
    data = request.json or {}
    topic = data.get("topic", "all")
    difficulty = data.get("difficulty", "adaptive")
    num_questions = int(data.get("num_questions", 5))
    challenge_mode = data.get("challenge_mode", "simulation")
    api_key = data.get("api_key") or os.environ.get("GEMINI_API_KEY")

    session = AdaptiveInterviewSession(
        topic=topic,
        difficulty=difficulty,
        num_questions=num_questions,
        api_key=api_key,
        challenge_mode=challenge_mode
    )
    sessions[session.session_id] = session
    active_session_id = session.session_id

    return jsonify({
        "status": "success",
        "session_id": session.session_id,
        "first_question": session.current_question(),
        "total_selected": session.num_questions,
        "is_adaptive": session.is_adaptive,
        "initial_tier": session.current_tier,
        "initial_rating": session.skill_rating
    })


@app.route("/api/current-question", methods=["GET"])
def get_current_question():
    session_id = request.args.get("session_id") or active_session_id
    session = sessions.get(session_id)
    if not session:
        return jsonify({"error": "No active session found."}), 404

    return jsonify({
        "question": session.current_question(),
        "is_completed": session.is_completed,
        "skill_rating": session.skill_rating,
        "current_tier": session.current_tier
    })


@app.route("/api/submit-answer", methods=["POST"])
def submit_answer():
    data = request.json or {}
    session_id = data.get("session_id") or active_session_id
    answer = data.get("answer", "")

    session = sessions.get(session_id)
    if not session:
        return jsonify({"error": "No active session found."}), 404

    result = session.submit_answer(answer)
    return jsonify(result)


@app.route("/api/ai-enhance", methods=["POST"])
def ai_enhance():
    """Manual on-demand simulation evaluation triggered by candidate."""
    data = request.json or {}
    session_id = data.get("session_id") or active_session_id
    api_key = data.get("api_key") or os.environ.get("GEMINI_API_KEY")

    session = sessions.get(session_id)
    if not session:
        return jsonify({"error": "No active session found."}), 404

    result = session.get_ai_enhanced_eval(api_key=api_key)
    return jsonify(result)


@app.route("/api/curveball", methods=["POST"])
def get_dynamic_curveball():
    """Dynamically throws an in-flight production constraint based on candidate's answer."""
    data = request.json or {}
    session_id = data.get("session_id") or active_session_id
    api_key = data.get("api_key") or os.environ.get("GEMINI_API_KEY")

    session = sessions.get(session_id)
    if not session or not session.turns:
        return jsonify({"error": "No active turns to throw curveball on."}), 400

    last_turn = session.turns[-1]
    from knowledge_base import get_question_by_id
    q_data = get_question_by_id(last_turn["question_id"])

    if not api_key:
        # Fallback to predefined curveball if no Gemini key
        return jsonify({
            "curveball": {
                "title": "Interviewer In-Flight Constraint",
                "challenge": q_data.get("curveball", "How does your architecture handle sudden 100x traffic spikes with zero data loss?")
            }
        })

    curveball = generate_ai_curveball(q_data, last_turn["candidate_answer"], api_key)
    if curveball:
        return jsonify({"curveball": curveball})
    return jsonify({
        "curveball": {
            "title": "Interviewer Scale Constraint",
            "challenge": q_data.get("curveball", "How would you make this design lock-free under extreme concurrent write pressure?")
        }
    })


@app.route("/api/hint", methods=["GET"])
def get_hint():
    session_id = request.args.get("session_id") or active_session_id
    session = sessions.get(session_id)
    if not session:
        return jsonify({"error": "No active session found."}), 404

    return jsonify(session.get_hint())


@app.route("/api/model-answer", methods=["GET"])
def get_model_answer():
    session_id = request.args.get("session_id") or active_session_id
    session = sessions.get(session_id)
    if not session:
        return jsonify({"error": "No active session found."}), 404

    return jsonify(session.get_model_answer())


@app.route("/api/report", methods=["GET"])
def get_report():
    session_id = request.args.get("session_id") or active_session_id
    session = sessions.get(session_id)
    if not session:
        return jsonify({"error": "No active session found."}), 404

    return jsonify(session.generate_report())


@app.route("/api/health", methods=["GET"])
def health():
    return jsonify({"status": "ok", "active_sessions": len(sessions)})


def open_browser(port):
    webbrowser.open(f"http://localhost:{port}")


if __name__ == "__main__":
    port = find_available_port(5000)
    should_open = "--no-browser" not in sys.argv

    print("\n" + "=" * 66)
    print("  Adaptive Interview Challenge - AI Mock Simulation Bot (v2)")
    print(f"  Access UI at: http://localhost:{port}")
    print("=" * 66 + "\n")

    if should_open:
        t = threading.Timer(1.0, lambda: open_browser(port))
        t.daemon = True
        t.start()

    app.run(host="0.0.0.0", port=port, debug=False)
