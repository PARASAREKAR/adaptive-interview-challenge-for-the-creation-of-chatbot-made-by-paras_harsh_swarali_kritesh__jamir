"""
OOP Technical Interview Chatbot - Adaptive Interview Challenge Session Engine (v2)
Features:
  - Dynamic Difficulty Adaptation (Junior -> Mid-Level -> Senior -> Staff)
  - Elo-style Candidate Skill Rating (1000 - 2000+)
  - Real-time Interviewer Curveballs & In-Flight Constraints
  - Performance Streaks and Hiring Committee Calibrated Scorecards
"""

import time
import random
from typing import Dict, Any, List, Optional
from knowledge_base import QUESTIONS, TOPICS, DIFFICULTIES, get_question_by_id
from evaluator import evaluate_answer, get_ai_deep_analysis


TIER_LEVELS = {
    "Junior": 1,
    "Mid-Level": 2,
    "Senior / Lead": 3,
    "Staff / Principal": 4
}

LEVEL_TO_TIER = {
    1: "Junior",
    2: "Mid-Level",
    3: "Senior / Lead",
    4: "Staff / Principal"
}


class AdaptiveInterviewSession:
    def __init__(
        self,
        topic: str = "all",
        difficulty: str = "adaptive",
        num_questions: int = 5,
        api_key: Optional[str] = None,
        challenge_mode: str = "simulation"
    ):
        self.session_id = f"sim_{int(time.time())}_{random.randint(100, 999)}"
        self.topic = topic
        self.selected_difficulty = difficulty
        self.is_adaptive = (difficulty.lower() == "adaptive" or difficulty.lower() == "all")
        self.num_questions = num_questions
        self.api_key = api_key
        self.challenge_mode = challenge_mode

        self.start_time = time.time()
        self.end_time: Optional[float] = None

        # Adaptive State
        self.current_tier = "Mid-Level" if self.is_adaptive else difficulty
        self.current_level = TIER_LEVELS.get(self.current_tier, 2)
        self.skill_rating = 1250  # Baseline Elo
        self.peak_skill_rating = 1250
        self.streak = 0
        self.highest_tier_reached = self.current_tier

        # Trajectory history: tracks how difficulty & rating evolve
        self.trajectory: List[Dict[str, Any]] = [{
            "question_index": 0,
            "tier": self.current_tier,
            "level": self.current_level,
            "skill_rating": self.skill_rating,
            "event": "START",
            "reason": "Interview initiated at baseline difficulty."
        }]

        # Used question IDs to avoid repeats
        self.asked_question_ids: List[str] = []
        self.turns: List[Dict[str, Any]] = []
        self.hints_used: Dict[str, int] = {}
        self.curveballs_addressed: List[str] = []
        self.is_completed = False

        # Pick first question
        self.active_question = self._select_next_question()

    # ------------------------------------------------------------------
    # Adaptive Question Selection
    # ------------------------------------------------------------------
    def _select_next_question(self) -> Optional[Dict[str, Any]]:
        """Dynamically picks the next question based on current adaptive tier and unvisited topics."""
        # Candidate pool
        pool = [q for q in QUESTIONS if q["id"] not in self.asked_question_ids]

        if not pool:
            # All questions exhausted
            return None

        # Filter by topic if specific topic requested
        if self.topic != "all":
            topic_pool = [q for q in pool if q["topic"] == self.topic]
            if topic_pool:
                pool = topic_pool

        # Filter by target difficulty tier
        tier_pool = [q for q in pool if q["difficulty"].lower() == self.current_tier.lower()]

        if not tier_pool:
            # Fallback to nearest tier
            tier_pool = pool

        # Pick question with least visited topic among turns
        covered_topics = [t["topic"] for t in self.turns]
        uncovered = [q for q in tier_pool if q["topic"] not in covered_topics]

        selected = random.choice(uncovered) if uncovered else random.choice(tier_pool)
        self.asked_question_ids.append(selected["id"])
        return selected

    # ------------------------------------------------------------------
    # Current Question Representation
    # ------------------------------------------------------------------
    def current_question(self) -> Optional[Dict[str, Any]]:
        if not self.active_question or self.is_completed:
            return None

        q = self.active_question
        turn_num = len(self.turns) + 1

        return {
            "id": q["id"],
            "topic": q["topic"],
            "difficulty": q["difficulty"],
            "current_tier": self.current_tier,
            "level_number": self.current_level,
            "skill_rating": self.skill_rating,
            "streak": self.streak,
            "title": q["title"],
            "question": q["question"],
            "code_snippet": q.get("code_snippet"),
            "curveball": q.get("curveball"),
            "total_hints": len(q.get("hints", [])),
            "index": turn_num,
            "total_questions": self.num_questions,
            "progress_pct": round(((turn_num - 1) / self.num_questions) * 100),
            "is_adaptive": self.is_adaptive
        }

    # ------------------------------------------------------------------
    # Submit Answer & Adapt Difficulty
    # ------------------------------------------------------------------
    def submit_answer(self, answer: str) -> Dict[str, Any]:
        if self.is_completed or not self.active_question:
            return {"error": "Interview challenge is already completed."}

        current_q = self.active_question
        eval_result = evaluate_answer(current_q, answer)
        score = eval_result["score"]

        # Calculate Adaptive Level Shift & Rating Delta
        old_tier = self.current_tier
        old_level = self.current_level
        adaptation_event = "MAINTAIN"
        adaptation_msg = "Consistent answer. Maintaining current level."
        rating_delta = 0

        if self.is_adaptive:
            if score >= 8.0:
                # Strong performance -> Scale UP
                rating_delta = int(score * 13) + (self.streak * 10)
                self.streak += 1
                if self.current_level < 4:
                    self.current_level += 1
                    self.current_tier = LEVEL_TO_TIER[self.current_level]
                    adaptation_event = "LEVEL_UP"
                    adaptation_msg = f"🔥 Performance Surge ({score}/10)! Escalating challenge to {self.current_tier}."
                else:
                    adaptation_msg = f"🔥 Staff-Level Mastery maintained ({score}/10)!"
            elif score <= 5.0:
                # Struggling -> Scale DOWN to scaffold fundamentals
                rating_delta = -int((10.0 - score) * 8)
                self.streak = 0
                if self.current_level > 1:
                    self.current_level -= 1
                    self.current_tier = LEVEL_TO_TIER[self.current_level]
                    adaptation_event = "SCAFFOLD"
                    adaptation_msg = f"📉 Foundational Check: Adapting challenge to {self.current_tier} to verify core fundamentals."
                else:
                    adaptation_msg = f"Foundational level maintained ({score}/10)."
            else:
                # Moderate score -> Maintain level
                rating_delta = int(score * 5)
                adaptation_msg = f"Solid answer ({score}/10). Testing adjacent design patterns at {self.current_tier}."

            self.skill_rating = max(800, self.skill_rating + rating_delta)
            if self.skill_rating > self.peak_skill_rating:
                self.peak_skill_rating = self.skill_rating

            if TIER_LEVELS.get(self.current_tier, 1) > TIER_LEVELS.get(self.highest_tier_reached, 1):
                self.highest_tier_reached = self.current_tier
        else:
            # Fixed difficulty mode
            rating_delta = int(score * 8)
            self.skill_rating += rating_delta

        # Record trajectory
        turn_index = len(self.turns) + 1
        self.trajectory.append({
            "question_index": turn_index,
            "tier": self.current_tier,
            "level": self.current_level,
            "skill_rating": self.skill_rating,
            "rating_delta": rating_delta,
            "score": score,
            "event": adaptation_event,
            "reason": adaptation_msg
        })

        turn_record = {
            "question_id": current_q["id"],
            "question_title": current_q["title"],
            "question_text": current_q["question"],
            "topic": current_q["topic"],
            "difficulty": current_q["difficulty"],
            "tier_at_time": old_tier,
            "candidate_answer": answer,
            "evaluation": eval_result,
            "curveball": current_q.get("curveball"),
            "hints_used_count": self.hints_used.get(current_q["id"], 0),
            "model_answer": current_q.get("model_answer"),
            "adaptation_event": adaptation_event,
            "adaptation_msg": adaptation_msg,
            "rating_delta": rating_delta,
            "current_skill_rating": self.skill_rating,
            "ai_enhanced": False,
            "ai_analysis": None,
        }

        self.turns.append(turn_record)

        # Check if interview complete
        if len(self.turns) >= self.num_questions:
            self.is_completed = True
            self.end_time = time.time()
            next_q = None
        else:
            self.active_question = self._select_next_question()
            next_q = self.current_question()

        return {
            "evaluation": eval_result,
            "adaptation": {
                "event": adaptation_event,
                "message": adaptation_msg,
                "old_tier": old_tier,
                "new_tier": self.current_tier,
                "level_number": self.current_level,
                "skill_rating": self.skill_rating,
                "rating_delta": rating_delta,
                "streak": self.streak
            },
            "is_completed": self.is_completed,
            "next_question": next_q,
            "turn_index": len(self.turns) - 1
        }

    # ------------------------------------------------------------------
    # AI Enhancement (Manual "Consult Simulation Interviewer")
    # ------------------------------------------------------------------
    def get_ai_enhanced_eval(self, api_key: Optional[str] = None) -> Dict[str, Any]:
        if not self.turns:
            return {"error": "No answers submitted yet."}

        last_turn = self.turns[-1]
        effective_key = api_key or self.api_key
        if not effective_key:
            return {"error": "Please provide your Gemini API key in Settings to activate the Simulation Interviewer."}

        question_data = get_question_by_id(last_turn["question_id"])
        if not question_data:
            return {"error": "Question data not found."}

        ai_result = get_ai_deep_analysis(
            question=question_data,
            answer=last_turn["candidate_answer"],
            basic_eval=last_turn["evaluation"],
            api_key=effective_key,
            simulation_context={
                "current_tier": self.current_tier,
                "level_number": self.current_level,
                "skill_rating": self.skill_rating,
                "streak": self.streak,
                "turn_num": len(self.turns),
                "total_questions": self.num_questions
            }
        )

        if ai_result:
            last_turn["ai_enhanced"] = True
            last_turn["ai_analysis"] = ai_result
            return {"ai_analysis": ai_result}
        else:
            return {"error": "Simulation Interviewer analysis failed. Please verify your API key."}

    # ------------------------------------------------------------------
    # Hints & Solutions
    # ------------------------------------------------------------------
    def get_hint(self) -> Dict[str, Any]:
        if not self.active_question or self.is_completed:
            return {"error": "No active question."}

        q = self.active_question
        hints = q.get("hints", [])
        used = self.hints_used.get(q["id"], 0)

        if used < len(hints):
            hint_text = hints[used]
            self.hints_used[q["id"]] = used + 1
            return {
                "hint": hint_text,
                "hint_number": used + 1,
                "remaining_hints": len(hints) - (used + 1)
            }
        return {
            "hint": "No further hints available for this question. State your assumptions clearly and provide your best rationale!",
            "hint_number": used,
            "remaining_hints": 0
        }

    def get_model_answer(self) -> Dict[str, Any]:
        if not self.active_question or self.is_completed:
            return {"error": "No active question."}
        return {
            "question_id": self.active_question["id"],
            "title": self.active_question["title"],
            "model_answer": self.active_question.get("model_answer", "No model answer registered.")
        }

    # ------------------------------------------------------------------
    # Final Adaptive Performance Scorecard
    # ------------------------------------------------------------------
    def generate_report(self) -> Dict[str, Any]:
        if not self.turns:
            return {"error": "No completed questions yet."}

        scores = [t["evaluation"]["score"] for t in self.turns]
        avg_score = round(sum(scores) / len(scores), 1)

        # Calibrated Hiring Recommendation & Seniority Bracket
        if self.skill_rating >= 1550 or avg_score >= 8.8:
            calibrated_level = "Staff / Principal Software Engineer (L6 / E6)"
            verdict = "STRONG HIRE — Staff Level Architectural Mastery"
            badge_color = "#10b981"
            hiring_summary = "Demonstrated Staff-level command over OOP foundations, distributed design patterns, and hardware/memory trade-offs. Formulates clean, decoupled abstractions under dynamic constraints."
        elif self.skill_rating >= 1380 or avg_score >= 7.2:
            calibrated_level = "Senior Software Engineer (L5 / E5)"
            verdict = "HIRE — Senior Engineering Competency"
            badge_color = "#3b82f6"
            hiring_summary = "Solid, production-ready grasp of OOP pillars, SOLID design contracts, and behavioral patterns. Can comfortably architect scalable systems with minor guidance on edge-case concurrency."
        elif self.skill_rating >= 1150 or avg_score >= 5.5:
            calibrated_level = "Mid-Level Software Engineer (L4 / E4)"
            verdict = "LEAN HIRE / BORDERLINE — Core Foundations Sound"
            badge_color = "#f59e0b"
            hiring_summary = "Grasps fundamental class relationships and pattern mechanics, but needs more practice articulating architectural trade-offs, immutability, and memory lifecycle."
        else:
            calibrated_level = "Junior Software Engineer (L3 / E3)"
            verdict = "NEEDS DEVELOPMENT — Re-evaluate Fundamentals"
            badge_color = "#ef4444"
            hiring_summary = "Encountered significant gaps in core pillars, data hiding, and polymorphism. Recommended to review the model answers and refactoring examples."

        # Topic Breakdown
        topic_scores: Dict[str, List[float]] = {}
        for t in self.turns:
            top = t["topic"]
            topic_scores.setdefault(top, []).append(t["evaluation"]["score"])

        breakdown = {
            top: {
                "topic_name": TOPICS.get(top, top),
                "avg_score": round(sum(s) / len(s), 1),
                "count": len(s)
            }
            for top, s in topic_scores.items()
        }

        duration_sec = int((self.end_time or time.time()) - self.start_time)

        return {
            "session_id": self.session_id,
            "verdict": verdict,
            "calibrated_level": calibrated_level,
            "badge_color": badge_color,
            "overall_score": avg_score,
            "final_skill_rating": self.skill_rating,
            "peak_skill_rating": self.peak_skill_rating,
            "highest_tier_reached": self.highest_tier_reached,
            "is_adaptive": self.is_adaptive,
            "total_questions_answered": len(self.turns),
            "total_duration_formatted": f"{duration_sec // 60}m {duration_sec % 60}s",
            "ai_enhanced_count": sum(1 for t in self.turns if t.get("ai_enhanced")),
            "hiring_summary": hiring_summary,
            "trajectory": self.trajectory,
            "topic_breakdown": breakdown,
            "detailed_turns": self.turns
        }


# Maintain backwards compatibility
InterviewSession = AdaptiveInterviewSession
