"""
OOP Technical Interview Chatbot - Terminal / CLI Application
Run full interactive interviews directly in your shell!
"""

import os
import sys
import time
from knowledge_base import TOPICS, DIFFICULTIES, QUESTIONS
from interview_session import InterviewSession

# ANSI Colors
CYAN = "\033[96m"
GREEN = "\033[92m"
YELLOW = "\033[93m"
RED = "\033[91m"
BOLD = "\033[1m"
DIM = "\033[2m"
RESET = "\033[0m"


def print_banner():
    print(f"""{CYAN}{BOLD}
======================================================================
     OOP TECHNICAL INTERVIEW CHATBOT  |  Alex Rivera (Staff Lead)     
======================================================================{RESET}
{DIM}Sharpen your Object-Oriented Design, SOLID principles, & patterns.{RESET}
""")


def select_options():
    print(f"{BOLD}Select Interview Topic:{RESET}")
    topic_keys = list(TOPICS.keys())
    for i, key in enumerate(topic_keys, 1):
        print(f"  {CYAN}[{i}]{RESET} {TOPICS[key]}")
    
    choice = input(f"\nEnter choice [1-{len(topic_keys)}] (default: 1): ").strip()
    try:
        topic_idx = int(choice) - 1 if choice else 0
        topic = topic_keys[topic_idx] if 0 <= topic_idx < len(topic_keys) else "all"
    except ValueError:
        topic = "all"

    print(f"\n{BOLD}Select Difficulty Level:{RESET}")
    print(f"  {CYAN}[1]{RESET} Junior Software Engineer")
    print(f"  {CYAN}[2]{RESET} Mid-Level Software Engineer (Default)")
    print(f"  {CYAN}[3]{RESET} Senior / Staff Lead")
    print(f"  {CYAN}[4]{RESET} Mixed / Adaptive")

    diff_choice = input(f"\nEnter choice [1-4] (default: 2): ").strip()
    diff_map = {"1": "Junior", "2": "Mid-Level", "3": "Senior / Lead", "4": "all"}
    difficulty = diff_map.get(diff_choice, "Mid-Level")

    num_q = input(f"\nHow many questions would you like? [3, 5, 8] (default: 5): ").strip()
    num_questions = int(num_q) if num_q in ["3", "5", "8"] else 5

    api_key = os.environ.get("GEMINI_API_KEY")
    if not api_key:
        custom_key = input("\nEnter Gemini API Key (optional - press Enter for Offline Engine): ").strip()
        if custom_key:
            api_key = custom_key

    return topic, difficulty, num_questions, api_key


def run_cli_interview():
    print_banner()
    topic, difficulty, num_questions, api_key = select_options()

    session = InterviewSession(
        topic=topic,
        difficulty=difficulty,
        num_questions=num_questions,
        api_key=api_key
    )

    print(f"\n{GREEN}{BOLD}Session Started!{RESET} Total Questions: {len(session.questions)}")
    print(f"{DIM}Commands during question: ':hint' for hint, ':skip' to pass, ':quit' to exit.{RESET}\n")
    print("-" * 70)

    while not session.is_completed:
        q = session.current_question()
        if not q:
            break

        print(f"\n{CYAN}{BOLD}[Question {q['index']} of {q['total_questions']}]{RESET} {BOLD}{q['title']}{RESET}")
        print(f"{DIM}Topic: {q['topic']} | Level: {q['difficulty']}{RESET}\n")
        print(f"{q['question']}")

        if q.get("code_snippet"):
            print(f"\n{DIM}--- Code Snippet ---{RESET}")
            print(q["code_snippet"])
            print(f"{DIM}--------------------{RESET}")

        print(f"\n{BOLD}Your Answer (type your response, or type ':hint' / ':skip'):{RESET}")
        
        # Collect multiline answer until empty line or single line
        lines = []
        while True:
            line = input("> ")
            if line.strip() == ":quit":
                print(f"\n{YELLOW}Interview terminated early.{RESET}")
                return
            if line.strip() == ":hint":
                hint_data = session.get_hint()
                print(f"\n{YELLOW}💡 Hint #{hint_data.get('hint_number')}: {hint_data.get('hint')}{RESET}\n")
                continue
            if line.strip() == ":skip":
                lines = ["I pass on this question to review the model answer."]
                break

            lines.append(line)
            # If line is empty and we already have content, submit
            if line == "" and len(lines) > 1:
                break
            # If user enters single line and doesn't want multiline, ask or submit
            if len(lines) == 1 and not lines[0].endswith("\\"):
                # single line entered
                break

        full_answer = "\n".join(lines).strip()
        if not full_answer:
            full_answer = "No response provided."

        print(f"\n{DIM}Alex is reviewing your answer...{RESET}")
        result = session.submit_answer(full_answer, api_key=api_key)

        eval_res = result["evaluation"]
        score = eval_res["score"]
        score_color = GREEN if score >= 8.0 else YELLOW if score >= 5.5 else RED

        print(f"\n{score_color}{BOLD}Score: {score}/10  |  {eval_res['rating']}{RESET}")
        print(f"\n{DIM}\"{eval_res['interviewer_critique']}\"{RESET}")

        if eval_res.get("strengths"):
            print(f"\n{GREEN}Strengths:{RESET}")
            for s in eval_res["strengths"]:
                print(f"  + {s}")

        if eval_res.get("improvements"):
            print(f"\n{YELLOW}Areas to improve:{RESET}")
            for imp in eval_res["improvements"]:
                print(f"  - {imp}")

        if eval_res.get("follow_up_question"):
            print(f"\n{CYAN}Interviewer Follow-up:{RESET} {eval_res['follow_up_question']}")

        print(f"\n{DIM}(Evaluated by: {eval_res.get('engine', 'Expert Rubric')}){RESET}")
        print("-" * 70)
        time.sleep(1)

    # FINAL REPORT
    print(f"\n{GREEN}{BOLD}======================================================================{RESET}")
    print(f"{GREEN}{BOLD}                     INTERVIEW COMPLETED!                             {RESET}")
    print(f"{GREEN}{BOLD}======================================================================{RESET}\n")

    report = session.generate_report()
    print(f"{BOLD}Candidate Verdict:{RESET} {report['verdict']}")
    print(f"{BOLD}Overall Score:{RESET}    {report['overall_score']} / 10")
    print(f"{BOLD}Duration:{RESET}         {report['total_duration_formatted']}")
    print(f"\n{report['summary']}\n")

    print(f"{BOLD}Topic Breakdown:{RESET}")
    for top_key, top_data in report["topic_breakdown"].items():
        print(f"  * {top_data['topic_name']:<45} {top_data['avg_score']:>4}/10")

    print(f"\n{CYAN}Thank you for interviewing! Keep refining your OOP design skills.{RESET}\n")


if __name__ == "__main__":
    try:
        run_cli_interview()
    except (KeyboardInterrupt, EOFError):
        print(f"\n{YELLOW}Interrupted. Exiting cleanly...{RESET}")
    finally:
        os._exit(0)
