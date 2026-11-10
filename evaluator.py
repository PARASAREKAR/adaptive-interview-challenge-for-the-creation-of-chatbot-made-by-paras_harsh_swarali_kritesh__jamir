"""
OOP Technical Interview Chatbot - Answer Evaluator (v2 - Adaptive Challenge Edition)
Supports:
  1. Smart offline semantic rubric evaluation (instant, zero-latency)
  2. Gemini AI Simulation Bot evaluation (manual on-demand trigger)
  3. Real-time dynamic interviewer curveball generator
"""

import os
import re
import json
from typing import Dict, Any, Optional, List

try:
    from google import genai
    GENAI_AVAILABLE = True
except ImportError:
    GENAI_AVAILABLE = False


# ---------------------------------------------------------------------------
# Synonym map for smarter offline concept matching
# ---------------------------------------------------------------------------
CONCEPT_SYNONYMS = {
    "encapsulation": ["data hiding", "bundling", "information hiding", "access control", "private fields"],
    "abstraction": ["hiding complexity", "simplification", "essential features", "interface"],
    "polymorphism": ["many forms", "dynamic dispatch", "method dispatch", "overriding", "overloading"],
    "inheritance": ["subclass", "extends", "derive", "parent class", "base class", "is-a"],
    "composition": ["has-a", "delegation", "composed", "wrapper", "contains"],
    "interface": ["contract", "api surface", "abstract type", "protocol"],
    "vtable": ["virtual table", "virtual method table", "function pointer table", "dispatch table"],
    "vptr": ["virtual pointer", "vtable pointer"],
    "singleton": ["single instance", "one instance", "global instance"],
    "factory": ["creator", "instantiation", "object creation"],
    "observer": ["listener", "subscriber", "event handler", "callback"],
    "strategy": ["algorithm swap", "interchangeable", "pluggable"],
    "dependency inversion": ["depend on abstractions", "dip", "inversion of dependencies"],
    "dependency injection": ["inject", "constructor injection", "di container"],
    "liskov": ["substitutability", "behavioral subtyping", "lsp"],
    "open closed": ["ocp", "open for extension", "closed for modification"],
    "single responsibility": ["srp", "one reason to change", "one actor"],
    "interface segregation": ["isp", "fat interface", "thin interface"],
    "immutability": ["immutable", "unchangeable", "frozen", "read only", "const"],
    "thread safety": ["thread safe", "concurrent", "synchronization", "lock free", "mutex"],
    "fragile base class": ["base class problem", "brittle base", "inheritance coupling"],
    "diamond problem": ["diamond inheritance", "multiple inheritance ambiguity"],
    "mro": ["method resolution order", "c3 linearization", "c3 algorithm"],
    "value object": ["value type", "structural equality"],
    "entity": ["identity", "unique id", "persistent identity"],
    "decorator pattern": ["wrapper pattern", "decorating", "class explosion"],
    "state pattern": ["finite state machine", "fsm", "state transition"],
    "doubly linked list": ["dll", "linked list", "nodes", "mru", "lru"],
    "hash map": ["dictionary", "hash table", "hashmap", "o(1) lookup"],
    "stack": ["call stack", "lifo", "stack memory", "frames"],
    "heap": ["dynamic memory", "heap memory", "garbage collector", "malloc"],
}


def _fuzzy_concept_match(concept: str, text: str) -> bool:
    """Check if a concept appears in text using exact match, synonyms, and fuzzy stems."""
    text_lower = text.lower()
    concept_lower = concept.lower()

    if re.search(r"\b" + re.escape(concept_lower) + r"\b", text_lower):
        return True

    tokens = concept_lower.split()
    if len(tokens) > 1 and all(t in text_lower for t in tokens):
        return True

    for base, syns in CONCEPT_SYNONYMS.items():
        if concept_lower == base or concept_lower in syns:
            if re.search(r"\b" + re.escape(base) + r"\b", text_lower):
                return True
            for syn in syns:
                if syn in text_lower:
                    return True

    stem = concept_lower[:6] if len(concept_lower) > 6 else concept_lower
    if len(stem) >= 4 and stem in text_lower:
        return True

    return False


# ---------------------------------------------------------------------------
# Offline Evaluator (Instant, zero latency)
# ---------------------------------------------------------------------------
def evaluate_offline(question: Dict[str, Any], answer: str) -> Dict[str, Any]:
    """Smart deterministic NLP & rubric-based evaluator."""
    clean_text = answer.strip().lower()
    word_count = len(clean_text.split())

    skip_phrases = {"idk", "i don't know", "dont know", "pass", "skip", "no idea", "not sure", "i pass"}
    if word_count < 4 or clean_text.strip("., ") in skip_phrases:
        return {
            "score": 1.0,
            "rating": "No Answer Provided",
            "strengths": ["Honesty in passing to review model rationale."],
            "improvements": ["Even an imperfect mental model demonstrates how you approach unfamiliar technical problems."],
            "interviewer_critique": "In a Staff-level technical simulation, never leave a prompt blank. State your working assumptions, sketch an interface, or break down the problem into smaller components.",
            "follow_up_question": question.get("follow_up", "Would you like to review the model solution?"),
            "engine": "Offline Semantic Engine"
        }

    expected_concepts = question.get("expected_concepts", [])
    matched_concepts = []
    missing_concepts = []

    for concept in expected_concepts:
        if _fuzzy_concept_match(concept, answer):
            matched_concepts.append(concept)
        else:
            missing_concepts.append(concept)

    matched_count = len(matched_concepts)
    total_expected = max(len(expected_concepts), 1)
    concept_ratio = matched_count / total_expected

    if matched_count >= 6 or concept_ratio >= 0.50:
        concept_score = 5.0 + min(concept_ratio * 1.5, 1.5)
    elif matched_count >= 4 or concept_ratio >= 0.35:
        concept_score = 3.8 + (concept_ratio * 2.0)
    elif matched_count >= 2 or concept_ratio >= 0.20:
        concept_score = 2.2 + (concept_ratio * 2.5)
    elif matched_count >= 1:
        concept_score = 1.5 + (concept_ratio * 2.0)
    else:
        concept_score = 0.5

    depth_score = min(word_count / 55.0, 1.0) * 1.5
    has_code = any(k in answer for k in ["```", "class ", "def ", "function ", "public ", "private ", "->", "=>", "interface "])
    has_examples = any(k in clean_text for k in [
        "for example", "e.g.", "such as", "instance", "consider", "case",
        "like a", "imagine", "real-world", "scenario", "suppose"
    ])
    has_tradeoffs = any(k in clean_text for k in [
        "trade-off", "tradeoff", "downside", "drawback", "advantage",
        "however", "on the other hand", "but", "caveat", "limitation", "cost", "overhead"
    ])

    raw_score = 2.5 + concept_score + depth_score
    if has_code:
        raw_score += 0.8
    if has_examples:
        raw_score += 0.6
    if has_tradeoffs:
        raw_score += 0.4

    score = round(min(max(raw_score, 1.0), 10.0), 1)

    if score >= 8.5:
        rating = "Outstanding (Strong Hire)"
    elif score >= 7.0:
        rating = "Solid Competency (Hire)"
    elif score >= 5.0:
        rating = "Moderate (Borderline)"
    else:
        rating = "Needs Development"

    strengths = []
    if matched_concepts:
        highlighted = ", ".join(f"'{c}'" for c in matched_concepts[:4])
        strengths.append(f"Command of essential concepts: {highlighted}.")
    if has_examples:
        strengths.append("Anchored explanation with clear practical scenarios.")
    if has_code:
        strengths.append("Structured concrete code/interfaces demonstrating the design.")
    if has_tradeoffs:
        strengths.append("Analyzed practical engineering trade-offs and runtime costs.")
    if word_count > 80:
        strengths.append("Exhibited thorough, well-articulated engineering communication.")
    if not strengths:
        strengths.append("Initiated a valid line of architectural reasoning.")

    improvements = []
    if missing_concepts:
        unmentioned = ", ".join(f"'{c}'" for c in missing_concepts[:3])
        improvements.append(f"Consider exploring: {unmentioned}.")
    if not has_examples and not has_code:
        improvements.append("Strengthen your answer with a concrete code snippet or real-world component design.")
    if word_count < 35:
        improvements.append("Expand on trade-offs. Staff-level interviews require discussing scale, concurrency, or memory.")
    if not has_tradeoffs:
        improvements.append("Highlight failure modes: what are the limitations or performance costs of this approach?")
    if not improvements:
        improvements.append("Explore hardware-level memory boundaries and thread safety for Staff-level depth.")

    title = question.get("title", "this topic")
    if score >= 8.5:
        critique = f"Impressive response on {title}. You articulated the core mechanics clearly, avoided common pitfalls, and structured your design with high technical rigor."
    elif score >= 7.0:
        critique = f"Solid answer on {title}. You demonstrated production-ready understanding. Deepening on concurrency or hardware memory bounds will bring this to Staff caliber."
    elif score >= 5.0:
        critique = f"You touched on valid points regarding {title}, but the answer needs stronger architectural boundaries. Ensure you clearly state the design invariants."
    else:
        critique = f"This response missed several core tenets of {title}. Review the model solution and hints to reinforce the foundational mechanics."

    return {
        "score": score,
        "rating": rating,
        "strengths": strengths,
        "improvements": improvements,
        "interviewer_critique": critique,
        "follow_up_question": question.get("follow_up", "How would you test this design in production?"),
        "curveball": question.get("curveball"),
        "engine": "Offline Semantic Engine"
    }


# ---------------------------------------------------------------------------
# Deep AI Simulation Bot Analysis (Manual Trigger - "Consult Simulation Interviewer")
# ---------------------------------------------------------------------------
def get_ai_deep_analysis(
    question: Dict[str, Any],
    answer: str,
    basic_eval: Dict[str, Any],
    api_key: str,
    simulation_context: Optional[Dict[str, Any]] = None
) -> Optional[Dict[str, Any]]:
    """
    Produces a realistic FAANG Staff-Lead Mock Interviewer evaluation.
    Simulates Alex Rivera acting as an interactive interview simulation partner.
    """
    if not GENAI_AVAILABLE or not api_key:
        return None

    try:
        client = genai.Client(api_key=api_key)
        context = simulation_context or {}
        tier = context.get("current_tier", "Senior")
        skill = context.get("skill_rating", 1250)
        turn_num = context.get("turn_num", 1)
        total_q = context.get("total_questions", 5)

        system_prompt = (
            "You are Alex Rivera, a Staff Software Engineer at a top-tier tech company (FAANG/Tier-1) "
            "acting as the lead interviewer in a high-fidelity OOP Technical Mock Interview Simulation.\n\n"
            "SIMULATION PERSONA & RULES:\n"
            f"- Current Simulation Bracket: {tier} Level (Candidate Skill Rating: {skill})\n"
            f"- Interview Progress: Question {turn_num} of {total_q}\n"
            "- Be direct, constructive, and technically discerning — exactly like a Staff Lead debriefing a candidate.\n"
            "- Directly reference the candidate's exact words, code, and design decisions.\n"
            "- Formulate an official FAANG Hiring Recommendation: Strong Hire, Hire, Lean Hire, or No Hire.\n"
            "- Formulate a dynamic Curveball Challenge that pushes their design to the next complexity level.\n\n"
            "You MUST output ONLY valid JSON matching this schema:\n"
            "{\n"
            '  "ai_score": float (1.0 to 10.0),\n'
            '  "hiring_verdict": string ("Strong Hire" | "Hire" | "Lean Hire" | "No Hire"),\n'
            '  "ai_critique": string (3-5 sentences of conversational, high-caliber interview critique),\n'
            '  "key_architectural_insight": string (one powerful sentence on the core engineering takeaway),\n'
            '  "staff_nuances": [string array of 2-3 specific senior/staff-level subtleties they should know],\n'
            '  "curveball_challenge": string (a challenging real-world constraint or scale twist),\n'
            '  "probing_follow_up": string (a natural follow-up question to test their depth)\n'
            "}"
        )

        prompt = f"""**Candidate Interview Answer to Review:**
Question: {question.get('title')} [{question.get('difficulty')}]
Full Prompt: {question.get('question')}
Planned Curveball: {question.get('curveball', 'None')}

**Candidate's Actual Response:**
\"\"\"{answer}\"\"\"

**Automated Rubric Score:** {basic_eval.get('score')}/10 [{basic_eval.get('rating')}]
Key Strengths noted: {'; '.join(basic_eval.get('strengths', []))}
Missing concepts: {'; '.join(basic_eval.get('improvements', []))}

Expected Concepts: {', '.join(question.get('expected_concepts', []))}

Model Reference:
{question.get('model_answer', 'N/A')}

Provide your Staff Interviewer debrief JSON."""

        response = client.models.generate_content(
            model="gemini-2.0-flash",
            contents=prompt,
            config={"system_instruction": system_prompt, "temperature": 0.35}
        )

        output_text = response.text or ""
        clean_json = re.sub(r"^```(?:json)?\s*", "", output_text.strip(), flags=re.MULTILINE)
        clean_json = re.sub(r"\s*```$", "", clean_json.strip(), flags=re.MULTILINE)

        parsed = json.loads(clean_json)
        parsed["engine"] = "Alex Rivera (Staff Simulation AI)"
        return parsed
    except Exception as e:
        print(f"[Evaluator] Gemini Simulation error: {e}")
        return None


# ---------------------------------------------------------------------------
# Dynamic AI Curveball Generator (Candidate clicks "Throw Curveball")
# ---------------------------------------------------------------------------
def generate_ai_curveball(question: Dict[str, Any], answer: str, api_key: str) -> Optional[Dict[str, str]]:
    """Generates a dynamic real-time curveball twist based on the candidate's exact answer."""
    if not GENAI_AVAILABLE or not api_key:
        return None

    try:
        client = genai.Client(api_key=api_key)
        prompt = f"""You are Alex Rivera, Staff Technical Interviewer.
The candidate just proposed this solution to: "{question.get('title')}":

\"\"\"{answer}\"\"\"

Generate a realistic, unexpected engineering constraint ("Interviewer Curveball") that tests their design under production reality (e.g., massive concurrency, network partitions, zero-downtime schema evolution, memory constraints).

Output ONLY JSON:
{{
  "title": "Curveball Constraint Title",
  "challenge": "2-3 sentences presenting the unexpected production constraint."
}}"""

        response = client.models.generate_content(
            model="gemini-2.0-flash",
            contents=prompt,
            config={"temperature": 0.5}
        )

        output_text = response.text or ""
        clean_json = re.sub(r"^```(?:json)?\s*", "", output_text.strip(), flags=re.MULTILINE)
        clean_json = re.sub(r"\s*```$", "", clean_json.strip(), flags=re.MULTILINE)
        return json.loads(clean_json)
    except Exception as e:
        print(f"[Evaluator] Curveball generator error: {e}")
        return None


def evaluate_answer(question: Dict[str, Any], answer: str, api_key: Optional[str] = None) -> Dict[str, Any]:
    return evaluate_offline(question, answer)
