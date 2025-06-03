## Cosmic Harmonics: Technical Blueprint

---

### 1. Product Design Requirements (PDR)

-   **Game Concept & Vision:**
    "Cosmic Harmonics" is a 2D physics-based puzzle game where players flick "celestial seeds" to arrange them into stable configurations around "gravity wells." A core dynamic involves certain seeds transforming into new, smaller gravity wells upon stabilization, fundamentally altering the level's physics puzzle for subsequent actions. The vision is to create a contemplative, aesthetically pleasing experience that rewards creativity and understanding of emergent physics interactions, culminating in a rich sensory "bloom."
-   **Target Audience & Player Demographic:**
    Players who enjoy contemplative puzzle games with a creative or constructive element, aesthetic experiences, and a gentle learning curve. This includes fans of games like "World of Goo," "Flower," "Eufloria," and "Mini Metro," but with more direct physics manipulation. The target is casual to mid-core players, likely interested in indie games. Age group: Teens to Adults (13+).
-   **Core Gameplay Mechanics & Key Systems:**
    -   **Mechanics:**
        -   **Flicking Seeds:** Mouse click-drag-release (or touch equivalent) to launch seeds with controlled velocity.
        -   **Gravity Interaction:** Seeds are affected by fixed "Star" gravity wells and player-created "Seed Wells."
        -   **Stabilization:** Seeds become stable in designated "Stability Zones" by remaining quasi-stationary for a set duration.
        -   **Transformation:** Specific seeds transform into new gravity wells ("Seed Wells") upon stabilization, altering the physics landscape.
        -   **Level Completion:** Achieving a stable configuration of all designated seeds.
    -   **Key Systems (C# Scripts/Components):**
        -   `InputManager`: Handles player input for flicking.
        -   `SeedController`: Manages individual seed behavior, physics, state (Idle, Flying, Stabilizing, Stable, Transformed), and transformation logic.
        -   `GravityWell_Star`: Defines behavior of pre-placed, fixed gravity wells.
        -   `GravityWell_Seed` (part of `SeedController`'s transformed state): Defines behavior of player-created gravity wells.
        -   `StabilityZone`: Detects seeds and manages the stabilization process.
        -   `LevelManager`: Orchestrates level loading, seed dispensing, win condition checking, and the "Bloom" sequence.
        -   `UIManager`: Manages HUD, menus, and visual feedback.
        -   `AudioManager`: Controls music and sound effects, including the dynamic "Bloom" audio.
        -   `FXManager` (Optional, can be integrated into other managers initially): Manages visual effects like particles for blooms and trails.
-   **Unique Selling Propositions (USPs):**
    -   **Dynamic Puzzle Solving:** Player actions directly and predictably change the physics landscape, requiring strategic thinking about the order and placement of transformations.
    -   **Emergent Harmony & Sensory Reward:** Focus on achieving a stable, visually and auditorily harmonious state, culminating in a rich, multi-sensory "bloom" effect.
    -   **Aesthetic Appeal:** A "Cosmic Gardener/Weaver" theme with serene, abstract, beautiful visuals (glowing particles, soft blooms, flowing trails) and generative/evolving audio.
-   **Functional Requirements:**
    -   Player can launch seeds using a mouse click-drag-release action.
    -   Launched seeds move according to 2D physics principles, affected by gravity.
    -   "Star" gravity wells exert a constant gravitational pull on seeds.
    -   Seeds entering a "Stability Zone" and remaining near-stationary for a defined period become "Stable."
    -   "Transformable Seeds," upon becoming "Stable," convert into new, smaller "Seed Wells."
    -   "Seed Wells" exert gravitational pull according to their defined properties (e.g., strength, range, affected seed types).
    -   The game indicates a seed's current state (e.g., Flying, Stabilizing, Stable, Transformed) visually and audibly.
    -   The game provides clear feedback for seed stabilization progress.
    -   Levels are completed when all required seeds are in a stable configuration.
    -   Upon level completion, a "Bloom" visual and audio sequence plays.
    -   Players can restart the current level.
    -   Players can navigate a main menu with options like "Play," "Level Select," "Options," and "Exit."
    -   Game progress (unlocked levels) is saved.
    -   Basic audio and visual settings can be configured by the player.
-   **Non‑Functional Requirements:**
    -   Flicking controls must feel intuitive, responsive, and predictable.
    -   Physics interactions should be consistent and understandable.
    -   The game must maintain a smooth frame rate (e.g., target 60 FPS) on target PC configurations.
    -   Load times for levels should be minimal (e.g., under 3-5 seconds).
    -   The visual style must be ethereal, serene, minimalist, and elegant, with clear communication of game elements.
    -   The audio design must be calming, generative, and provide satisfying, informative feedback without being jarring.
    -   The game must support keyboard and mouse input (primary focus).
    -   UI elements must be clear, readable, and non-intrusive.
    -   (Future) Potential for mobile: Controls must adapt well to touch input. UI must be scalable.
-   **Player Experience Goal:**
    To provide a deeply satisfying and contemplative puzzle experience where players feel like cosmic gardeners, gently guiding elements into harmony. The game aims to evoke feelings of creativity, insight ("aha!" moments), and tranquility, culminating in a powerful sensory reward that makes solving puzzles feel like composing a small piece of cosmic art.

---

### 2. Tech Stack

-   **Core Engine:** Unity 6 (6000.0.4f1).
-   **Programming Language:** C#.
-   **Render Pipeline:** Universal Render Pipeline (URP), configured for 2D. This allows for efficient 2D lighting, particle effects, and post-processing suitable for the "glowing," "bloom" aesthetic.
-   **Key Unity Packages:**
    -   **2D Tools:** 2D Sprite, 2D Tilemap Editor (if needed for complex static backgrounds, otherwise not essential), 2D Pixel Perfect (if a pixel art style is chosen, otherwise less critical for abstract vector style), 2D Animation, 2D PSD Importer.
    -   **Input:** Input System (for robust mouse/gamepad/touch input).
    -   **Camera:** Cinemachine (for smooth camera movements, focus shifts during "Bloom," or potential subtle parallax effects).
    -   **UI Text:** TextMesh Pro (for sharp, scalable text).
    -   **Effects:** Post-Processing Stack (via URP, for bloom, vignette, color grading to enhance the cosmic aesthetic).
    -   **Physics:** Unity's 2D Physics (Rigidbody2D, CircleCollider2D, PointEffector2D, PhysicsMaterial2D).
-   **External Libraries & Plugins (if any):**
    -   **Tweening:** DOTween (free version) or LeanTween. Highly recommended for UI animations, smooth transitions, and programmatic animation of game elements (e.g., "Bloom" sequence details).
    -   **Serialization:** For save games, standard C# classes with `JsonUtility` or a simple JSON library like Newtonsoft [Json.NET](http://json.net/) (via Unity Package Manager or direct import) if more flexibility than `JsonUtility` is needed. Odin Inspector is overkill for this scope.
    -   **Analytics:** Unity Analytics (for basic tracking, if desired). Ensure API keys are handled securely and not exposed in client code (Security Checklist Point 3).
    -   **Networking/Backend Services:** Not applicable for the initial PC release.
-   **Version Control:** Git, with Git Large File Storage (LFS) for binary assets (textures, audio).
-   **Asset Creation & Management Tools:**
    -   **Art:**
        -   Vector: Inkscape (free), Affinity Designer (paid).
        -   Raster/Pixel: Aseprite (paid, pixel art), Krita (free, general painting/raster), Photoshop (paid).
        -   Particle Effects: Unity's built-in particle system (Shuriken).
    -   **Audio:**
        -   DAW: Reaper (paid, affordable), Audacity (free, basic).
        -   SFX Generation: Bfxr (free, web-based), LabChirp (free).
        -   Music: Bosca Ceoil (free, simple), or royalty-free libraries.
    -   **Level Design:** Primarily within Unity Editor. ScriptableObjects for level data.
-   **Target Platforms:**
    -   **Primary:** PC (Windows, macOS, Linux).
    -   **Potential Future:** Mobile (iOS, Android).
-   **Rationale for Choices:**
    -   **Unity:** Chosen for its mature 2D toolset, extensive documentation, large community, cross-platform capabilities, and suitability for physics-based games. The free Personal Edition is ideal for a solo beginner.
    -   **C#:** Native scripting language for Unity, powerful and widely used.
    -   **URP:** Provides modern rendering features, performance benefits, and tools like Shader Graph (optional, for custom visual effects) and robust post-processing necessary for the desired ethereal aesthetic (blooms, glows).
    -   **Input System:** Offers a more flexible and powerful way to handle inputs compared to the old Input Manager, crucial for future-proofing for multiple control schemes.
    -   **Cinemachine:** Simplifies complex camera behaviors and adds polish.
    -   **TextMesh Pro:** Standard for high-quality text rendering in Unity.
    -   **DOTween/LeanTween:** Drastically simplifies animation tasks, saving development time and improving game feel.
    -   **Git & LFS:** Industry-standard version control, essential for managing code and large assets.
    -   **ScriptableObjects for Level Data:** Decouples level design from scenes, making management easier and reducing merge conflicts.

---

### 3. Game Flow & Architecture Diagram

-   **High‑Level Game Flowchart:**

    ```mermaid
    graph TD
        A[Start Game] --> B{Splash Screens};
        B --> C[Main Menu];
        C -- Play --> D{Level Select Screen / First Unplayed Level};
        C -- Level Select --> D;
        C -- Options --> E[Options Menu];
        E -- Back --> C;
        C -- Exit --> F[Quit Game];

        D -- Select Level / Start --> G[Load Level];
        G --> H[Gameplay Screen - Core Loop];
        H -- Flick Seed --> I[Seed Physics Interaction];
        I -- Seed Enters Zone --> J[Seed Stabilization Process];
        J -- Stabilized --> K{Check All Seeds Stable?};
        K -- No --> H;
        K -- Yes --> L[Win Condition Met: Play "Bloom" Sequence];
        L --> M[Level Complete Screen];
        M -- Next Level --> D;
        M -- Replay Level --> G;
        M -- Main Menu --> C;

        H -- Pause Button --> N[Pause Menu];
        N -- Resume --> H;
        N -- Restart Level --> G;
        N -- Options --> O[Options Menu (In-Game)];
        O -- Back --> N;
        N -- Main Menu --> C;

    ```

-   **System Architecture Diagram:**
    -   **Key Systems/Managers:**
        -   `GameManager` (or `GameStateManager`): Overall game state (Menu, Playing, Paused), scene loading, persistent data.
        -   `InputManager`: Captures raw mouse/touch input, translates to world space, fires events for flick actions.
        -   `LevelManager`:
            -   Loads level data (from ScriptableObjects).
            -   Manages seed dispenser/available seeds.
            -   Tracks stability of all required seeds for the current level.
            -   Checks win conditions.
            -   Initiates the "Bloom" sequence visuals and audio triggers.
            -   Handles transitions (next level, retry).
        -   `SeedController` (Prefab Component):
            -   Handles its own Rigidbody2D, Collider2D.
            -   Contains properties (mass, type).
            -   Internal State Machine (Idle, Flying, Stabilizing, Stable, Transformed).
            -   Responds to physics, collisions, and triggers (from `StabilityZone`).
            -   Contains logic for behavior when transformed (acting as a `GravityWell_Seed`).
        -   `GravityWell_Star` (Prefab Component or Scene Object Component): Manages fixed gravity points (can use PointEffector2D or custom script).
        -   `StabilityZone` (Prefab Component or Scene Object Component): Trigger collider with script to detect `SeedController` entry/stay, manage stabilization timer, communicate status to `SeedController` and potentially `LevelManager`.
        -   `UIManager`:
            -   Displays HUD elements (e.g., seed availability, restart button).
            -   Manages Main Menu, Pause Menu, Options Menu, Level Select Screen, Level Complete Screen.
            -   Shows feedback (e.g., stabilization progress indicators if UI-based).
        -   `AudioManager`:
            -   Plays background music (potentially evolving).
            -   Plays SFX for flicking, collisions, stabilization, transformation, UI interactions.
            -   Orchestrates layered audio for the "Bloom" effect.
        -   `FXManager` (Optional, can start integrated): Manages pooling and playback of particle effects (trails, stabilization glows, "Bloom" particles).
        -   `SaveLoadManager`: Handles saving/loading of player progress (unlocked levels, settings).
    -   **Data Flow Example (Flicking a Seed):**
        1. `InputManager` detects click-drag-release -> Calculates flick vector.
        2. `InputManager` (or `LevelManager`) tells `SeedDispenser` to spawn/launch a `Seed`.
        3. `SeedController` on the new seed receives initial velocity.
        4. `SeedController`'s `FixedUpdate` allows Unity Physics to move it.
        5. `GravityWell_Star`s (and transformed `SeedWell`s) apply forces to `SeedController`'s Rigidbody2D in their `FixedUpdate`.
        6. `SeedController` collides with `StabilityZone` -> `OnTriggerEnter/Stay` on `SeedController` (or `StabilityZone`).
        7. `SeedController` checks its velocity; if low enough for long enough, updates its state to `Stabilizing`, then `Stable`.
        8. If `SeedController` is a Transformable Seed and becomes `Stable`, it changes its state to `Transformed` and activates its `GravityWell_Seed` behavior.
        9. `SeedController` (or `StabilityZone`) notifies `LevelManager` of stabilization/transformation.
        10. `LevelManager` checks win conditions.
        11. `UIManager` and `AudioManager` reflect state changes (e.g., visual cues for stabilization, sounds).
    -   **Scene Management Strategy:**
        -   **Boot/Persistent Scene (Optional but Recommended):** A small initial scene that loads essential managers (`GameManager`, `AudioManager`, `SaveLoadManager`) marked as `DontDestroyOnLoad`. This scene then loads the Main Menu.
        -   **Main Menu Scene:** Contains UI for menu navigation.
        -   **Gameplay Scene:** A generic scene that `LevelManager` populates with level-specific data from ScriptableObjects. Alternatively, distinct scenes per level if layouts are very unique and not easily proceduralized (though ScriptableObjects are preferred for data).
        -   Additive loading can be used for smooth transitions between Menu and Gameplay, or between levels, but direct scene loading is simpler to start.

---

### 4. Project Rules

-   **Development Best Practices & Coding Standards:**

    -   **C# Naming Conventions:**
        -   `PascalCase` for classes, enums, methods, properties, events, public fields.
        -   `camelCase` for local variables, private/protected fields.
        -   Prefix interfaces with `I` (e.g., `IGravitySource`).
        -   Constants in `ALL_CAPS_SNAKE_CASE`.
    -   **Unity-Specific Patterns:**
        -   Cache `GetComponent<T>()` results in `Awake()` or `Start()`. Avoid in `Update()`, `FixedUpdate()`.
        -   Avoid `GameObject.Find()` and `FindObjectOfType()` in frequently called methods; use direct references (assigned in Inspector or via code) or manager lookups.
        -   Prefer `CompareTag()` over `gameObject.tag == "TagName"`.
        -   Reset/initialize state in `OnEnable()` if object pooling is used, clean up in `OnDisable()`.
    -   **ScriptableObject Usage:** Extensively use ScriptableObjects for level configurations, seed definitions, global settings, event channels.
    -   **Scene Hierarchy:** Use empty GameObjects as folders for organization (e.g., `_Managers`, `_Environment`, `_DynamicObjects`, `_UI`). Name objects clearly and consistently.
    -   **Prefab Organization:** Maintain a clear structure for prefabs. Name them descriptively (e.g., `Seed_Transformable_TypeA_Prefab`, `FX_StabilizationPulse_Prefab`).
    -   **Folder Structure (within `Assets`):**

        ```
        Assets/
        ├── _Project/
        │   ├── Animations/
        │   ├── Audio/
        │   │   ├── Music/
        │   │   └── SFX/
        │   ├── Fonts/
        │   ├── Materials/
        │   ├── Prefabs/
        │   │   ├── Characters/ (Seeds)
        │   │   ├── Environment/ (Stars, Zones)
        │   │   ├── FX/
        │   │   └── UI/
        │   ├── Scenes/
        │   │   ├── _Main/
        │   │   └── _Levels/
        │   ├── ScriptableObjects/
        │   │   ├── Levels/
        │   │   ├── SeedTypes/
        │   │   └── EventChannels/ (Optional)
        │   ├── Scripts/
        │   │   ├── Core/
        │   │   ├── Gameplay/
        │   │   ├── Editor/ (Custom inspectors)
        │   │   └── UI/
        │   ├── Shaders/ (If custom shaders are made)
        │   └── Textures/ (or Sprites/)
        │       ├── UI/
        │       └── Gameplay/
        ├── Editor/ (Unity Editor scripts, if any from Asset Store)
        ├── Plugins/ (Third-party libraries)
        └── StreamingAssets/ (For data needed at runtime that shouldn't be in Resources)

        ```

-   **Version Control & Branching Strategy:**

    -   **Git + LFS:** Mandatory. Initialize LFS for common binary file types (`.png`, `.jpg`, `.wav`, `.mp3`, `.asset`).
    -   **Branching Model:** For a solo developer, a simplified model:
        -   `main` (or `master`): Always stable, releasable state.
        -   `develop`: Main integration branch for ongoing development.
        -   `feature/<feature-name>`: Branched from `develop` for new features or significant changes (e.g., `feature/transform-mechanic`, `feature/level-editor`). Merge back into `develop` when complete.
        -   Hotfix branches from `main` if urgent fixes are needed post-release.
    -   **Commit Message Conventions:**
        -   Start with a type (e.g., `feat:`, `fix:`, `docs:`, `style:`, `refactor:`, `perf:`, `test:`, `chore:`).
        -   Concise subject line (e.g., `feat: Implement seed transformation logic`).
        -   Optional body for more details.
        -   Commit frequently with small, logical changes.
    -   **.gitignore Setup:** Crucial for Unity projects. (Security Checklist Point 4)

        ```
        # Unity specific
        /[Ll]ibrary/
        /[Tt]emp/
        /[Oo]bj/
        /[Bb]uild/
        /[Bb]uilds/
        /[Ll]ogs/
        /[Uu]ser[Ss]ettings/
        /[Mm]emoryCaptures/
        /[Rr]ecordings/
        /[Aa]ssets/AssetStoreTools*

        # MemoryProfiler build
        /[Ll]ibrary/MemoryProfiler/

        # Visual Studio / Rider
        .vs/
        *.suo
        *.user
        *.userprefs
        *.sln.docstates
        *.csproj
        *.unityproj
        *.sln

        # Rider
        .idea/

        # Autogenerated VS/MD/Consulo solution and project files
        ExportedObj/
        .consulo/
        *.csproj.user
        *.pidb
        *.svd
        *.pdb
        *.mdb
        *.opendb
        *.VC.db

        # Unity3D generated meta files
        *.pidb.meta
        *.pdb.meta
        *.mdb.meta

        # Builds
        *.apk
        *.unitypackage

        # Crashlytics
        crashlytics-build.properties

        ```

-   **Testing & QA Strategy:**
    -   **Unit Testing:** Use Unity Test Runner for critical, non-MonoBehaviour logic (e.g., physics calculations if custom, data validation).
    -   **Playtesting:**
        -   **Self-testing:** Daily during development.
        -   **Peer testing:** Regularly share builds with friends or other developers for feedback.
        -   **Target Audience Testing (Later):** If possible, get feedback from players who fit the target demographic.
    -   **Bug Tracking:** Use a simple tool: Trello board, GitHub Issues, or even a shared spreadsheet. Columns: To Do, In Progress, To Test, Done. Bugs: Description, Steps to Reproduce, Severity, Screenshot/GIF.
    -   **QA Checklist for Builds:**
        -   Core mechanics functional (flicking, gravity, stabilization, transformation).
        -   All current levels playable and winnable.
        -   UI navigable and responsive.
        -   No major visual/audio glitches.
        -   Save/Load works correctly.
        -   Game starts and exits cleanly.
        -   Performance meets target on test hardware.
-   **Documentation Standards:**
    -   **This Technical Blueprint:** Keep updated as design evolves.
    -   **Code Comments:**
        -   XML-doc summaries (`/// <summary>...`) for all public classes, methods, and properties.
        -   Clear comments for complex or non-obvious logic blocks.
        -   Comment _why_, not _what_ if the code is self-explanatory.
    -   **Project Wiki (Optional but good for solo dev memory):** Notion, GitHub Wiki, or even a shared Google Doc for design notes, ideas, research, level design principles, feedback log.
    -   **Level Design Documentation:** For each level, a brief note on its goal, mechanics introduced/tested, and intended solution path or player insight. Can be comments in the ScriptableObject or a separate doc.
-   **Performance Optimisation Guidelines:**
    -   **Targets:** Aim for 60 FPS on mid-range PC target specs. Define target memory usage later.
    -   **Profiling:** Regularly use Unity Profiler (CPU, GPU, Memory) to identify bottlenecks. Especially after adding new features or complex levels.
    -   **Sprites & Textures:**
        -   Use Sprite Atlases for UI and frequently used game sprites to reduce draw calls.
        -   Appropriate texture compression (e.g., ASTC for quality/size on mobile, DXT5/BC7 on PC). For the abstract style, vector graphics might be converted to high-res sprites; ensure sensible import settings.
        -   Disable Mipmaps for 2D sprites that don't scale significantly (especially pixel art).
    -   **Physics:** Keep colliders simple (CircleColliders are good). Avoid complex MeshColliders for dynamic objects. Tune Physics 2D settings (Project Settings > Physics 2D) like Fixed Timestep, Velocity/Position Iterations if needed.
    -   **Object Pooling:** Consider for frequently created/destroyed objects like particle effects or potentially seeds if levels involve many rapid retries/failures. Implement only if profiling shows GC spikes.
-   **Accessibility Guidelines (Initial Considerations):**
    -   **Visuals:** As per PDR 6.3, design with potential colorblindness in mind. Use shape, pattern, or value differences in addition to color for critical information (e.g., seed types, zone affiliations).
    -   **Subtitles/Text:** Ensure all text is clear and readable (TextMesh Pro helps).
    -   **Controls:** For PC, ensure mouse-only play is viable. (Future: remappable keys if keyboard controls are added).
    -   **UI Scaling:** Consider options for UI scaling if text/elements are too small for some users/resolutions.

---

### 5. Implementation Plan

-   **Project Phases & Milestones:**
    1. **Phase 1: Prototyping (MVP - ~2-4 weeks for solo beginner)**
        - **Goal:** Prove core loop is functional and feels okay.
        - **Milestone:** Functional seed flicking, basic gravity, one seed type, one stability zone, simple stabilization logic, win condition, "level complete" message.
    2. **Phase 2: Core Mechanic Implementation & Vertical Slice (~4-6 weeks)**
        - **Goal:** Implement key "Transformation" mechanic and create 1-2 polished levels showcasing it, including a first-pass "Bloom" effect.
        - **Milestone:** Transformable seed converts to gravity well. Basic LevelManager loads levels (ScriptableObjects). Rudimentary "Bloom." Initial audio pass.
    3. **Phase 3: Alpha - Feature & Content Expansion (~8-12 weeks)**
        - **Goal:** All core game systems implemented. Core set of levels (e.g., 10-15) designed and playable. Main menu, options, save/load fully functional.
        - **Milestone:** Feature complete. All planned seed types/interactions present. First full pass on UI/UX. Art and audio more refined.
    4. **Phase 4: Beta - Content Complete & Polish (~6-8 weeks)**
        - **Goal:** All planned content (target 15-25 levels) complete. Focus on bug fixing, extensive playtesting, performance optimization, and overall polish.
        - **Milestone:** Content complete. Game is stable. Balancing and difficulty curve tuning. Accessibility features considered.
    5. **Phase 5: Gold Master (~1-2 weeks)**
        - **Goal:** Final release candidate. Final testing and bug fixes. Build preparation for target platforms.
        - **Milestone:** Game is ready for release.
    6. **Phase 6: Post-Launch (Ongoing)**
        - Monitor feedback, fix critical bugs.
        - Potential for updates (new levels, mechanics) if successful.
-   **Step‑by‑Step Task Breakdown (Example for Phase 1 - Prototyping/MVP):**
    1. **Project Setup:**
        - Create new Unity project (URP template).
        - Initialize Git repository, set up LFS, create `.gitignore`.
        - Basic folder structure.
    2. **Input System:**
        - Set up Input Actions for "Flick" (Start, Drag, Release positions).
        - Create `InputManager` script to read actions and convert to world coordinates.
    3. **Seed Implementation (Basic):**
        - Create Seed prefab (simple sprite, Rigidbody2D, CircleCollider2D).
        - `SeedController` script: basic movement logic (apply force from flick).
    4. **Gravity Well (Star - Basic):**
        - Create Star prefab (sprite, CircleCollider2D - trigger if custom gravity, or use PointEffector2D).
        - `GravityWell_Star` script: apply radial force to seeds within range in `FixedUpdate` (or configure PointEffector2D).
    5. **Stability Zone (Basic):**
        - Create Zone prefab (sprite, trigger Collider2D).
        - `StabilityZone` script: `OnTriggerEnter/Stay/Exit` to detect seeds.
        - `SeedController` modification: Logic to detect "quasi-stationary" (velocity below threshold) within a zone for a timer.
    6. **Level Logic (Basic):**
        - `LevelManager` script (rudimentary): Hardcode positions for 1 Star, 1 Seed launch point, 1 Zone.
        - Track if the single seed becomes stable.
        - Display "Level Complete!" debug message or simple UI text.
    7. **Basic UI:**
        - Canvas with a "Restart" button.
    8. **Testing & Iteration:** Continuously test flick feel, gravity, stability. Debug physics.
-   **Task Management:** Use a simple tool like Trello (Kanban board: Backlog, To Do, In Progress, Testing, Done), Notion, or GitHub Projects. Break down milestones into smaller, manageable tasks.
-   **Estimated Timelines & Dependencies:** (Rough estimates for solo dev, can vary widely)
    -   Dependencies are sequential for the phases. Within phases, tasks like "Art Polish" depend on "Mechanic Implemented."
    -   Focus on getting a playable loop ASAP, then iterate.
-   **Build & Deployment Plan:**
    -   Regularly create standalone builds (Windows, Mac, Linux) for testing throughout development.
    -   Configure Player Settings (icon, company/product name, resolution options).
    -   For release, test builds thoroughly on all target platforms/OS versions.
    -   Distribute via platforms like Steam, [Itch.io](http://itch.io/), GOG. Each has its own build upload and configuration process.

---

### 6. Unity Scene, Prefab, and UI Architecture Guidelines

-   **Scene Composition:**
    -   **Strategy:** Use a "Manager" or "Boot" scene that loads first and contains persistent systems (`GameManager`, `AudioManager`, `SaveLoadManager` using `DontDestroyOnLoad`). This scene then loads the `MainMenu` scene.
    -   `MainMenu` scene: Contains all UI and logic for the main menu, level select, options.
    -   `Gameplay` scene: A single, generic scene. The `LevelManager` dynamically populates this scene with level elements (Stars, Seed Launchers, Zones) based on ScriptableObject data for the selected level. This minimizes scene count and makes level management data-driven.
    -   **Hierarchy:** In each scene, use empty GameObjects as folders (e.g., `// -- MANAGERS --`, `// -- UI --`, `// -- LEVEL GEO --`). Name GameObjects clearly (e.g., `PlayerSeedLauncher`, `GravityWell_MainStar`, `StabilityZone_Alpha`).
-   **Prefab Design:**
    -   **Modularity:** Create prefabs for all reusable entities:
        -   `Seed_Standard_Prefab`, `Seed_TransformableTypeA_Prefab`, etc.
        -   `GravityWell_Star_Prefab`.
        -   `StabilityZone_TypeA_Prefab`.
        -   UI elements: `Button_Standard_Prefab`, `Panel_Settings_Prefab`.
        -   Particle Effects: `FX_SeedTrail_Prefab`, `FX_StabilizationGlow_Prefab`, `FX_BloomBurst_Prefab`.
    -   **Scope:** Prefabs should be self-contained where possible. E.g., a `Seed_Prefab` contains its sprite, Rigidbody2D, Collider2D, `SeedController` script, and any child objects for visual effects.
    -   **Variants & Nested Prefabs:** Utilize Prefab Variants if you have many seeds that are similar but differ slightly (e.g., different mass, color, transformation effect). Nested prefabs can be used for complex entities, but keep hierarchies manageable.
    -   **Naming:** `[Category]_[SpecificName]_[Variant/State]_Prefab` (e.g., `Seed_Transformable_Blue_Prefab`). Inside prefabs, child GameObjects should also be clearly named (e.g., `Visuals`, `Collider`, `TrailEffect`).
-   **UI/UX Best Practices for Games:**
    -   **Canvas Setup:**
        -   Use `Screen Space - Overlay` for most UI unless diegetic UI integrated into the world is desired.
        -   `Canvas Scaler`: Set to `Scale With Screen Size`, define a reference resolution (e.g., 1920x1080). Adjust `Match` slider (0 for width, 1 for height, 0.5 for a mix) depending on which dimension is more critical for layout.
    -   **Anchors & Pivots:** Meticulously set anchors and pivots for all UI elements to ensure responsive scaling across different aspect ratios and resolutions. Use anchor presets effectively.
    -   **TextMesh Pro:** Use for all text. Choose clear, readable fonts that fit the "serene, elegant" aesthetic. Ensure good contrast with backgrounds. Implement a basic text hierarchy (titles, body text, captions).
    -   **Intuitive Navigation:** Menus should be simple and easy to navigate with mouse. For future gamepad support, ensure clear focus states and logical D-pad/stick navigation.
    -   **Feedback:** UI should provide immediate feedback for interactions (button clicks, hover states).
    -   **Minimalism:** Keep UI uncluttered. Only show necessary information. The game's aesthetic is minimalist.
-   **Component Architecture (MonoBehaviour & ScriptableObject):**
    -   **Single Responsibility Principle (SRP):** Each script (`MonoBehaviour`) should have one primary responsibility. E.g., `SeedController` handles seed logic, `InputManager` handles input, `UIManager` handles UI. Avoid massive "god" scripts.
    -   **Inter-Component Communication:**
        -   **Direct References:** For tightly coupled components (e.g., `SeedController` referencing its own `Rigidbody2D`). Assign via Inspector or `GetComponent` in `Awake()`.
        -   **UnityEvents:** Useful for Inspector-assignable event handling (e.g., UI button clicks calling a `LevelManager` method).
        -   **C# Events/Actions:** Good for decoupled communication between systems (e.g., `LevelManager` fires an `OnLevelComplete` event, `AudioManager` and `UIManager` subscribe).
        -   **ScriptableObject Events (Event Channels):** An advanced pattern for highly decoupled systems. A `GameEvent` ScriptableObject is invoked by one system and listened to by others. Good for larger projects, can be overkill initially but powerful.
        -   Avoid `SendMessage` due to performance and lack of type safety.
    -   **ScriptableObjects for Data:**
        -   **Level Definitions:** Each level is a `LevelData` ScriptableObject asset (positions/types of stars, seed types/quantities, zone configs, win conditions).
        -   **Seed Definitions:** `SeedData` ScriptableObjects (mass, sprite, transformation type, audio cues).
        -   **Game Settings:** Can store configurable game parameters (e.g., default physics values, difficulty modifiers).
-   **Asset & Rendering Guidelines for 2D:**
    -   **Sprite Atlasing:** Use Unity's Sprite Atlas feature to pack sprites used together (e.g., UI elements, animation frames for a seed) into single textures to reduce draw calls.
    -   **Texture Import Settings:**
        -   **Texture Type:** `Sprite (2D and UI)`.
        -   **Sprite Mode:** `Single` or `Multiple` (for sprite sheets).
        -   **Pixels Per Unit:** Consistent value across sprites that should be same relative size (e.g., 100).
        -   **Mesh Type:** `Tight` (usually best for non-rectangular sprites) or `Full Rect`.
        -   **Generate Physics Shape:** Enable if sprites need to drive collider shapes accurately.
        -   **Compression:** `Normal Quality`. For PC, Crunch compression might be too aggressive if fine gradients are key; test platform-specific formats like BC7. For abstract visuals, gradients are important, so high quality.
        -   **Read/Write Enabled:** Disable unless script access to texture data is needed (rare, impacts memory).
        -   **Mip Maps:** Generally disable for 2D sprites unless they scale significantly in the world or you notice aliasing at smaller sizes.
    -   **Materials and Shaders (URP):**
        -   Use URP's 2D Lit/Unlit shaders (`Sprite-Lit-Default`, `Sprite-Unlit-Default`).
        -   The "ethereal, glowing" aesthetic may benefit from `Sprite-Lit-Default` with 2D lights (global light, point lights for bloom effects) or custom shaders/VFX Graph for particle effects.
        -   Post-processing (Bloom, Tonemapping, Color Adjustments) will be key for the visual style.
    -   **Sorting Layers and Order in Layer:** Use Sorting Layers (`Background`, `Midground`, `Gameplay`, `Foreground`, `UI`) to manage 2D render order. Within a layer, use `Order in Layer` on Sprite Renderers.
    -   **2D Lighting (URP):** If using 2D lights, configure a 2D Renderer Data asset. Use `Light2D` components (Global, Spot, Point) to illuminate sprites. Normal maps can be added to sprites for more detailed lighting effects if desired, but might be overly complex for the abstract style.
-   **Performance Practices (Visuals & UI):**
    -   **Object Pooling:** Implement for `Seed` prefabs (if many are flicked and reset quickly) and especially for `Particle Effect` prefabs. Create a simple `ObjectPooler` class.
    -   **UI Optimization:**
        -   Minimize UI elements that need to update every frame.
        -   Use `CanvasGroup` to fade elements in/out (more performant than changing individual alpha values on many child elements).
        -   Disable `Raycast Target` on UI elements that are purely visual and don't need to intercept input (e.g., background images, static text).
        -   Separate canvases for static vs. dynamic UI elements if profiling shows UI batching issues.
    -   **Profile Rendering:** Use the Frame Debugger to analyze draw calls, batches, and rendering sequence. Check for overdraw.

---

### 7. Game Systems & Data Management Guidelines

-   **Core Game Systems Architecture (Client-Side):**
    -   **Save/Load System:**
        -   **Data to Save:** Unlocked levels, player settings (audio volume, visual preferences). Potentially best scores/times per level if added later.
        -   **Serialization Format:** JSON is recommended for readability and ease of use. Unity's `JsonUtility` is sufficient for simple data structures. For more complex needs, Newtonsoft [Json.NET](http://json.net/).
        -   **Save File Location:** Use `Application.persistentDataPath` for save files.
        -   **Naming:** E.g., `CosmicHarmonics_SaveData.json`, `CosmicHarmonics_Settings.json`.
        -   **Security:** Implement basic checksums (e.g., hash of save data + a salt) stored alongside the save file to detect trivial tampering. This is a deterrent, not foolproof security (Security Checklist Point 8). Avoid storing sensitive PII.
        -   **Error Handling:** Handle cases where save file is missing or corrupted (e.g., load default data).
    -   **Event Management System:**
        -   **Simple C# Events/Actions:** For many system communications. Example: `public static event Action<int> OnLevelCompleted;`
        -   **UnityEvents:** Good for Inspector-driven connections, especially for UI.
        -   **ScriptableObject Event Channels (Consider for scalability):** Create `GameEvent` ScriptableObject assets. Systems can raise these events, and other systems can subscribe to them without direct references. E.g., `SO_SeedStabilizedEvent`, `SO_LevelStartEvent`.
    -   **State Management:**
        -   **Game State:** A simple FSM in `GameManager` (e.g., `enum GameState { MainMenu, Playing, Paused, LevelComplete }`).
        -   **Seed State:** An FSM within `SeedController` (e.g., `enum SeedState { InLauncher, Flying, Stabilizing, Stable, Transformed }`) using enums and switch statements, as outlined in PDR 3.5.
        -   **Level State:** Managed by `LevelManager` (tracking seeds, win conditions).
-   **Data Storage & Configuration:**
    -   **PlayerPrefs:** For very simple user settings like volume, resolution, quality toggle. Easy to use but less flexible and less secure than files.
    -   **Serialized Files (JSON):** Primary method for save game data (player progress).
    -   **ScriptableObjects:** Primary means for design-time game data:
        -   `LevelDataSO`: Defines all parameters for a level (star positions/types, seed types/launch points, zone configurations, specific win conditions, bloom parameters).
        -   `SeedDataSO`: Defines properties for each seed type (mass, visual appearance, transformation capabilities, associated SFX, bloom contribution).
        -   `StarDataSO`: Defines properties for fixed star types (gravity strength, range, visual appearance).
        -   `GlobalGameConfigSO`: For game-wide balance values, physics constants, UI settings.
-   **Backend Services Integration (Not for initial scope, future considerations):**
    -   If online features like leaderboards, cloud saves, or user accounts were added:
        -   **Server Architecture & API Design:** Would likely use a BaaS (Backend-as-a-Service) like PlayFab, Firebase, or Nakama to simplify development. If custom, RESTful APIs over HTTPS.
        -   **Data Storage (Online):** Cloud databases provided by BaaS (e.g., Firestore, PlayFab Player Data).
        -   **Security (Online):** Critical. Use BaaS authentication. Validate all client input server-side. Server authoritative for scores/progress. (Security Checklist Points 1, 2, 6, 7, 10).
        -   **Scalability & Performance (Online):** BaaS solutions typically handle this.
-   **Integration with Third-Party Services:**
    -   **Analytics:** Unity Analytics is a good starting point.
        -   Track events like level start, level complete, level failed, specific mechanic usage.
        -   Ensure API keys are stored securely, ideally configured server-side or via build pipeline, not hardcoded in client scripts (Security Checklist Point 3). For Unity Analytics, this is usually handled by Unity project settings.
    -   **Platform Services (Future, for Steam/Mobile):** Steamworks SDK, Google Play Games Services, Apple Game Center for achievements, leaderboards, cloud saves. Integration would occur much later.

---

### 8. Optimised Unity C# & Performance Guidelines

-   **C# Best Practices in Unity:**

    -   **Caching References:**

        -   In `Awake()` or `Start()`, get and store references to frequently accessed components or GameObjects:

            ```csharp
            // Bad:
            // void Update() { GetComponent<Rigidbody2D>().velocity = Vector2.zero; }

            // Good:
            Rigidbody2D rb;
            void Awake() { rb = GetComponent<Rigidbody2D>(); }
            void Update() { if (rb != null) rb.velocity = Vector2.zero; }

            ```

        -   Avoid `Camera.main` in `Update()`; cache it if used repeatedly.

    -   **Minimize Allocations (Reduce Garbage Collection):**
        -   **Strings:** Avoid string concatenations (`+` or `string.Format`) in loops or `Update()`. Use `StringBuilder` if complex string manipulation is needed.
        -   **`new` keyword:** Avoid unnecessary `new List<T>()`, `new GameObject()`, etc., in `Update()` or other frequent calls. Use object pooling.
        -   **LINQ:** Be cautious with LINQ in performance-critical paths as it can allocate memory.
        -   **Boxing:** Avoid accidental boxing of value types (e.g., passing a struct to a method expecting `object`).
    -   **Structs vs. Classes:**
        -   Use `struct` for small, data-only types that don't need reference semantics or inheritance. Can help avoid heap allocations if used appropriately (e.g., passing by value, local variables).
        -   Be mindful of copying costs if structs are large.
    -   **Coroutines:**

        -   `yield return null;` (waits one frame, no allocation).
        -   `yield return new WaitForSeconds(duration);` allocates memory. For repeated identical waits, cache the `WaitForSeconds` object:

            ```csharp
            WaitForSeconds waitOneSecond;
            void Start() { waitOneSecond = new WaitForSeconds(1f); }
            IEnumerator MyCoroutine() { yield return waitOneSecond; }

            ```

        -   For frequent, short custom delays, consider manual timer in `Update()` instead of many short-lived coroutines.

    -   **Unity Event Functions (`Update`, `FixedUpdate`, `LateUpdate`, etc.):**
        -   Understand their execution order.
        -   Keep logic in them concise. Offload heavy computations to coroutines or event-driven updates.
        -   Empty Unity event functions (e.g., an empty `Update()`) have a small overhead; remove if not used.

-   **Common Unity Performance Pitfalls & Solutions:**

    -   **Physics (2D):**
        -   **Colliders:** Use simple primitive colliders (`CircleCollider2D`, `BoxCollider2D`) over `PolygonCollider2D` where possible. Avoid `MeshCollider` for dynamic 2D objects.
        -   **Rigidbody Settings:** Ensure `Rigidbody2D.SleepMode` is `StartAwake` (default) or `StartAsleep` to allow bodies to sleep when not moving, saving CPU.
        -   **Fixed Timestep:** (Project Settings > Time) Adjust carefully. Smaller timestep = more accuracy, more CPU. Larger = less accuracy, less CPU.
        -   **Layer Collision Matrix:** (Project Settings > Physics 2D) Configure layers so objects only interact if necessary (e.g., seeds don't need to collide with decorative background elements).
        -   **Raycasts/OverlapChecks:** Minimize frequency. Use non-allocating versions if available (`Physics2D.RaycastNonAlloc`).
    -   **Rendering:**
        -   **Draw Calls:** Profile with Frame Debugger and Stats window. Reduce via Sprite Atlasing, material sharing.
        -   **SetPass Calls:** Similar to draw calls, caused by material/shader changes.
        -   **Overdraw:** Where multiple transparent sprites cover each other. Minimize layers of transparency or use opaque shaders where possible. URP's 2D Renderer helps with sorting.
        -   **Static Batching:** For non-moving geometry, mark as static to allow engine batching.
    -   **Large MonoBehaviours:** Decompose scripts with many responsibilities into smaller, focused components.
    -   **Memory Management:**

        -   **Event Unsubscription:** Always unsubscribe from C# events in `OnDestroy()` or `OnDisable()` to prevent memory leaks from orphaned references.

            ```csharp
            void OnEnable() { SomeManager.OnSomeEvent += HandleSomeEvent; }
            void OnDisable() { SomeManager.OnSomeEvent -= HandleSomeEvent; }

            ```

        -   **Static Collections:** Be very careful with static lists/dictionaries. Ensure they are cleared or items are removed when no longer needed, as they persist for the application's lifetime.
        -   **Texture/Audio Import Settings:** Large uncompressed assets consume significant memory. Use appropriate compression.

-   **Profiling & Optimization Tools:**
    -   **Unity Profiler:** Essential. Profile CPU Usage (Deep Profile mode helps pinpoint expensive methods), GPU Usage, Memory (Simple and Detailed views), Rendering, Physics. Connect to builds for accurate device profiling.
    -   **Frame Debugger:** Analyzes individual draw calls, shader properties, render states. Helps identify why things aren't batching or why rendering is slow.
    -   **Platform-Specific Profilers:** (If issues persist on specific platforms) Xcode Instruments (iOS/macOS), Android Profiler (Android Studio), RenderDoc, PIX (Windows).
-   **Code Snippets (Examples):**

    -   **Problematic (GetComponent in Update):**

        ```csharp
        // Inefficient if called frequently
        void Update() {
            GetComponent<SpriteRenderer>().color = Color.red;
        }

        ```

    -   **Optimised (Cached Reference):**

        ```csharp
        SpriteRenderer spriteRenderer;
        void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        void Update() {
            if (spriteRenderer != null) {
                spriteRenderer.color = Color.red;
            }
        }

        ```

    -   **Problematic (String concatenation in Update):**

        ```csharp
        // Generates garbage every frame
        public int score;
        public TextMeshProUGUI scoreText;
        void Update() {
            scoreText.text = "Score: " + score;
        }

        ```

    -   **Optimised (Update text only when score changes, or use StringBuilder for complex cases):**

        ```csharp
        public int score;
        private int _previousScore = -1;
        public TextMeshProUGUI scoreText;
        void Update() {
            if (score != _previousScore) {
                scoreText.text = $"Score: {score}"; // Or use a pooled StringBuilder
                _previousScore = score;
            }
        }

        ```

-   **Structuring for Maintainability & Scalability:**
    -   **Namespaces:** Organize scripts into logical namespaces (e.g., `CosmicHarmonics.Core`, `CosmicHarmonics.Gameplay`, `CosmicHarmonics.UI`).
    -   **Interfaces:** Use interfaces to define contracts for behavior, promoting loose coupling (e.g., `IAffectableByGravity`, `ITransformable`).
    -   **Event-Driven Architecture:** Prefer events (C# Actions, UnityEvents, ScriptableObject Events) over direct method calls between disparate systems to reduce dependencies.
    -   **SOLID Principles:** Strive to apply SOLID principles where practical:
        -   **S**ingle Responsibility Principle (already mentioned).
        -   **O**pen/Closed Principle (extend behavior with new seed types/transformations without modifying old code, e.g., via strategy pattern or ScriptableObject configurations).
        -   **L**iskov Substitution Principle (subtypes should be substitutable for their base types).
        -   **I**nterface Segregation Principle (prefer smaller, specific interfaces).
        -   **D**ependency Inversion Principle (depend on abstractions, not concretions; use interfaces or events).

---

### 9. Security Checklist

This checklist MUST be enforced across relevant systems. Points relevant to "Cosmic Harmonics" in its current PC-first, offline-focused scope are primarily client-side. Online features would necessitate broader application.

1. **For Online Features: Use battle‑tested auth libraries/services.** Never hand‑roll auth. Prefer platform solutions (Steam, PlayFab, Firebase Auth, Nakama) for user accounts, sessions, etc.
   _(N/A for initial scope, but critical if online features are added.)_
2. **For Online Features: Lock down protected server endpoints.** Every server request handling sensitive data or actions must be authenticated and authorized.
   _(N/A for initial scope.)_
3. **Never expose secrets in client code.** API keys, server credentials, and sensitive tokens must not be hardcoded in the game client. Store on a secure server or use mechanisms like OAuth device flow where appropriate for client-side keys if absolutely unavoidable and the service is designed for it (e.g., some analytics).
   _(Relevant for Unity Analytics or any third-party service keys. These are typically configured in Unity project settings or via build scripts, not directly in version-controlled C# files.)_
4. **Git‑ignore sensitive files and Unity-specific generated files.** Always add `.env` (if used for local dev secrets), `[Ll]ibrary/`, `[Tt]emp/`, `[Oo]bj/`, `[Bb]uild/`, `[Bb]uilds/`, `[Ll]ogs/`, `[Uu]ser[Ss]ettings/` to `.gitignore`.
   _(Implemented in Section 4: Project Rules.)_
5. **Sanitise error messages.**
    - **Online:** Never reveal internal server logic/stack traces to clients; return generic, friendly errors. _(N/A for initial scope.)_
    - **Client:** Display user-friendly error messages to players. Log detailed technical errors for developers only (e.g., to a local file or developer console, not shown to player).
6. **For Online Features: Enforce Server Authority & Validation.** The server must be authoritative for game state, especially in competitive or monetized games. Validate all client inputs rigorously. Do not trust the client.
   _(N/A for initial scope.)_
7. **For Online Features: Add Role‑Based Access Control (RBAC) if applicable.** If your game's backend has different user roles (admin, moderator, player), define and enforce permissions server-side.
   _(N/A for initial scope.)_
8. **Secure Data Handling:**
    - **Offline Save Files:** Implement checksums or basic obfuscation to deter casual save file editing. Understand this is not foolproof but a deterrent. Avoid storing sensitive Personal Identifiable Information (PII) unsecured.
      _(Relevant for game progress saves. Covered in Section 7: Game Systems & Data Management Guidelines.)_
    - **Online Databases:** Use secure DB solutions (e.g., Supabase, Firebase) and configure their security rules (Row Level Security, etc.) correctly. _(N/A for initial scope.)_
9. **Host online services on secure platforms.** Choose reputable cloud providers (AWS, Azure, GCP) or Backend-as-a-Service (BaaS) for game servers, ensuring they offer DDoS protection, WAFs, and auto-patching.
   _(N/A for initial scope.)_
10. **Enable HTTPS everywhere.** All communication from the game client to any web service (APIs, analytics, remote config) must use HTTPS/TLS. Unity's `UnityWebRequest` supports this.
    _(Relevant if using Unity Analytics or any other web service. Ensure all `UnityWebRequest` calls use `https://` URLs.)_
11. **Limit risks from User‑Generated Content (UGC) (if applicable).** Scan uploads if possible, validate file‑types, enforce size limits, and have a reporting system. Never trust UGC blindly.
    _(N/A for this game concept.)_
12. **Client-Side Anti-Cheat (for competitive online games - advanced topic).** Consider strategies like memory scanning detection, code integrity checks, and input anomaly detection. Often involves third-party solutions for larger games. For most indies, server-side authority is the primary defense.
    _(N/A for this non-competitive, primarily single-player game.)_
13. **Secure Asset Pipeline.** Ensure assets from third-party sources (stores, contractors) are scanned for malware and come from trusted vendors before integration.
    _(General good practice when acquiring assets from the Unity Asset Store or other marketplaces.)_

This blueprint provides a comprehensive technical foundation for developing "Cosmic Harmonics" in Unity. Remember that this is a living document and should be updated as the project evolves. Good luck!
