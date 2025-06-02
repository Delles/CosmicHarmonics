# AI Collaboration Guide for "Cosmic Harmonics"

This document outlines preferences for how the AI assistant should provide guidance and structure tutorials during the development of "Cosmic Harmonics."

## General Principles:

1.  **Step-by-Step Format:** Provide instructions in a clear, numbered, step-by-step format for implementing features or setting up systems.
2.  **Blueprint Referencing:** At the beginning of each major task or feature implementation, explicitly state which sections of the `TECHNICAL_BLUEPRINT.md` are being addressed.
3.  **Goal Definition:** Clearly state the goal of the current set of steps/tutorial.
4.  **Code Snippets:**
    -   Provide complete, copy-pasteable C# code snippets where appropriate.
    -   Include explanations for key parts of the code.
    -   Ensure code follows naming conventions and best practices outlined in the blueprint.
5.  **Unity Editor Instructions:** Be specific about actions in the Unity Editor (e.g., "Window > Package Manager," "Right-click > Create > C# Script," "Drag script X onto GameObject Y in the Inspector").
6.  **Clarity on "Why":** Briefly explain the reasoning behind certain choices or steps, especially if there are alternatives, linking back to blueprint decisions if possible.
7.  **Assumptions:** State any assumptions being made (e.g., "Assuming Unity 2022.3.x LTS is installed").
8.  **Testing Steps:** Include simple steps or checks to verify that the implementation is working correctly.
9.  **Iterative Approach:** Acknowledge that this is an iterative process. Encourage questions and be prepared to clarify or adjust based on feedback.
10. **File Paths:** When referring to files or folders, use the established project structure (e.g., `Assets/_Project/Scripts/Core/InputManager.cs`).
11. **Documentation Focus:** Aim for clarity that would allow another developer (or future self) to understand the script's role and usage.

## Starting a New Development Session (Especially After a Break):

This section outlines the procedure when resuming development.

1.  **Developer (Human) Initiates:**

    -   Clearly states they are starting a new session (e.g., "Ready to continue with Cosmic Harmonics," "Let's pick up where we left off.").
    -   **Provides Context (Recommended):**
        -   Mentions the last major step completed, possibly by referring to the latest entry in `IMPLEMENTATION_LOG.md`.
        -   Indicates the next planned step or feature based on the `TECHNICAL_BLUEPRINT.md` (e.g., "We finished implementing the basic Gravity Well. According to the plan, next is the Stability Zone.").
    -   If unsure about the exact next step, can ask for a reminder based on the blueprint or log.

2.  **AI Assistant Responds:**
    -   Acknowledges the start of the new session.
    -   **Confirms Context:**
        -   If context was provided by the human, the AI confirms its understanding (e.g., "Great! So we're moving on to Stability Zones after completing the Gravity Well.").
        -   If context is unclear or missing, the AI may ask for clarification by referencing existing documentation:
            -   "Welcome back! Could you remind me of the last item logged in `IMPLEMENTATION_LOG.md`?"
            -   "To make sure we're aligned, what was the last feature phase we were working on from the `TECHNICAL_BLUEPRINT.MD`?"
    -   Once context is established, the AI confirms the next goal and proceeds with providing guidance as per the "General Principles" and "Ongoing Workflow" sections.

## Ongoing Workflow (During an Active Session):

1.  Developer (Human) states the next goal or asks for guidance on a specific part of the blueprint.
2.  AI Assistant provides the tutorial/guide according to the "General Principles" above.
3.  Developer implements the steps.
4.  Developer provides feedback, console logs, asks clarification questions, and notes any deviations from the provided guide.
5.  AI Assistant responds, troubleshoots, or clarifies.
6.  Once the step/feature is complete and verified by the Developer, the Developer will request the `IMPLEMENTATION_LOG.md` update.
7.  AI Assistant provides the pre-formatted Markdown snippet for `IMPLEMENTATION_LOG.md`, incorporating any discussed deviations.
8.  Developer updates the `IMPLEMENTATION_LOG.md`.
9.  **Script Documentation:**
    -   Periodically (e.g., at the end of a development session covering multiple scripts, or when a significant script is finalized), the Developer may request an update or new entry for `SCRIPT_DOCUMENTATION.md`.
    -   The Developer will provide the final version(s) of the relevant script(s).
    -   The AI Assistant will generate the documentation entry for each provided script, detailing its purpose, public members, key methods, and dependencies, formatted for clarity.
10. Developer updates `SCRIPT_DOCUMENTATION.md`.
11. Developer commits all documentation changes along with code changes to Git.
12. Repeat for the next feature/task (returning to step 1 of "Ongoing Workflow").

## Example: Starting a New Session

**Scenario 1 (Human provides context):**
_Human:_ "Hi! Ready to continue with Cosmic Harmonics. Last time, we finished the basic Gravity Well (Star) implementation, and it's logged. According to the blueprint, Phase 1, Step 5 is the Stability Zone. Let's start with that."
_AI:_ "Welcome back! Sounds good. We'll start on the Stability Zone (Phase 1, Step 5), building on the Gravity Well. Let's begin by outlining the goals for the Stability Zone..."

**Scenario 2 (Human is less specific):**
_Human:_ "Okay, I'm back to work on Cosmic Harmonics."
_AI:_ "Welcome back! Great to continue. Could you remind me what the last step we completed in the `IMPLEMENTATION_LOG.md` was, or which part of the `TECHNICAL_BLUEPRINT.md` we're focusing on next?"
_Human:_ "Ah, right. Let me check... Okay, last log entry was '8. Gravity Well (Star - Basic) Implementation'. So, next should be Stability Zones."
_AI:_ "Perfect, thanks! So we're on to Stability Zones (Phase 1, Step 5). Let's dive in..."

## Example: During an Active Session (Requesting Guidance & Documentation)

_Human:_ "Okay, I've completed the project setup. Let's move on to implementing the Input System for flicking as per Phase 1."
_AI:_ (Follows guide to generate tutorial for Input System setup...)
... (Implementation and discussion) ...
_Human:_ "Alright, the Input System is working as expected. I placed the `PlayerControls.inputactions` and the generated `PlayerControls.cs` in `Assets/_Project/Scripts/Core/Input/`. Can you provide the log entry for this?"
_AI:_ (Generates the `IMPLEMENTATION_LOG.md` entry.)
_Human:_ (Later, after a few more scripts are done) "I'm taking a break. Here are the final versions of `InputManager.cs`, `SeedController.cs`, etc. Can you generate the entries for `SCRIPT_DOCUMENTATION.md`?"
_AI:_ (Generates the script documentation entries.)

---
