"""
Automated Verification Suite for OOP Interview Chatbot
Tests knowledge base, session flow, evaluator (offline & fallback), and Flask routes.
"""

import sys
from knowledge_base import QUESTIONS, TOPICS, filter_questions, get_question_by_id
from evaluator import evaluate_offline, evaluate_answer
from interview_session import InterviewSession
from app import app


def test_knowledge_base():
    print("[1] Testing Knowledge Base...")
    assert len(QUESTIONS) >= 8, f"Expected at least 8 questions, got {len(QUESTIONS)}"
    assert "core_pillars" in TOPICS
    assert "solid" in TOPICS

    for q in QUESTIONS:
        assert "id" in q and "title" in q and "question" in q
        assert "rubric" in q
        assert "expected_concepts" in q
        assert "model_answer" in q

    q1 = get_question_by_id("pillar_1")
    assert q1 is not None
    assert q1["title"] == "Encapsulation vs Abstraction"

    junior_qs = filter_questions(difficulty="Junior")
    assert len(junior_qs) > 0
    print("    -> Knowledge Base passed successfully!")


def test_evaluator():
    print("[2] Testing Evaluator...")
    q1 = get_question_by_id("pillar_1")
    
    # Test excellent answer
    sample_excellent = """Encapsulation is the bundling of data and methods inside a class, using access modifiers like private to enforce data hiding. Abstraction is hiding the internal complexity and exposing only essential interfaces, like a car's steering wheel hiding the engine implementation."""
    eval_res = evaluate_offline(q1, sample_excellent)
    assert eval_res["score"] >= 7.5, f"Expected score >= 7.5, got {eval_res['score']}"
    assert len(eval_res["strengths"]) > 0

    # Test empty / skip answer
    skip_res = evaluate_offline(q1, "idk pass")
    assert skip_res["score"] == 1.0

    print("    -> Evaluator passed successfully!")


def test_session_lifecycle():
    print("[3] Testing InterviewSession Lifecycle...")
    sess = InterviewSession(topic="core_pillars", difficulty="Junior", num_questions=2)
    assert len(sess.questions) <= 2
    
    q_curr = sess.current_question()
    assert q_curr["index"] == 1
    assert "model_answer" not in q_curr  # Should not leak model answer before grading

    # Test Hint
    hint_res = sess.get_hint()
    assert "hint" in hint_res

    # Submit answer
    sub_res = sess.submit_answer("Encapsulation bundles data and hides it with access modifiers. Abstraction exposes only the essential interfaces.")
    assert "evaluation" in sub_res
    assert sub_res["evaluation"]["score"] > 0

    # Submit second answer to complete
    sub_res2 = sess.submit_answer("Overloading is compile-time with same name different parameters. Overriding is runtime with same signature in subclass.")
    assert sess.is_completed is True

    # Check report
    report = sess.generate_report()
    assert "overall_score" in report
    assert "verdict" in report
    assert len(report["detailed_turns"]) == 2
    print(f"    -> Session Lifecycle passed! (Final Score: {report['overall_score']}/10, Verdict: {report['verdict']})")


def test_flask_routes():
    print("[4] Testing Flask API Routes...")
    client = app.test_client()

    # GET /api/config
    res = client.get("/api/config")
    assert res.status_code == 200
    data = res.get_json()
    assert "topics" in data
    assert data["total_questions_in_kb"] >= 8

    # POST /api/start
    start_res = client.post("/api/start", json={"topic": "all", "difficulty": "Mid-Level", "num_questions": 3})
    assert start_res.status_code == 200
    start_data = start_res.get_json()
    assert "session_id" in start_data
    sess_id = start_data["session_id"]

    # GET /api/current-question
    q_res = client.get(f"/api/current-question?session_id={sess_id}")
    assert q_res.status_code == 200
    assert q_res.get_json()["question"]["index"] == 1

    # POST /api/submit-answer
    ans_res = client.post("/api/submit-answer", json={
        "session_id": sess_id,
        "answer": "This is a solid technical answer demonstrating encapsulation and abstraction principles."
    })
    assert ans_res.status_code == 200
    assert "evaluation" in ans_res.get_json()

    print("    -> Flask API routes passed successfully!")


if __name__ == "__main__":
    print("\nStarting OOP Interview Bot Test Suite...\n")
    test_knowledge_base()
    test_evaluator()
    test_session_lifecycle()
    test_flask_routes()
    print("\n=======================================================")
    print(" ALL VERIFICATION TESTS PASSED SUCCESSFULLY! ")
    print("=======================================================\n")
