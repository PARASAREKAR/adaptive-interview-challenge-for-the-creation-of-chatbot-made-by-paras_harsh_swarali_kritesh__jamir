/**
 * ALEX RIVERA | ADAPTIVE INTERVIEW CHALLENGE - CLIENT SIMULATION ENGINE (v2)
 * Features:
 * - Dynamic Difficulty Adaptation (Junior -> Mid-Level -> Senior -> Staff)
 * - Real-time Elo-style Skill Rating & Streak Tracking
 * - In-flight Interviewer Curveballs & Production Constraints
 * - Manual on-demand Staff AI Debriefs ("Consult Simulation Interviewer")
 * - Calibrated FAANG Hiring Recommendation Scorecard
 */

document.addEventListener("DOMContentLoaded", () => {
    // -------------------------------------------------------------------------
    // APPLICATION STATE
    // -------------------------------------------------------------------------
    let sessionId = null;
    let currentQuestion = null;
    let questionsRoadmap = [];
    let voiceEnabled = false;
    let isRecording = false;
    let recognition = null;
    let timerSeconds = 0;
    let timerInterval = null;
    let apiKey = localStorage.getItem("oop_gemini_api_key") || "";

    // Adaptive HUD State
    let currentTier = "Mid-Level";
    let currentLevel = 2;
    let skillRating = 1250;
    let streakCount = 0;
    let isAdaptive = true;

    // -------------------------------------------------------------------------
    // DOM ELEMENTS
    // -------------------------------------------------------------------------
    const messagesContainer = document.getElementById("messagesContainer");
    const answerInput = document.getElementById("answerInput");
    const submitBtn = document.getElementById("submitBtn");
    const passBtn = document.getElementById("passBtn");
    const micBtn = document.getElementById("micBtn");
    const insertCodeSnippetBtn = document.getElementById("insertCodeSnippetBtn");
    const charCount = document.getElementById("charCount");
    const timerDisplay = document.getElementById("timerDisplay");
    const voiceToggleBtn = document.getElementById("voiceToggleBtn");

    // Adaptive HUD in Navbar
    const hudTierLabel = document.getElementById("hudTierLabel");
    const hudRatingLabel = document.getElementById("hudRatingLabel");
    const hudStreakLabel = document.getElementById("hudStreakLabel");

    // Sidebar
    const metaTopic = document.getElementById("metaTopic");
    const metaDifficulty = document.getElementById("metaDifficulty");
    const metaMode = document.getElementById("metaMode");
    const metaProgress = document.getElementById("metaProgress");
    const progressBarFill = document.getElementById("progressBarFill");
    const questionRoadmapList = document.getElementById("questionRoadmapList");
    const sidebarHintBtn = document.getElementById("sidebarHintBtn");
    const hintsRemainingBadge = document.getElementById("hintsRemainingBadge");
    const sidebarModelAnswerBtn = document.getElementById("sidebarModelAnswerBtn");
    const sidebarCurveballBtn = document.getElementById("sidebarCurveballBtn");
    const newInterviewBtn = document.getElementById("newInterviewBtn");

    // Modals & Drawers
    const settingsModal = document.getElementById("settingsModal");
    const settingsBtn = document.getElementById("settingsBtn");
    const closeSettingsModal = document.getElementById("closeSettingsModal");
    const cancelSettingsBtn = document.getElementById("cancelSettingsBtn");
    const applySettingsBtn = document.getElementById("applySettingsBtn");
    const geminiApiKeyInput = document.getElementById("geminiApiKey");

    const scorecardModal = document.getElementById("scorecardModal");
    const finishInterviewBtn = document.getElementById("finishInterviewBtn");
    const closeScorecardModal = document.getElementById("closeScorecardModal");
    const scorecardBody = document.getElementById("scorecardBody");
    const copyReportBtn = document.getElementById("copyReportBtn");
    const restartFromScorecardBtn = document.getElementById("restartFromScorecardBtn");

    const cheatSheetDrawer = document.getElementById("cheatSheetDrawer");
    const cheatSheetBtn = document.getElementById("cheatSheetBtn");
    const closeCheatSheetDrawer = document.getElementById("closeCheatSheetDrawer");

    if (apiKey) geminiApiKeyInput.value = apiKey;

    // -------------------------------------------------------------------------
    // INITIALIZATION
    // -------------------------------------------------------------------------
    initSpeechRecognition();
    startNewSession();

    // -------------------------------------------------------------------------
    // SESSION MANAGEMENT
    // -------------------------------------------------------------------------
    async function startNewSession() {
        const topic = document.getElementById("topicSelect").value;
        const difficulty = document.getElementById("difficultySelect").value;
        const numQuestions = document.getElementById("numQuestionsSelect").value;
        apiKey = geminiApiKeyInput.value.trim();
        if (apiKey) localStorage.setItem("oop_gemini_api_key", apiKey);

        // Reset display
        messagesContainer.innerHTML = "";
        questionRoadmapList.innerHTML = "";
        resetTimer();
        startTimer();

        metaTopic.textContent = document.getElementById("topicSelect").selectedOptions[0].text.split("(")[0].trim();
        metaDifficulty.textContent = difficulty === "adaptive" ? "Adaptive Scale" : difficulty;
        metaMode.textContent = difficulty === "adaptive" ? "Dynamic Escalation" : "Fixed Difficulty";

        // Welcome Greeting
        appendInterviewerMessage(`### 🎯 Adaptive Interview Challenge Initiated!
I'm **Alex Rivera**, Staff Software Engineer & Lead Interviewer. You are entering an **Adaptive FAANG Mock Interview Simulation**.

⚡ **How This Challenge Works:**
1. **Dynamic Escalation:** Your performance dictates the difficulty. High scores (≥8/10) trigger **Level Ups** to Senior/Staff architecture challenges; struggling triggers foundational checks.
2. **Skill Rating (Elo):** Track your real-time candidate rating and streaks in the top HUD.
3. **In-Flight Curveballs:** Watch for unexpected production constraints attached to challenges.
4. **Manual AI Debrief:** Click **"🤖 Consult Simulation Interviewer"** anytime to receive my calibrated hiring decision and architectural feedback.

Let's begin with our first scenario!`);

        try {
            const res = await fetch("/api/start", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    topic: topic,
                    difficulty: difficulty,
                    num_questions: numQuestions,
                    api_key: apiKey
                })
            });
            const data = await res.json();
            sessionId = data.session_id;
            currentQuestion = data.first_question;
            isAdaptive = data.is_adaptive;
            currentTier = data.initial_tier || "Mid-Level";
            skillRating = data.initial_rating || 1250;
            streakCount = 0;

            updateAdaptiveHUD(currentTier, 2, skillRating, streakCount);

            // Setup roadmap
            questionsRoadmap = Array.from({ length: data.total_selected }, (_, i) => ({
                index: i + 1,
                title: i === 0 ? currentQuestion.title : `Challenge ${i + 1}`,
                status: i === 0 ? "active" : "pending",
                tier: i === 0 ? currentQuestion.difficulty : null,
                score: null
            }));
            renderRoadmap();
            updateProgress(currentQuestion.index, currentQuestion.total_questions);

            // Present first question
            presentQuestion(currentQuestion);
        } catch (err) {
            console.error("Failed to start session:", err);
            appendInterviewerMessage("⚠️ Error initializing adaptive session. Please verify backend server is running.");
        }
    }

    function updateAdaptiveHUD(tier, levelNum, rating, streak) {
        currentTier = tier;
        currentLevel = levelNum;
        skillRating = rating;
        streakCount = streak;

        hudTierLabel.textContent = `Level ${levelNum}: ${tier}`;
        hudRatingLabel.textContent = `${rating.toLocaleString()} Rating`;
        hudStreakLabel.textContent = `${streak} Streak`;
        metaDifficulty.textContent = tier;

        // Update ladder dots
        for (let i = 1; i <= 4; i++) {
            const stepEl = document.getElementById(`ladderStep${i}`);
            if (stepEl) {
                stepEl.classList.toggle("active", i === levelNum);
            }
        }
    }

    function presentQuestion(q) {
        if (!q) return;

        let content = `### Challenge ${q.index}: ${q.title}
<div class="question-hero-banner">
    <div class="question-tags-row">
        <span class="q-tag topic">${q.topic.replace(/_/g, ' ')}</span>
        <span class="q-tag difficulty">${q.difficulty}</span>
        <span class="q-tag" style="background: rgba(99, 102, 241, 0.2); color: #c7d2fe;">Level ${q.level_number || 2}</span>
    </div>
    <p>${q.question}</p>
</div>`;

        if (q.curveball) {
            content += `
<div class="curveball-box">
    <div class="curveball-header">
        <i class="fa-solid fa-bolt-lightning"></i>
        <span>Interviewer In-Flight Curveball:</span>
    </div>
    <p class="curveball-body">${q.curveball}</p>
</div>`;
        }

        if (q.code_snippet) {
            content += `\n\`\`\`python\n${q.code_snippet}\n\`\`\`\n`;
        }

        appendInterviewerMessage(content);
        hintsRemainingBadge.textContent = `${q.total_hints} Left`;

        if (voiceEnabled) {
            speakText(`Challenge ${q.index}. ${q.title}. ${q.question}`);
        }
    }

    // -------------------------------------------------------------------------
    // SUBMIT ANSWER & ADAPTIVE DIFFICULTY TRANSITION
    // -------------------------------------------------------------------------
    async function handleAnswerSubmission() {
        const answer = answerInput.value.trim();
        if (!answer) return;

        appendCandidateMessage(answer);
        answerInput.value = "";
        updateCharCount();
        submitBtn.disabled = true;

        try {
            const res = await fetch("/api/submit-answer", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    session_id: sessionId,
                    answer: answer
                })
            });

            const data = await res.json();
            submitBtn.disabled = false;

            if (data.error) {
                appendInterviewerMessage(`⚠️ ${data.error}`);
                return;
            }

            // Update Adaptive HUD with new rating and level
            if (data.adaptation) {
                const ad = data.adaptation;
                updateAdaptiveHUD(ad.new_tier, ad.level_number, ad.skill_rating, ad.streak);
            }

            // Render evaluation card with adaptive notification banner
            renderEvaluationCard(data.evaluation, data.adaptation);

            // Update roadmap status
            const prevIndex = currentQuestion.index - 1;
            if (questionsRoadmap[prevIndex]) {
                questionsRoadmap[prevIndex].status = "completed";
                questionsRoadmap[prevIndex].score = data.evaluation.score;
                questionsRoadmap[prevIndex].title = currentQuestion.title;
                questionsRoadmap[prevIndex].tier = currentQuestion.difficulty;
            }

            // Advance or complete
            if (data.is_completed) {
                stopTimer();
                updateProgress(currentQuestion.total_questions, currentQuestion.total_questions);
                renderRoadmap();
                handleInterviewCompletion();
            } else {
                currentQuestion = data.next_question;
                if (questionsRoadmap[currentQuestion.index - 1]) {
                    questionsRoadmap[currentQuestion.index - 1].status = "active";
                    questionsRoadmap[currentQuestion.index - 1].title = currentQuestion.title;
                    questionsRoadmap[currentQuestion.index - 1].tier = currentQuestion.difficulty;
                }
                updateProgress(currentQuestion.index, currentQuestion.total_questions);
                renderRoadmap();

                // Present next question after short delay
                setTimeout(() => {
                    presentQuestion(currentQuestion);
                }, 1700);
            }
        } catch (err) {
            console.error("Submission failed:", err);
            submitBtn.disabled = false;
            appendInterviewerMessage("⚠️ Error processing answer. Please try again.");
        }
    }

    // -------------------------------------------------------------------------
    // RENDER EVALUATION CARD & ADAPTIVE BANNER
    // -------------------------------------------------------------------------
    function renderEvaluationCard(evalData, adaptation) {
        const ratingClass = evalData.score >= 8.0 ? "strong" : evalData.score >= 5.0 ? "moderate" : "poor";

        let bannerHtml = "";
        if (adaptation && adaptation.message) {
            const bannerType = adaptation.event.toLowerCase();
            const icon = adaptation.event === "LEVEL_UP" ? "fa-circle-up" : adaptation.event === "SCAFFOLD" ? "fa-circle-down" : "fa-equals";
            bannerHtml = `
<div class="adaptive-shift-banner ${bannerType}">
    <i class="fa-solid ${icon} shift-icon"></i>
    <span>${adaptation.message}</span>
    <span style="margin-left: auto; font-family: var(--font-mono); font-size: 0.76rem;">Rating: ${adaptation.skill_rating} (${adaptation.rating_delta >= 0 ? '+' : ''}${adaptation.rating_delta})</span>
</div>`;
        }

        const strengthsHtml = (evalData.strengths || []).map(s => 
            `<li><i class="fa-solid fa-circle-check" style="color: var(--accent-emerald);"></i> <span>${s}</span></li>`
        ).join("");

        const improvementsHtml = (evalData.improvements || []).map(imp => 
            `<li><i class="fa-solid fa-triangle-exclamation" style="color: var(--accent-amber);"></i> <span>${imp}</span></li>`
        ).join("");

        const cardHtml = `
<div class="eval-card">
    ${bannerHtml}

    <div class="eval-card-header">
        <div class="score-display">
            <span class="score-value">${evalData.score}</span>
            <span class="score-denominator">/ 10</span>
        </div>
        <span class="rating-badge ${ratingClass}">${evalData.rating}</span>
    </div>

    <div class="eval-critique-box">
        "${evalData.interviewer_critique}"
    </div>

    <div class="eval-feedback-grid">
        ${strengthsHtml ? `
        <div class="feedback-section">
            <span class="feedback-title strengths"><i class="fa-solid fa-check"></i> Key Strengths</span>
            <ul class="feedback-list">${strengthsHtml}</ul>
        </div>` : ""}

        ${improvementsHtml ? `
        <div class="feedback-section">
            <span class="feedback-title improvements"><i class="fa-solid fa-lightbulb"></i> Architectural Nuances</span>
            <ul class="feedback-list">${improvementsHtml}</ul>
        </div>` : ""}
    </div>

    ${evalData.follow_up_question ? `
    <div class="follow-up-card">
        <strong><i class="fa-solid fa-circle-question" style="color: var(--accent-indigo);"></i> Interviewer Follow-up:</strong>
        <p style="margin-top: 4px;">${evalData.follow_up_question}</p>
    </div>` : ""}

    <!-- MANUAL AI SIMULATION ACTIONS -->
    <div class="ai-trigger-container">
        <button class="pill-btn" onclick="window.triggerLiveCurveball(this)" title="Ask Alex to inject a real-time production constraint">
            <i class="fa-solid fa-bolt-lightning text-amber"></i>
            <span>Throw Live Curveball</span>
        </button>
        <button class="ai-enhance-btn" onclick="window.triggerAIEnhance(this)">
            <i class="fa-solid fa-wand-magic-sparkles"></i>
            <span>Consult Simulation Interviewer (Alex)</span>
        </button>
    </div>
</div>`;

        appendInterviewerMessage(cardHtml, false);

        if (voiceEnabled && evalData.interviewer_critique) {
            speakText(evalData.interviewer_critique);
        }
    }

    // -------------------------------------------------------------------------
    // MANUAL AI SIMULATION INTERVIEWER DEBRIEF
    // -------------------------------------------------------------------------
    window.triggerAIEnhance = async function(btn) {
        if (!apiKey) {
            apiKey = prompt("Please enter your Gemini API Key to activate the Staff Simulation AI (stored locally in browser):");
            if (!apiKey) return;
            localStorage.setItem("oop_gemini_api_key", apiKey);
            geminiApiKeyInput.value = apiKey;
        }

        btn.disabled = true;
        btn.innerHTML = `<i class="fa-solid fa-spinner fa-spin"></i> <span>Alex Rivera is analyzing your simulation trajectory...</span>`;

        try {
            const res = await fetch("/api/ai-enhance", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    session_id: sessionId,
                    api_key: apiKey
                })
            });

            const data = await res.json();

            if (data.error) {
                btn.disabled = false;
                btn.innerHTML = `<i class="fa-solid fa-wand-magic-sparkles"></i> <span>Retry Simulation Debrief</span>`;
                alert(`Simulation Debrief Note: ${data.error}`);
                return;
            }

            btn.parentElement.remove();
            renderAIAnalysisCard(data.ai_analysis);
        } catch (err) {
            console.error("AI debrief failed:", err);
            btn.disabled = false;
            btn.innerHTML = `<i class="fa-solid fa-wand-magic-sparkles"></i> <span>Retry Simulation Debrief</span>`;
        }
    };

    function renderAIAnalysisCard(aiData) {
        const nuancesHtml = (aiData.staff_nuances || []).map(s => 
            `<li><i class="fa-solid fa-compass" style="color: #c084fc;"></i> <span>${s}</span></li>`
        ).join("");

        const aiCardHtml = `
<div class="ai-analysis-card">
    <div class="ai-analysis-header">
        <div class="ai-header-left">
            <div class="ai-spark-icon"><i class="fa-solid fa-brain"></i></div>
            <div class="ai-header-titles">
                <h4>Alex Rivera — Staff Simulation Debrief</h4>
                <span>Calibrated Candidate Assessment & Seniority Radar</span>
            </div>
        </div>
        <div class="hiring-verdict-badge">
            ${aiData.hiring_verdict || "Hire"}
        </div>
    </div>

    <div class="ai-critique-text">
        ${aiData.ai_critique}
    </div>

    ${aiData.key_architectural_insight ? `
    <div class="ai-insight-box">
        <strong>💡 Core Architectural Takeaway:</strong> ${aiData.key_architectural_insight}
    </div>` : ""}

    ${nuancesHtml ? `
    <div class="feedback-section">
        <span class="feedback-title" style="color: #c084fc;"><i class="fa-solid fa-layer-group"></i> Staff-Level Subtleties & Edge Cases:</span>
        <ul class="ai-suggestions-list">${nuancesHtml}</ul>
    </div>` : ""}

    ${aiData.curveball_challenge ? `
    <div class="curveball-box" style="margin-top: 6px;">
        <div class="curveball-header">
            <i class="fa-solid fa-triangle-exclamation"></i>
            <span>Interviewer In-Flight Constraint:</span>
        </div>
        <p class="curveball-body">${aiData.curveball_challenge}</p>
    </div>` : ""}

    ${aiData.probing_follow_up ? `
    <div class="follow-up-card" style="border-color: rgba(168, 85, 247, 0.4); background: rgba(168, 85, 247, 0.08);">
        <strong><i class="fa-solid fa-comments text-accent"></i> Deep-Dive Follow-up Question:</strong>
        <p style="margin-top: 4px; color: #f3e8ff;">${aiData.probing_follow_up}</p>
    </div>` : ""}
</div>`;

        appendInterviewerMessage(aiCardHtml, false);

        if (voiceEnabled && aiData.ai_critique) {
            speakText(aiData.ai_critique);
        }
    }

    // -------------------------------------------------------------------------
    // LIVE CURVEBALL INJECTION
    // -------------------------------------------------------------------------
    window.triggerLiveCurveball = async function(btn) {
        btn.disabled = true;
        btn.innerHTML = `<i class="fa-solid fa-spinner fa-spin"></i> Inoculating constraint...`;

        try {
            const res = await fetch("/api/curveball", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    session_id: sessionId,
                    api_key: apiKey
                })
            });

            const data = await res.json();
            btn.parentElement.remove();

            if (data.curveball) {
                const cbHtml = `
<div class="curveball-box" style="animation: bannerPop 0.3s ease;">
    <div class="curveball-header">
        <i class="fa-solid fa-bolt-lightning text-amber"></i>
        <span>${data.curveball.title || "Live Production Constraint"}:</span>
    </div>
    <p class="curveball-body">${data.curveball.challenge}</p>
</div>`;
                appendInterviewerMessage(cbHtml, false);

                if (voiceEnabled) {
                    speakText(`Interviewer twist: ${data.curveball.challenge}`);
                }
            }
        } catch (err) {
            console.error("Curveball failed:", err);
            btn.disabled = false;
        }
    };

    sidebarCurveballBtn.addEventListener("click", () => {
        window.triggerLiveCurveball(sidebarCurveballBtn);
    });

    // -------------------------------------------------------------------------
    // SIMULATION COMPLETION & ADAPTIVE SCORECARD
    // -------------------------------------------------------------------------
    function handleInterviewCompletion() {
        if (typeof confetti === "function") {
            confetti({
                particleCount: 110,
                spread: 80,
                origin: { y: 0.6 }
            });
        }

        appendInterviewerMessage(`### 🏁 Adaptive Simulation Concluded!
Outstanding endurance! You have completed the **Adaptive Interview Challenge**. I have compiled your calibrated hiring recommendation, seniority tier, Elo progression, and domain breakdown into your **Simulation Scorecard**.

Click **"Scorecard"** in the top navigation to view your comprehensive assessment!`);

        setTimeout(openScorecard, 1600);
    }

    // -------------------------------------------------------------------------
    // HINTS & MODEL SOLUTIONS
    // -------------------------------------------------------------------------
    sidebarHintBtn.addEventListener("click", async () => {
        try {
            const res = await fetch(`/api/hint?session_id=${sessionId}`);
            const data = await res.json();
            if (data.hint) {
                appendInterviewerMessage(`💡 **Interviewer Hint #${data.hint_number}:**\n\n${data.hint}`);
                hintsRemainingBadge.textContent = `${data.remaining_hints} Left`;
            }
        } catch (err) {
            console.error("Hint error:", err);
        }
    });

    sidebarModelAnswerBtn.addEventListener("click", async () => {
        if (!confirm("Would you like to reveal the ideal model solution for the current question?")) return;
        try {
            const res = await fetch(`/api/model-answer?session_id=${sessionId}`);
            const data = await res.json();
            if (data.model_answer) {
                appendInterviewerMessage(`### 📖 Model Solution: ${data.title}\n\n${data.model_answer}`);
            }
        } catch (err) {
            console.error("Model answer error:", err);
        }
    });

    passBtn.addEventListener("click", () => {
        answerInput.value = "I am passing on this challenge to review the model solution and analyze the trade-offs.";
        handleAnswerSubmission();
    });

    // -------------------------------------------------------------------------
    // CANDIDATE SCORECARD & DEBRIEF MODAL
    // -------------------------------------------------------------------------
    async function openScorecard() {
        try {
            const res = await fetch(`/api/report?session_id=${sessionId}`);
            const report = await res.json();
            if (report.error) {
                alert(report.error);
                return;
            }

            let topicBarsHtml = "";
            for (const [key, t] of Object.entries(report.topic_breakdown || {})) {
                const pct = (t.avg_score / 10) * 100;
                topicBarsHtml += `
<div class="topic-bar-item">
    <div class="topic-bar-header">
        <span>${t.topic_name}</span>
        <span style="color: var(--accent-indigo);">${t.avg_score} / 10</span>
    </div>
    <div class="progress-track">
        <div class="progress-fill" style="width: ${pct}%;"></div>
    </div>
</div>`;
            }

            let trajectoryHtml = (report.trajectory || []).filter(item => item.question_index > 0).map(tr => `
<div class="trajectory-item">
    <div>
        <strong>Q${tr.question_index}: Level ${tr.level} (${tr.tier})</strong>
        <p style="font-size: 0.74rem; color: var(--text-muted);">${tr.reason}</p>
    </div>
    <div style="text-align: right; font-family: var(--font-mono);">
        <span style="color: ${tr.score >= 7.5 ? 'var(--accent-emerald)' : 'var(--accent-amber)'}; font-weight: 700;">Score: ${tr.score}/10</span>
        <div style="font-size: 0.72rem; color: var(--accent-cyan);">${tr.skill_rating} Rating</div>
    </div>
</div>`).join("");

            let turnsHtml = (report.detailed_turns || []).map((turn, i) => `
<div class="eval-card" style="margin-top: 12px;">
    <div class="eval-card-header">
        <strong>Q${i + 1}: ${turn.question_title} [${turn.difficulty}]</strong>
        <span class="score-display">
            <span class="score-value" style="font-size: 1.3rem;">${turn.evaluation.score}</span>
            <span class="score-denominator">/ 10</span>
        </span>
    </div>
    <p style="font-size: 0.84rem; color: var(--text-secondary); margin: 6px 0;">
        <strong>Your Answer:</strong> "${turn.candidate_answer.substring(0, 150)}${turn.candidate_answer.length > 150 ? '...' : ''}"
    </p>
    <p style="font-size: 0.85rem; color: #cbd5e1;">
        <strong>Interviewer Debrief:</strong> ${turn.evaluation.interviewer_critique}
    </p>
</div>`).join("");

            scorecardBody.innerHTML = `
<div class="scorecard-banner">
    <div class="scorecard-calibrated-level">
        ${report.calibrated_level}
    </div>
    <div class="scorecard-verdict" style="color: ${report.badge_color};">
        ${report.verdict}
    </div>
    <div class="scorecard-score-big">
        ${report.final_skill_rating} <span style="font-size: 1.2rem; color: var(--text-muted);">Rating (Peak: ${report.peak_skill_rating})</span>
    </div>
    <p style="color: var(--text-secondary); font-size: 0.92rem; max-width: 650px; margin: 0 auto;">
        ${report.hiring_summary}
    </p>
    <div style="font-size: 0.78rem; color: var(--text-muted); margin-top: 10px;">
        Avg Score: ${report.overall_score}/10 | Highest Bracket: ${report.highest_tier_reached} | Duration: ${report.total_duration_formatted}
    </div>
</div>

<h4 style="font-size: 0.82rem; text-transform: uppercase; color: var(--text-muted); margin-bottom: 8px;">
    Adaptive Difficulty Progression & Elo Trajectory
</h4>
<div class="trajectory-timeline">
    ${trajectoryHtml}
</div>

<h4 style="font-size: 0.82rem; text-transform: uppercase; color: var(--text-muted); margin: 20px 0 10px 0;">
    Domain Competency Radar
</h4>
<div class="topic-bars-grid">
    ${topicBarsHtml}
</div>

<h4 style="font-size: 0.82rem; text-transform: uppercase; color: var(--text-muted); margin: 20px 0 10px 0;">
    Detailed Challenge Log
</h4>
<div class="detailed-turns-list">
    ${turnsHtml}
</div>`;

            scorecardModal.style.display = "flex";

            scorecardModal.dataset.markdown = `# Adaptive Interview Challenge — Simulation Scorecard
**Calibrated Level:** ${report.calibrated_level}
**Verdict:** ${report.verdict}
**Final Skill Rating:** ${report.final_skill_rating} (Peak: ${report.peak_skill_rating})
**Average Score:** ${report.overall_score} / 10
**Duration:** ${report.total_duration_formatted}

### Hiring Committee Debrief:
${report.hiring_summary}

### Question Breakdown:
${(report.detailed_turns || []).map((t, idx) => `
#### ${idx + 1}. ${t.question_title} [${t.difficulty}] (Score: ${t.evaluation.score}/10)
- **Answer:** ${t.candidate_answer}
- **Debrief:** ${t.evaluation.interviewer_critique}
`).join("\n")}`;

        } catch (err) {
            console.error("Scorecard error:", err);
        }
    }

    copyReportBtn.addEventListener("click", () => {
        const md = scorecardModal.dataset.markdown;
        if (md) {
            navigator.clipboard.writeText(md);
            copyReportBtn.innerHTML = '<i class="fa-solid fa-check"></i> Copied!';
            setTimeout(() => {
                copyReportBtn.innerHTML = '<i class="fa-regular fa-copy"></i> Copy Markdown Report';
            }, 2000);
        }
    });

    finishInterviewBtn.addEventListener("click", openScorecard);
    closeScorecardModal.addEventListener("click", () => scorecardModal.style.display = "none");
    restartFromScorecardBtn.addEventListener("click", () => {
        scorecardModal.style.display = "none";
        settingsModal.style.display = "flex";
    });

    // -------------------------------------------------------------------------
    // UI HELPERS (ROADMAP, CHAT, TIMER)
    // -------------------------------------------------------------------------
    function renderRoadmap() {
        questionRoadmapList.innerHTML = questionsRoadmap.map(q => `
<li class="roadmap-item ${q.status}">
    <span class="roadmap-status-icon">
        ${q.status === 'completed' ? '<i class="fa-solid fa-check"></i>' : 
          q.status === 'active' ? '<i class="fa-solid fa-terminal"></i>' : 
          '<i class="fa-regular fa-circle"></i>'}
    </span>
    <span style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis; max-width: 170px;">
        ${q.title}
    </span>
    ${q.score !== null ? `<span class="roadmap-score-pill">${q.score}</span>` : ''}
</li>`).join("");
    }

    function updateProgress(current, total) {
        metaProgress.textContent = `${current} / ${total}`;
        const pct = Math.round((current / total) * 100);
        progressBarFill.style.width = `${pct}%`;
    }

    function appendInterviewerMessage(markdownText, parseMarkdown = true) {
        const row = document.createElement("div");
        row.className = "message-row interviewer";
        const html = parseMarkdown && typeof marked !== "undefined" ? marked.parse(markdownText) : markdownText;

        row.innerHTML = `
<div class="msg-avatar">
    <img src="https://api.dicebear.com/7.x/bottts/svg?seed=AlexRiveraOOP&backgroundColor=1e293b" alt="Alex">
</div>
<div class="msg-content">
    <div class="msg-header">
        <span class="msg-sender">Alex Rivera</span>
        <span>• Staff Simulation Lead</span>
    </div>
    <div class="msg-bubble">${html}</div>
</div>`;

        messagesContainer.appendChild(row);
        scrollToBottom();

        row.querySelectorAll("pre code").forEach(block => {
            if (window.hljs) hljs.highlightElement(block);
        });

        return row;
    }

    function appendCandidateMessage(text) {
        const row = document.createElement("div");
        row.className = "message-row candidate";
        const escaped = text.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        const formatted = escaped.replace(/\n/g, "<br>");

        row.innerHTML = `
<div class="msg-avatar">
    <img src="https://api.dicebear.com/7.x/identicon/svg?seed=CandidateUser" alt="You">
</div>
<div class="msg-content">
    <div class="msg-header" style="justify-content: flex-end;">
        <span>You (Candidate)</span>
    </div>
    <div class="msg-bubble">${formatted}</div>
</div>`;

        messagesContainer.appendChild(row);
        scrollToBottom();
        return row;
    }

    function scrollToBottom() {
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }

    function startTimer() {
        timerInterval = setInterval(() => {
            timerSeconds++;
            const mins = String(Math.floor(timerSeconds / 60)).padStart(2, "0");
            const secs = String(timerSeconds % 60).padStart(2, "0");
            timerDisplay.textContent = `${mins}:${secs}`;
        }, 1000);
    }

    function stopTimer() {
        clearInterval(timerInterval);
    }

    function resetTimer() {
        stopTimer();
        timerSeconds = 0;
        timerDisplay.textContent = "00:00";
    }

    function updateCharCount() {
        const words = answerInput.value.trim().split(/\s+/).filter(Boolean).length;
        charCount.textContent = `${words} words`;
    }

    // -------------------------------------------------------------------------
    // SPEECH RECOGNITION (STT) & SPEECH SYNTHESIS (TTS)
    // -------------------------------------------------------------------------
    function initSpeechRecognition() {
        const SpeechRec = window.SpeechRecognition || window.webkitSpeechRecognition;
        if (!SpeechRec) {
            micBtn.style.display = "none";
            return;
        }

        recognition = new SpeechRec();
        recognition.continuous = true;
        recognition.interimResults = true;
        recognition.lang = "en-US";

        recognition.onstart = () => {
            isRecording = true;
            micBtn.classList.add("recording");
            micBtn.querySelector(".mic-label").textContent = "Listening...";
        };

        recognition.onresult = (event) => {
            let transcript = "";
            for (let i = event.resultIndex; i < event.results.length; ++i) {
                transcript += event.results[i][0].transcript;
            }
            answerInput.value = (answerInput.value + " " + transcript).trim();
            updateCharCount();
        };

        recognition.onerror = () => stopRecording();
        recognition.onend = () => stopRecording();

        micBtn.addEventListener("click", () => {
            if (isRecording) {
                recognition.stop();
            } else {
                recognition.start();
            }
        });
    }

    function stopRecording() {
        isRecording = false;
        micBtn.classList.remove("recording");
        micBtn.querySelector(".mic-label").textContent = "Dictate";
    }

    function speakText(text) {
        if (!window.speechSynthesis) return;
        window.speechSynthesis.cancel();

        const cleanText = text.replace(/<[^>]*>/g, '').replace(/[*_#`]/g, '');
        const utterance = new SpeechSynthesisUtterance(cleanText);
        utterance.rate = 1.05;

        const voices = window.speechSynthesis.getVoices();
        const preferred = voices.find(v => v.lang.startsWith("en") && (v.name.includes("Google") || v.name.includes("Natural") || v.name.includes("Guy") || v.name.includes("David")));
        if (preferred) utterance.voice = preferred;

        window.speechSynthesis.speak(utterance);
    }

    voiceToggleBtn.addEventListener("click", () => {
        voiceEnabled = !voiceEnabled;
        voiceToggleBtn.classList.toggle("active", voiceEnabled);
        if (!voiceEnabled && window.speechSynthesis) {
            window.speechSynthesis.cancel();
        }
    });

    // -------------------------------------------------------------------------
    // EVENT LISTENERS
    // -------------------------------------------------------------------------
    submitBtn.addEventListener("click", handleAnswerSubmission);

    answerInput.addEventListener("keydown", (e) => {
        if ((e.ctrlKey || e.metaKey) && e.key === "Enter") {
            e.preventDefault();
            handleAnswerSubmission();
        }
    });

    answerInput.addEventListener("input", updateCharCount);

    insertCodeSnippetBtn.addEventListener("click", () => {
        const snippetTemplate = "\n```python\nclass Solution:\n    def example(self):\n        pass\n```\n";
        answerInput.value += snippetTemplate;
        answerInput.focus();
        updateCharCount();
    });

    settingsBtn.addEventListener("click", () => settingsModal.style.display = "flex");
    closeSettingsModal.addEventListener("click", () => settingsModal.style.display = "none");
    cancelSettingsBtn.addEventListener("click", () => settingsModal.style.display = "none");
    newInterviewBtn.addEventListener("click", () => settingsModal.style.display = "flex");

    applySettingsBtn.addEventListener("click", () => {
        settingsModal.style.display = "none";
        startNewSession();
    });

    cheatSheetBtn.addEventListener("click", () => cheatSheetDrawer.style.display = "flex");
    closeCheatSheetDrawer.addEventListener("click", () => cheatSheetDrawer.style.display = "none");
    cheatSheetDrawer.addEventListener("click", (e) => {
        if (e.target === cheatSheetDrawer) cheatSheetDrawer.style.display = "none";
    });
});
