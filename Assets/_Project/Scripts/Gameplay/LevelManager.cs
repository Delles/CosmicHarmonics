using UnityEngine;
using System.Collections.Generic; // For potential future use with multiple seeds
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private GameObject seedPrefab;
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private GameObject stabilityZonePrefab;

    [Header("Level Configuration")]
    [SerializeField] private Vector2 initialSeedPosition = new Vector2(0, -4f);
    [SerializeField] private Vector2 starSpawnPosition = new Vector2(3, 2);
    [SerializeField] private Vector2 stabilityZoneSpawnPosition = new Vector2(-3, 2);
    [SerializeField] private Vector2 stabilityZoneScale = Vector2.one;

    [Header("UI References")]
    [SerializeField] private GameplayUIController gameplayUIController; // Changed to SerializeField
    [SerializeField] private MainMenuController mainMenuController; // Add this

    [Header("Runtime References")]
    public SeedController _activeSeed { get; private set; } // Made property for clarity

    private bool _levelSetupComplete = false;
    private bool _levelCompleted = false;
    private Camera _mainCamera; // Cache the main camera

    // Keep track of spawned objects for cleanup if we reset to main menu
    private GameObject _spawnedStar;
    private GameObject _spawnedStabilityZone;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Consider if LevelManager persists across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("LevelManager: Main Camera not found! Ensure camera is tagged 'MainCamera'.", this);
        }

        // Remove the FindAnyObjectByType call from Awake for gameplayUIController
        if (gameplayUIController == null) // Check if it's assigned in Inspector
        {
            Debug.LogError("GameplayUIController is not assigned in the LevelManager Inspector slot.");
        }
        if (mainMenuController == null) // Check new field
        {
            Debug.LogError("MainMenuController is not assigned in the LevelManager Inspector slot.");
        }
    }

    // Changed from Start() to be called manually
    public void StartLevel()
    {
        Debug.Log("LevelManager: StartLevel called.");
        if (_levelSetupComplete) // If level is already setup, perhaps reset it first
        {
            ResetLevelInternally(); // A new method to clean up and restart
        }

        if (gameplayUIController != null) // Use the serialized field
        {
            gameplayUIController.ShowGameplayUI(); // Show the gameplay canvas
            gameplayUIController.HideLevelCompletePanel(); // Ensure panel is hidden
        }
        else
        {
            Debug.LogWarning("GameplayUIController not assigned in LevelManager Inspector. UI might not behave as expected.");
        }
        
        Time.timeScale = 1f; // Ensure game is running
        SetupLevel();

        if (InputManager.Instance != null)
        {
            InputManager.Instance.SetGamePlayingState(true);
        }
    }

    void Update()
    {
        if (!_levelSetupComplete || _activeSeed == null) return;

        if (_activeSeed.IsLaunched && !_activeSeed.IsStable && !_levelCompleted)
        {
            CheckSeedOutOfBounds();
        }

        // Win condition check can remain, or be tied to HandleSeedStabilized more directly
        CheckWinCondition();
    }

    void SetupLevel()
    {
        // Clear previous level objects if any
        ClearLevelObjects();

        Debug.Log("LevelManager: Setting up level.");

        // Spawn Star
        if (starPrefab != null)
        {
            _spawnedStar = Instantiate(starPrefab, starSpawnPosition, Quaternion.identity);
            _spawnedStar.name = "Spawned_Star";
            Transform envParent = GameObject.Find("// -- ENVIRONMENT --")?.transform;
            if (envParent != null)
                _spawnedStar.transform.SetParent(envParent);
            Debug.Log($"LevelManager: Spawned Star at {starSpawnPosition}");
        }
        else
        {
            Debug.LogError("LevelManager: Star Prefab not assigned!");
        }

        // Spawn Stability Zone
        if (stabilityZonePrefab != null)
        {
            _spawnedStabilityZone = Instantiate(stabilityZonePrefab, stabilityZoneSpawnPosition, Quaternion.identity);
            _spawnedStabilityZone.name = "Spawned_StabilityZone";
            _spawnedStabilityZone.transform.localScale = stabilityZoneScale;
            Transform envParent = GameObject.Find("// -- ENVIRONMENT --")?.transform;
            if (envParent != null)
                _spawnedStabilityZone.transform.SetParent(envParent);
            Debug.Log($"LevelManager: Spawned Stability Zone at {stabilityZoneSpawnPosition} with scale {stabilityZoneScale}");
        }
        else
        {
            Debug.LogError("LevelManager: Stability Zone Prefab not assigned!");
        }

        // Spawn Initial Seed
        SpawnSeed();

        _levelSetupComplete = true;
        _levelCompleted = false; // Ensure level isn't marked complete on setup/restart
        Debug.Log("LevelManager: Level setup complete. Active seed should be ready for aiming.");
    }

    private void ClearLevelObjects()
    {
        if (_activeSeed != null)
        {
            _activeSeed.OnSeedStabilized -= HandleSeedStabilized;
            _activeSeed.OnSeedLaunched -= HandleSeedLaunched;
            Destroy(_activeSeed.gameObject);
            _activeSeed = null;
        }
        if (_spawnedStar != null)
        {
            Destroy(_spawnedStar);
            _spawnedStar = null;
        }
        if (_spawnedStabilityZone != null)
        {
            Destroy(_spawnedStabilityZone);
            _spawnedStabilityZone = null;
        }
    }

    public void SpawnSeed()
    {
        if (seedPrefab != null)
        {
            // Clean up existing seed if any
            if (_activeSeed != null)
            {
                _activeSeed.OnSeedStabilized -= HandleSeedStabilized;
                _activeSeed.OnSeedLaunched -= HandleSeedLaunched;
                Destroy(_activeSeed.gameObject);
            }

            GameObject seedGO = Instantiate(seedPrefab, initialSeedPosition, Quaternion.identity);
            seedGO.name = "Spawned_ActiveSeed";
            Transform dynamicParent = GameObject.Find("// -- DYNAMIC_OBJECTS --")?.transform;
            if (dynamicParent != null)
                seedGO.transform.SetParent(dynamicParent);

            _activeSeed = seedGO.GetComponent<SeedController>();

            if (_activeSeed != null)
            {
                _activeSeed.PrepareForAiming(initialSeedPosition); // Tell the new seed to be kinematic and ready
                // Subscribe to the seed's stabilization event
                _activeSeed.OnSeedStabilized += HandleSeedStabilized;
                _activeSeed.OnSeedLaunched += HandleSeedLaunched; // New event
                Debug.Log($"LevelManager: Spawned Seed and prepared for aiming at {initialSeedPosition}");
            }
            else
            {
                Debug.LogError("LevelManager: Seed Prefab does not have a SeedController component!");
            }
        }
        else
        {
            Debug.LogError("LevelManager: Seed Prefab not assigned!");
        }
    }

    // NEW Method: Called by InputManager to update the aiming seed's position
    public void UpdateActiveSeedAimPosition(Vector2 mouseWorldPosition)
    {
        if (!_levelSetupComplete || _activeSeed == null || !_activeSeed.IsReadyForAiming) return;
        _activeSeed.UpdateAimPosition(mouseWorldPosition);
    }

    // Called by InputManager when a flick is performed
    public void RequestLaunchActiveSeed(Vector2 flickStartPosition, Vector2 flickEndPosition)
    {
        if (!_levelSetupComplete || _activeSeed == null || !_activeSeed.IsReadyForAiming || _activeSeed.IsLaunched || _activeSeed.IsStable)
        {
            Debug.LogWarning("LevelManager: Cannot launch seed: Not ready, already launched, or stable.");
            return;
        }

        Vector2 flickVector = flickEndPosition - flickStartPosition;
        _activeSeed.Launch(flickVector); // SeedController now handles making itself Dynamic
        Debug.Log($"LevelManager: Launch requested. Flick Start: {flickStartPosition}, Flick End: {flickEndPosition}, Flick Vector: {flickVector}");
    }

    private void HandleSeedLaunched(SeedController seed)
    {
        if (seed == _activeSeed)
        {
            Debug.Log("LevelManager: Active seed launched!");
        }
    }

    private void HandleSeedStabilized(SeedController stabilizedSeed)
    {
        if (stabilizedSeed == _activeSeed && !_levelCompleted)
        {
            _levelCompleted = true;
            Debug.Log("LEVEL COMPLETE! Active seed has stabilized.");
            
            if (InputManager.Instance != null)
            {
                InputManager.Instance.SetGamePlayingState(false); // Gameplay effectively paused by UI
            }

            if (gameplayUIController != null) // Use the serialized field
            {
                gameplayUIController.ShowLevelCompletePanel();
            }
            else
            {
                Debug.LogWarning("GameplayUIController not assigned in LevelManager Inspector. Cannot show Level Complete panel.");
            }
        }
    }

    private void CheckSeedOutOfBounds()
    {
        if (_mainCamera == null || _activeSeed == null) return;

        Vector3 screenPoint = _mainCamera.WorldToViewportPoint(_activeSeed.transform.position);
        // Viewport coordinates are (0,0) bottom-left to (1,1) top-right.
        // We can add a small buffer.
        float buffer = 0.05f; // 5% buffer outside the viewport

        bool isOutOfX = screenPoint.x < -buffer || screenPoint.x > 1 + buffer;
        bool isOutOfY = screenPoint.y < -buffer || screenPoint.y > 1 + buffer;

        if (isOutOfX || isOutOfY)
        {
            Debug.Log($"LevelManager: Active seed '{_activeSeed.name}' is out of camera view. Resetting seed.", this);
            // Reset just the seed to its initial aiming position
            ResetActiveSeed();
        }
    }

    // Public method to be called by InputManager for manual reset (e.g., Right Mouse Button)
    public void RequestManualReset()
    {
        Debug.Log("LevelManager: Manual reset requested.");
        if (_activeSeed != null && _levelSetupComplete)
        {
            ResetActiveSeed();
            _levelCompleted = false; // Reset level complete status
            if (InputManager.Instance != null)
            {
                InputManager.Instance.SetGamePlayingState(true); // After reset, game is playable again
            }
        }
        else if (!_levelSetupComplete)
        {
            Debug.LogWarning("LevelManager: Level not set up. Cannot perform manual reset yet. Starting level instead.");
            StartLevel();
        }
    }

    private void ResetActiveSeed()
    {
        if (_activeSeed != null)
        {
            // Reset the seed back to its initial aiming state
            _activeSeed.PrepareForAiming(initialSeedPosition);
            _levelCompleted = false; // Reset completion flag
            Debug.Log($"LevelManager: Reset active seed to position {initialSeedPosition}");
        }
    }

    // Renamed from ResetLevel to avoid confusion with manual in-game reset.
    // This method is more like "restart the whole level from scratch"
    private void ResetLevelInternally()
    {
        Debug.Log("LevelManager: Resetting level internally.");
        ClearLevelObjects(); // Clear existing spawned objects
        _levelSetupComplete = false;
        _levelCompleted = false;
        // SetupLevel() will be called by StartLevel()
    }

    // New method to handle returning to the main menu
    public void ReturnToMainMenu() // Modified to also hide GameplayUI
    {
        Debug.Log("LevelManager: Returning to Main Menu.");
        ClearLevelObjects();
        _levelSetupComplete = false;
        _levelCompleted = false;

        if (InputManager.Instance != null)
        {
            InputManager.Instance.SetGamePlayingState(false);
        }

        if (gameplayUIController != null)
        {
            gameplayUIController.HideGameplayUI(); // Hide the gameplay canvas
        }

        if (mainMenuController != null) // Use the serialized field
        {
            mainMenuController.ShowMenu(); // This also sets Time.timeScale = 0f
        }
        else
        {
            Debug.LogError("MainMenuController not assigned in LevelManager Inspector. Cannot return to menu.");
        }
    }

    void CheckWinCondition()
    {
        // For MVP: if the _activeSeed is stable, level is complete.
        if (_activeSeed != null && _activeSeed.IsStable && !_levelCompleted)
        {
            HandleSeedStabilized(_activeSeed);
        }
    }

    // Optional: Call this if you want to reset the level (e.g., from a UI button later)
    public void ResetLevel()
    {
        Debug.Log("LevelManager: Resetting level...");
        ResetLevelInternally();
        SetupLevel();
    }

    void OnDestroy()
    {
        // Unsubscribe from events if _activeSeed might persist or LevelManager is destroyed before seed
        if (_activeSeed != null)
        {
            _activeSeed.OnSeedStabilized -= HandleSeedStabilized;
            _activeSeed.OnSeedLaunched -= HandleSeedLaunched;
        }
        // If LevelManager was a persistent singleton, clear Instance here:
        // if (Instance == this) Instance = null;
    }
}