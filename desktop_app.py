"""
OOP Technical Interview Chatbot - Desktop GUI Application (Tkinter v2 - Adaptive Challenge Edition)
Native desktop application with Adaptive Difficulty Simulation, Elo Skill Rating, and Real-Time Interviewer Curveballs.
"""

import os
import sys
import threading
import tkinter as tk
from tkinter import ttk, messagebox, scrolledtext
from knowledge_base import TOPICS, DIFFICULTIES, QUESTIONS
from interview_session import AdaptiveInterviewSession


class OOPInterviewDesktopApp:
    def __init__(self, root):
        self.root = root
        self.root.title("Alex Rivera | Adaptive Interview Challenge Simulation")
        self.root.geometry("1060x780")
        self.root.minsize(860, 640)

        # Theme Colors
        self.bg_dark = "#07090e"
        self.bg_card = "#0e131f"
        self.bg_input = "#05070b"
        self.text_light = "#f8fafc"
        self.text_dim = "#94a3b8"
        self.accent_indigo = "#6366f1"
        self.accent_purple = "#8b5cf6"
        self.accent_cyan = "#06b6d4"
        self.accent_green = "#10b981"
        self.accent_amber = "#f59e0b"

        self.root.configure(bg=self.bg_dark)

        self.session = None
        self.api_key = os.environ.get("GEMINI_API_KEY", "")

        self.root.protocol("WM_DELETE_WINDOW", self._on_close)

        self._setup_styles()
        self._build_ui()
        self._start_new_interview()

    def _on_close(self):
        try:
            self.root.destroy()
        except Exception:
            pass
        os._exit(0)

    def _setup_styles(self):
        style = ttk.Style()
        style.theme_use("clam")
        style.configure("TFrame", background=self.bg_dark)
        style.configure("Card.TFrame", background=self.bg_card)
        style.configure("TLabel", background=self.bg_dark, foreground=self.text_light, font=("Segoe UI", 10))
        style.configure("Header.TLabel", background=self.bg_card, foreground=self.text_light, font=("Segoe UI", 12, "bold"))
        style.configure("Sub.TLabel", background=self.bg_card, foreground=self.text_dim, font=("Segoe UI", 9))
        style.configure("TButton", font=("Segoe UI", 9, "bold"))

    def _build_ui(self):
        # Top Header Bar
        header = tk.Frame(self.root, bg=self.bg_card, height=64, padx=18, pady=10)
        header.pack(fill=tk.X, side=tk.TOP)

        title_lbl = tk.Label(header, text="Alex Rivera", font=("Segoe UI", 13, "bold"), bg=self.bg_card, fg=self.text_light)
        title_lbl.pack(side=tk.LEFT)
        sub_lbl = tk.Label(header, text=" | Staff Lead Interviewer  [Adaptive Challenge Simulation]", font=("Segoe UI", 10), bg=self.bg_card, fg=self.accent_indigo)
        sub_lbl.pack(side=tk.LEFT)

        new_btn = tk.Button(header, text="New Challenge", bg="#1e293b", fg=self.text_light, bd=0, padx=12, pady=5, cursor="hand2", command=self._prompt_new_session)
        new_btn.pack(side=tk.RIGHT, padx=4)

        report_btn = tk.Button(header, text="Scorecard", bg=self.accent_indigo, fg="#ffffff", bd=0, padx=14, pady=5, cursor="hand2", command=self._show_scorecard)
        report_btn.pack(side=tk.RIGHT, padx=4)

        # Main Split Body
        body = tk.Frame(self.root, bg=self.bg_dark)
        body.pack(fill=tk.BOTH, expand=True, padx=16, pady=12)

        # Left Sidebar (Adaptive Status & Controls)
        sidebar = tk.Frame(body, bg=self.bg_card, width=260, padx=14, pady=14)
        sidebar.pack(fill=tk.Y, side=tk.LEFT, padx=(0, 12))
        sidebar.pack_propagate(False)

        tk.Label(sidebar, text="ADAPTIVE CHALLENGE METER", font=("Segoe UI", 8, "bold"), bg=self.bg_card, fg=self.text_dim).pack(anchor="w", pady=(0, 6))

        self.lbl_tier = tk.Label(sidebar, text="Current Bracket: Level 2 (Mid-Level)", bg=self.bg_card, fg=self.accent_amber, font=("Segoe UI", 9, "bold"))
        self.lbl_tier.pack(anchor="w", pady=2)

        self.lbl_rating = tk.Label(sidebar, text="Skill Rating: 1,250 Elo", bg=self.bg_card, fg=self.accent_cyan, font=("Segoe UI", 9, "bold"))
        self.lbl_rating.pack(anchor="w", pady=2)

        self.lbl_streak = tk.Label(sidebar, text="Streak: 0 🔥", bg=self.bg_card, fg=self.text_light, font=("Segoe UI", 9))
        self.lbl_streak.pack(anchor="w", pady=2)

        self.lbl_prog = tk.Label(sidebar, text="Progress: 0 / 5", bg=self.bg_card, fg=self.text_dim, font=("Segoe UI", 9))
        self.lbl_prog.pack(anchor="w", pady=2)

        ttk.Separator(sidebar, orient="horizontal").pack(fill=tk.X, pady=12)

        tk.Label(sidebar, text="SIMULATION TOOLS", font=("Segoe UI", 8, "bold"), bg=self.bg_card, fg=self.text_dim).pack(anchor="w", pady=(0, 6))

        self.btn_ai = tk.Button(sidebar, text="🤖 Consult Simulation Lead", bg="#7c3aed", fg="#ffffff", font=("Segoe UI", 9, "bold"), bd=0, pady=7, cursor="hand2", command=self._request_ai_deep_review)
        self.btn_ai.pack(fill=tk.X, pady=4)

        self.btn_hint = tk.Button(sidebar, text="💡 Request Hint", bg="#1e293b", fg=self.text_light, bd=0, pady=6, cursor="hand2", command=self._get_hint)
        self.btn_hint.pack(fill=tk.X, pady=4)

        self.btn_model = tk.Button(sidebar, text="📖 Model Solution", bg="#1e293b", fg=self.text_light, bd=0, pady=6, cursor="hand2", command=self._show_model_answer)
        self.btn_model.pack(fill=tk.X, pady=4)

        # Right Chat Area
        chat_frame = tk.Frame(body, bg=self.bg_dark)
        chat_frame.pack(fill=tk.BOTH, expand=True, side=tk.RIGHT)

        # Chat Transcript Box
        self.chat_display = scrolledtext.ScrolledText(
            chat_frame, wrap=tk.WORD, bg=self.bg_card, fg=self.text_light,
            font=("Segoe UI", 10), bd=0, padx=16, pady=16, insertbackground="#ffffff"
        )
        self.chat_display.pack(fill=tk.BOTH, expand=True, pady=(0, 10))
        self.chat_display.config(state=tk.DISABLED)

        # Tags for styled chat text
        self.chat_display.tag_config("interviewer_title", foreground="#818cf8", font=("Segoe UI", 10, "bold"))
        self.chat_display.tag_config("candidate_title", foreground="#38bdf8", font=("Segoe UI", 10, "bold"))
        self.chat_display.tag_config("question_box", foreground="#e2e8f0", background="#090d16", font=("Segoe UI", 10))
        self.chat_display.tag_config("curveball_box", foreground="#fef3c7", background="#2a1e05", font=("Segoe UI", 9, "italic"))
        self.chat_display.tag_config("code_block", foreground="#34d399", background="#050811", font=("Consolas", 9))
        self.chat_display.tag_config("score_tag", foreground=self.accent_green, font=("Segoe UI", 10, "bold"))
        self.chat_display.tag_config("shift_tag", foreground=self.accent_cyan, background="#0a1a2e", font=("Segoe UI", 9, "bold"))
        self.chat_display.tag_config("ai_tag", foreground="#c084fc", background="#1a1030", font=("Segoe UI", 10))
        self.chat_display.tag_config("hint_tag", foreground=self.accent_amber, font=("Segoe UI", 10))

        # Input Box & Actions
        input_bar = tk.Frame(chat_frame, bg=self.bg_card, padx=12, pady=10)
        input_bar.pack(fill=tk.X, side=tk.BOTTOM)

        self.input_entry = tk.Text(input_bar, height=3, bg=self.bg_input, fg=self.text_light, font=("Segoe UI", 10), bd=1, relief="solid", insertbackground="#ffffff")
        self.input_entry.pack(fill=tk.X, side=tk.LEFT, expand=True, padx=(0, 10))
        self.input_entry.bind("<Control-Return>", lambda e: self._submit_answer())

        btn_box = tk.Frame(input_bar, bg=self.bg_card)
        btn_box.pack(side=tk.RIGHT)

        self.submit_btn = tk.Button(btn_box, text="Submit Answer\n(Ctrl+Enter)", bg=self.accent_indigo, fg="#ffffff", font=("Segoe UI", 9, "bold"), bd=0, padx=14, pady=6, cursor="hand2", command=self._submit_answer)
        self.submit_btn.pack(fill=tk.X, pady=2)

        self.skip_btn = tk.Button(btn_box, text="Pass", bg="#1e293b", fg=self.text_dim, font=("Segoe UI", 8), bd=0, pady=2, cursor="hand2", command=self._skip_question)
        self.skip_btn.pack(fill=tk.X, pady=2)

    def _append_chat(self, sender, text, tag=None):
        self.chat_display.config(state=tk.NORMAL)
        if sender == "interviewer":
            self.chat_display.insert(tk.END, "Alex Rivera (Staff Lead):\n", "interviewer_title")
        elif sender == "candidate":
            self.chat_display.insert(tk.END, "You (Candidate):\n", "candidate_title")
        elif sender == "ai":
            self.chat_display.insert(tk.END, "🤖 Alex Rivera (Staff Simulation Debrief):\n", "ai_tag")
        
        self.chat_display.insert(tk.END, f"{text}\n\n", tag or "question_box")
        self.chat_display.config(state=tk.DISABLED)
        self.chat_display.see(tk.END)

    def _start_new_interview(self, topic="all", difficulty="adaptive", num_questions=5):
        self.session = AdaptiveInterviewSession(topic=topic, difficulty=difficulty, num_questions=num_questions, api_key=self.api_key)
        self.chat_display.config(state=tk.NORMAL)
        self.chat_display.delete("1.0", tk.END)
        self.chat_display.config(state=tk.DISABLED)

        self._update_sidebar_hud()
        self._append_chat(
            "interviewer",
            "🎯 Welcome to the Adaptive Interview Challenge!\n"
            "This simulation dynamically scales difficulty question-by-question based on your real-time performance.\n"
            "High scores trigger Level Ups to Senior and Staff architecture challenges."
        )
        self._present_current_question()

    def _update_sidebar_hud(self):
        if not self.session:
            return
        self.lbl_tier.config(text=f"Current Bracket: Level {self.session.current_level} ({self.session.current_tier})")
        self.lbl_rating.config(text=f"Skill Rating: {self.session.skill_rating:,} Elo")
        self.lbl_streak.config(text=f"Streak: {self.session.streak} 🔥")
        self.lbl_prog.config(text=f"Progress: {len(self.session.turns)} / {self.session.num_questions}")

    def _present_current_question(self):
        q = self.session.current_question()
        if not q:
            return

        self._update_sidebar_hud()
        text = f"Challenge {q['index']}: {q['title']}\n[Bracket: Level {q.get('level_number', 2)} ({q['difficulty']}) | Domain: {q['topic']}]\n\n{q['question']}"
        if q.get("curveball"):
            text += f"\n\n⚡ In-Flight Curveball: {q['curveball']}"
        if q.get("code_snippet"):
            text += f"\n\n--- Code Snippet ---\n{q['code_snippet']}\n--------------------"

        self._append_chat("interviewer", text, "question_box")

    def _submit_answer(self):
        ans = self.input_entry.get("1.0", tk.END).strip()
        if not ans:
            return

        self.input_entry.delete("1.0", tk.END)
        self._append_chat("candidate", ans)
        self.submit_btn.config(state=tk.DISABLED, text="Evaluating...")

        def run_eval():
            res = self.session.submit_answer(ans)
            self.root.after(0, lambda: self._process_eval_result(res))

        threading.Thread(target=run_eval, daemon=True).start()

    def _process_eval_result(self, res):
        self.submit_btn.config(state=tk.NORMAL, text="Submit Answer\n(Ctrl+Enter)")
        eval_res = res["evaluation"]
        ad = res.get("adaptation", {})

        # Display adaptive shift
        if ad.get("message"):
            self._append_chat("interviewer", f"⚡ {ad['message']} [Rating: {ad.get('skill_rating')} ({ad.get('rating_delta', 0):+d})]", "shift_tag")

        eval_summary = f"Score: {eval_res['score']}/10  [{eval_res['rating']}]\n"
        eval_summary += f"Feedback: \"{eval_res['interviewer_critique']}\"\n"
        if eval_res.get("strengths"):
            eval_summary += "\n+ Strengths: " + "; ".join(eval_res["strengths"])
        if eval_res.get("improvements"):
            eval_summary += "\n- Nuances: " + "; ".join(eval_res["improvements"])
        if eval_res.get("follow_up_question"):
            eval_summary += f"\n\nFollow-up: {eval_res['follow_up_question']}"

        self._append_chat("interviewer", eval_summary, "score_tag")
        self._update_sidebar_hud()

        if res.get("is_completed"):
            self.lbl_prog.config(text="Simulation Complete!")
            self._append_chat("interviewer", "🎉 Simulation Complete! Click 'Scorecard' at the top to review your calibrated hiring recommendation and trajectory.")
            self._show_scorecard()
        else:
            self._present_current_question()

    def _request_ai_deep_review(self):
        if not self.session or not self.session.turns:
            messagebox.showinfo("Simulation Debrief", "Please submit an answer first before consulting the lead interviewer.")
            return

        if not self.api_key:
            key = tk.simpledialog.askstring("Gemini API Key", "Enter your Gemini API key to activate the Simulation Interviewer:")
            if not key:
                return
            self.api_key = key
            self.session.api_key = key

        self.btn_ai.config(state=tk.DISABLED, text="Analyzing...")

        def run_ai():
            res = self.session.get_ai_enhanced_eval(self.api_key)
            self.root.after(0, lambda: self._process_ai_result(res))

        threading.Thread(target=run_ai, daemon=True).start()

    def _process_ai_result(self, res):
        self.btn_ai.config(state=tk.NORMAL, text="🤖 Consult Simulation Lead")
        if res.get("error"):
            messagebox.showwarning("Debrief Note", res["error"])
            return

        ai = res["ai_analysis"]
        text = f"Hiring Decision: [{ai.get('hiring_verdict', 'Hire')}]  (Score: {ai.get('ai_score', 'N/A')}/10)\n\n"
        text += f"{ai.get('ai_critique', '')}\n\n"
        if ai.get("key_architectural_insight"):
            text += f"💡 Architectural Takeaway: {ai.get('key_architectural_insight')}\n\n"
        if ai.get("staff_nuances"):
            text += "Staff Nuances:\n• " + "\n• ".join(ai["staff_nuances"]) + "\n\n"
        if ai.get("curveball_challenge"):
            text += f"⚡ In-Flight Constraint: {ai.get('curveball_challenge')}\n\n"
        if ai.get("probing_follow_up"):
            text += f"Follow-up: {ai.get('probing_follow_up')}"

        self._append_chat("ai", text, "ai_tag")

    def _get_hint(self):
        hint = self.session.get_hint()
        if hint.get("hint"):
            self._append_chat("interviewer", f"💡 Hint #{hint.get('hint_number')}: {hint.get('hint')}", "hint_tag")

    def _show_model_answer(self):
        if not messagebox.askyesno("Reveal Model Solution", "Reveal the model solution for this challenge?"):
            return
        ans = self.session.get_model_answer()
        self._append_chat("interviewer", f"📖 Model Solution ({ans.get('title')}):\n\n{ans.get('model_answer')}", "code_block")

    def _skip_question(self):
        self.input_entry.delete("1.0", tk.END)
        self.input_entry.insert("1.0", "I pass on this challenge to review the model solution.")
        self._submit_answer()

    def _prompt_new_session(self):
        win = tk.Toplevel(self.root)
        win.title("Configure Adaptive Simulation")
        win.geometry("420x340")
        win.configure(bg=self.bg_card)

        tk.Label(win, text="Select Mode / Strategy:", bg=self.bg_card, fg=self.text_light).pack(anchor="w", padx=16, pady=(12, 2))
        diff_cb = ttk.Combobox(win, values=["adaptive", "Staff / Principal", "Senior / Lead", "Mid-Level", "Junior"], state="readonly")
        diff_cb.set("adaptive")
        diff_cb.pack(fill=tk.X, padx=16)

        tk.Label(win, text="Select Focus Domain:", bg=self.bg_card, fg=self.text_light).pack(anchor="w", padx=16, pady=(10, 2))
        topic_cb = ttk.Combobox(win, values=list(TOPICS.keys()), state="readonly")
        topic_cb.set("all")
        topic_cb.pack(fill=tk.X, padx=16)

        tk.Label(win, text="Challenge Length:", bg=self.bg_card, fg=self.text_light).pack(anchor="w", padx=16, pady=(10, 2))
        num_cb = ttk.Combobox(win, values=[3, 5, 8], state="readonly")
        num_cb.set(5)
        num_cb.pack(fill=tk.X, padx=16)

        def apply():
            top = topic_cb.get()
            diff = diff_cb.get()
            num = int(num_cb.get())
            win.destroy()
            self._start_new_interview(top, diff, num)

        tk.Button(win, text="Start Adaptive Challenge", bg=self.accent_indigo, fg="#ffffff", font=("Segoe UI", 9, "bold"), bd=0, pady=7, cursor="hand2", command=apply).pack(fill=tk.X, padx=16, pady=20)

    def _show_scorecard(self):
        rep = self.session.generate_report()
        if rep.get("error"):
            messagebox.showinfo("Scorecard", rep["error"])
            return

        win = tk.Toplevel(self.root)
        win.title("Adaptive Simulation Scorecard")
        win.geometry("600x580")
        win.configure(bg=self.bg_card)

        txt = scrolledtext.ScrolledText(win, wrap=tk.WORD, bg=self.bg_dark, fg=self.text_light, font=("Segoe UI", 10), padx=14, pady=14)
        txt.pack(fill=tk.BOTH, expand=True, padx=16, pady=16)

        summary = f"""==================================================
        ADAPTIVE SIMULATION SCORECARD          
==================================================
Calibrated Level: {rep.get('calibrated_level', 'Senior')}
Verdict:          {rep['verdict']}
Final Skill Elo:  {rep.get('final_skill_rating')} (Peak: {rep.get('peak_skill_rating')})
Overall Score:    {rep['overall_score']} / 10
Duration:         {rep['total_duration_formatted']}
Questions:        {rep['total_questions_answered']}

Hiring Committee Debrief:
{rep.get('hiring_summary', rep.get('summary', ''))}

--- Trajectory Progression ---
"""
        for item in rep.get("trajectory", []):
            if item.get("question_index", 0) > 0:
                summary += f"• Q{item['question_index']} [{item['tier']} - Level {item['level']}]: Score {item.get('score')}/10 -> {item['skill_rating']} Elo ({item['reason']})\n"

        summary += "\n--- Domain Competency Breakdown ---\n"
        for top, data in rep["topic_breakdown"].items():
            summary += f"• {data['topic_name']}: {data['avg_score']}/10\n"

        txt.insert(tk.END, summary)
        txt.config(state=tk.DISABLED)


def main():
    root = tk.Tk()
    app = OOPInterviewDesktopApp(root)
    try:
        root.mainloop()
    finally:
        os._exit(0)


if __name__ == "__main__":
    main()
