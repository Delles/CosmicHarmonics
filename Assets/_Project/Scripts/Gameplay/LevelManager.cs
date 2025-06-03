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
    [SerializeField] private Vector2 initialSeedPosition = new Vector2(-5f, 0f); // Renamed for clarity - initial position before mouse takes over
    [SerializeField] private Vector2 starSpawnPosition = new Vector2(3f, 0f);
    [SerializeField] private Vector2 stabilityZoneSpawnPosition = new Vector2(0f, 3f);
    [SerializeField] private Vector2 stabilityZoneScale = new Vector2(3f, 2f); // Example scale for a box zone

    [Header("Runtime References")]
    public SeedController _activeSeed; // Made public so InputManager can access it
    // private List<SeedController> _allSeedsInLevel; // For future when we have multiple seeds
    // private List<SeedController> _stabilizedSeeds; // For future

    private bool _levelSetupComplete = false;
    private bool _levelCompleted = false;
    private Camera _mainCamera; // Cache the main camera

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("LevelManager: Main Camera not found! Ensure camera is tagged 'MainCamera'.", this);
        }
        
        // _allSeedsInLevel = new List<SeedController>();
        // _stabilizedSeeds = new List<SeedController>();
    }

    void Start()
    {
        SetupLevel();
    }

    void Update()
    {
        if (!_levelSetupComplete || _levelCompleted) return;

        // Check for seed out of bounds
        if (_activeSeed != null && _activeSeed.IsLaunched && !_activeSeed.IsStable) // Only check if launched and not yet stable
        {
            CheckSeedOutOfBounds();
        }

        CheckWinCondition();
    }

    void SetupLevel()
    {
        Debug.Log("LevelManager: Setting up level...");

        // Spawn Star
        if (starPrefab != null)
        {
            Instantiate(starPrefab, starSpawnPosition, Quaternion.identity, transform.parent.Find("// -- ENVIRONMENT --")); // Optional: parent to Environment
            Debug.Log($"LevelManager: Spawned Star at {starSpawnPosition}");
        }
        else
        {
            Debug.LogError("LevelManager: Star Prefab not assigned!");
        }

        // Spawn Stability Zone
        if (stabilityZonePrefab != null)
        {
            GameObject zoneGO = Instantiate(stabilityZonePrefab, stabilityZoneSpawnPosition, Quaternion.identity, transform.parent.Find("// -- ENVIRONMENT --"));
            zoneGO.transform.localScale = stabilityZoneScale; // Adjust scale if necessary
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
        Debug.Log("LevelManager: Level setup complete.");
    }

    public void SpawnSeed()
    {
        if (seedPrefab != null)
        {
            // Ensure only one active seed for now in this simple setup
            if (_activeSeed != null && _activeSeed.gameObject.activeInHierarchy)
            {
                 Debug.LogWarning("LevelManager: An active seed already exists. Resetting it for aiming.");
                 _activeSeed.PrepareForAiming(initialSeedPosition); // Reset existing seed
                 return;
            }

            GameObject seedGO = Instantiate(seedPrefab, initialSeedPosition, Quaternion.identity, transform.parent.Find("// -- DYNAMIC_OBJECTS --"));
            _activeSeed = seedGO.GetComponent<SeedController>();

            if (_activeSeed != null)
            {
                _activeSeed.PrepareForAiming(initialSeedPosition); // Tell the new seed to be kinematic and ready
                // Subscribe to the seed's stabilization event
                _activeSeed.OnSeedStabilized += HandleSeedStabilized;
                _activeSeed.OnSeedLaunched += HandleSeedLaunched; // New event
                // _allSeedsInLevel.Add(_activeSeed); // For future
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
        if (_activeSeed != null && _activeSeed.IsReadyForAiming)
        {
            _activeSeed.UpdateAimPosition(mouseWorldPosition);
        }
    }

    // Called by InputManager when a flick is performed
    public void RequestLaunchActiveSeed(Vector2 flickStartPosition, Vector2 flickEndPosition)
    {
        if (_activeSeed == null || !_activeSeed.gameObject.activeInHierarchy)
        {
            Debug.LogError("LevelManager: No active seed to launch or seed is inactive. Level may need reset.");
            return;
        }

        // Seed should ALREADY be at flickStartPosition because it was following the mouse,
        // and UpdateAimPosition would have stopped when InputManager.IsFlicking became true.
        // We ensure it's not already launched or stable.
        if (_activeSeed != null && !_activeSeed.IsLaunched && !_activeSeed.IsStable)
        {
            // Sanity check or ensure its position is exactly the flick start.
            // _activeSeed.transform.position = flickStartPosition; // This might be redundant if UpdateAimPosition worked correctly up to the flick start.

            Vector2 flickVector = flickEndPosition - flickStartPosition;
            _activeSeed.Launch(flickVector); // SeedController now handles making itself Dynamic
            Debug.Log("LevelManager: Launching active seed.");
        }
        else if (_activeSeed != null && (_activeSeed.IsLaunched || _activeSeed.IsStable))
        {
            Debug.Log($"LevelManager: Seed '{_activeSeed.name}' is already launched or stable. Ignoring flick attempt.");
        }
        else if (_activeSeed == null)
        {
             Debug.LogError("LevelManager: _activeSeed is null. Cannot launch.");
        }
    }

    private void HandleSeedLaunched(SeedController seed)
    {
        Debug.Log($"LevelManager: Seed {seed.name} was launched.");
        // Future: maybe deduct a seed from available count, etc.
    }

    private void HandleSeedStabilized(SeedController stabilizedSeed)
    {
        Debug.Log($"LevelManager: Seed {stabilizedSeed.name} has stabilized!");
        // if (!_stabilizedSeeds.Contains(stabilizedSeed)) // For future
        // {
        //     _stabilizedSeeds.Add(stabilizedSeed);
        // }
        // For this MVP, one seed stabilizing means level complete.
        if (!_levelCompleted) // Check if not already completed
        {
            _levelCompleted = true; // Set completion flag
            Debug.LogWarning("LEVEL COMPLETE! The seed has stabilized.");
            // Additional win condition logic can go here
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

    // Public method to be called by InputManager for manual reset
    public void RequestManualReset()
    {
        Debug.Log("LevelManager: Manual reset requested.", this);
        if (_activeSeed != null)
        {
            // Reset the seed to its initial position, ready for aiming again.
            ResetActiveSeed();
            _levelCompleted = false; // Ensure win condition is also reset
        }
        else
        {
            // If no active seed, might mean we need to spawn one (or do a full level reset)
            Debug.LogWarning("LevelManager: No active seed to reset, consider full level reset or spawning.");
            ResetLevel(); // Or SpawnSeed() if you prefer less aggressive reset
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

    void CheckWinCondition()
    {
        // For MVP: if the _activeSeed is stable, level is complete.
        if (_activeSeed != null && _activeSeed.IsStable && !_levelCompleted)
        {
            _levelCompleted = true;
            Debug.LogWarning("LEVEL COMPLETE! The seed has stabilized.");
            // Here you would trigger UI, sounds, next level logic, etc.
            // For now, we just log.
        }
    }

    // Optional: Call this if you want to reset the level (e.g., from a UI button later)
    public void ResetLevel()
    {
        Debug.Log("LevelManager: Resetting level...");
        // Clear existing spawned objects (simplistic for now, better would be pooling or specific cleanup)
        if (_activeSeed != null)
        {
            _activeSeed.OnSeedStabilized -= HandleSeedStabilized; // Unsubscribe
            _activeSeed.OnSeedLaunched -= HandleSeedLaunched;   // Unsubscribe
            Destroy(_activeSeed.gameObject);
            _activeSeed = null;
        }

        // This is a very naive way to clear other objects.
        // Proper way would be to keep references or use a parent GameObject to destroy.
        foreach (var star in FindObjectsByType<GravityWell_Star>(FindObjectsSortMode.None)) Destroy(star.gameObject);
        foreach (var zone in FindObjectsByType<StabilityZone>(FindObjectsSortMode.None)) Destroy(zone.gameObject);

        // _allSeedsInLevel.Clear();
        // _stabilizedSeeds.Clear();
        _levelCompleted = false;
        _levelSetupComplete = false;
        Start(); // Re-setup the level
    }


    void OnDestroy()
    {
        // Unsubscribe from events if _activeSeed might persist or LevelManager is destroyed before seed
        if (_activeSeed != null)
        {
            _activeSeed.OnSeedStabilized -= HandleSeedStabilized;
            _activeSeed.OnSeedLaunched -= HandleSeedLaunched;
        }
    }
}