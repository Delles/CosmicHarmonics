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
6.  **Implementation Log Updates:** After a significant feature or setup phase is complete, provide a pre-formatted Markdown snippet to be added to `IMPLEMENTATION_LOG.md`. This snippet should summarize what was done.
7.  **Clarity on "Why":** Briefly explain the reasoning behind certain choices or steps, especially if there are alternatives, linking back to blueprint decisions if possible.
8.  **Assumptions:** State any assumptions being made (e.g., "Assuming Unity 2022.3.x LTS is installed").
9.  **Testing Steps:** Include simple steps or checks to verify that the implementation is working correctly.
10. **Iterative Approach:** Acknowledge that this is an iterative process. Encourage questions and be prepared to clarify or adjust based on feedback.
11. **File Paths:** When referring to files or folders, use the established project structure (e.g., `Assets/_Project/Scripts/Core/InputManager.cs`).

## Workflow:

1.  Developer (Human) states the next goal or asks for guidance on a specific part of the blueprint.
2.  AI Assistant provides the tutorial/guide according to the principles above.
3.  Developer implements the steps.
4.  Developer provides feedback, console logs, or asks clarification questions.
5.  AI Assistant responds, troubleshoots, or clarifies.
6.  Once the step/feature is complete and verified, AI Assistant provides the `IMPLEMENTATION_LOG.md` update.
7.  Developer updates the log and commits changes to Git.
8.  Repeat for the next feature/task.

## Example Interaction Start:

_Human:_ "Okay, I've completed the project setup. Let's move on to implementing the Input System for flicking as per Phase 1."
_AI:_ (Follows guide to generate tutorial for Input System setup...)

---
