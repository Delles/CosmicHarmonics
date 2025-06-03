using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public Vector2 FlickStartPosition { get; private set; }
    public Vector2 FlickEndPosition { get; private set; }
    public bool IsFlicking { get; private set; }

    public event System.Action<Vector2> OnFlickStart;
    public event System.Action<Vector2, Vector2> OnFlickEnd; // World start, world end

    private PlayerControls _playerControls;
    private Camera _mainCamera;
    
    // To check if we are in a state where ESC should trigger return to menu
    // i.e., game is running, not already on main menu, not on level complete screen
    private bool _isGameCurrentlyPlaying = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _playerControls = new PlayerControls();
        _mainCamera = Camera.main;

        if (_mainCamera == null)
        {
            Debug.LogError("InputManager: Main Camera not found! Ensure your camera is tagged 'MainCamera'.");
        }
    }

    void Update()
    {
        if (!_isGameCurrentlyPlaying || IsFlicking || LevelManager.Instance == null || LevelManager.Instance._activeSeed == null) return;

        if (LevelManager.Instance._activeSeed.IsReadyForAiming)
        {
            LevelManager.Instance.UpdateActiveSeedAimPosition(GetPointerPositionInWorld());
        }
    }

    private void OnEnable()
    {
        if (_playerControls == null) _playerControls = new PlayerControls();
        _playerControls.Gameplay.Enable();
        _playerControls.Gameplay.FlickPress.started += OnFlickPressStarted;
        _playerControls.Gameplay.FlickPress.canceled += OnFlickPressCanceled;
        _playerControls.Gameplay.ResetAction.performed += OnResetActionPerformed;
        _playerControls.Gameplay.StopGame.performed += OnStopGamePerformed; // Subscribe to new action
    }

    private void OnDisable()
    {
        if (_playerControls != null)
        {
            _playerControls.Gameplay.FlickPress.started -= OnFlickPressStarted;
            _playerControls.Gameplay.FlickPress.canceled -= OnFlickPressCanceled;
            _playerControls.Gameplay.ResetAction.performed -= OnResetActionPerformed;
            _playerControls.Gameplay.StopGame.performed -= OnStopGamePerformed; // Unsubscribe
            _playerControls.Gameplay.Disable();
        }
    }

    // Method to be called by LevelManager when game starts/stops
    public void SetGamePlayingState(bool isPlaying)
    {
        _isGameCurrentlyPlaying = isPlaying;
    }

    private void OnStopGamePerformed(InputAction.CallbackContext context)
    {
        Debug.Log("StopGame Action Performed (ESC pressed). Is game currently playing: " + _isGameCurrentlyPlaying);
        // We only want ESC to return to menu if:
        // 1. The game is actually considered "playing" (not on main menu already)
        // 2. The level complete panel is NOT showing (as it has its own menu button and pauses game)
        
        // Check if LevelManager exists and if the level is complete.
        // A simpler check: if Time.timeScale is already 0, we might be on a menu.
        if (Time.timeScale == 0f) 
        {
            // Game is likely already paused by Main Menu or Level Complete screen.
            // Let their UI buttons handle the logic.
            // Or, if on LevelCompletePanel, pressing ESC could potentially act as "ReturnToMenuButton"
            // For now, let's keep it simple: ESC works when game is actively running.
            Debug.Log("Game is already paused (Time.timeScale is 0). ESC action ignored for now.");
            return;
        }

        if (_isGameCurrentlyPlaying && LevelManager.Instance != null)
        {
            Debug.Log("ESC pressed during active gameplay. Returning to main menu.");
            LevelManager.Instance.ReturnToMainMenu();
            SetGamePlayingState(false); // Game is no longer "playing" in the foreground
        }
    }

    private void OnFlickPressStarted(InputAction.CallbackContext context)
    {
        if (!_isGameCurrentlyPlaying || LevelManager.Instance == null || LevelManager.Instance._activeSeed == null || !LevelManager.Instance._activeSeed.IsReadyForAiming)
        {
            IsFlicking = false;
            return;
        }
        
        IsFlicking = true;
        FlickStartPosition = GetPointerPositionInWorld();
        OnFlickStart?.Invoke(FlickStartPosition);
        // Debug.Log($"Flick Press Started at Screen: {Pointer.current.position.ReadValue()} World: {FlickStartPosition}");
    }

    private void OnFlickPressCanceled(InputAction.CallbackContext context)
    {
        if (!IsFlicking || !_isGameCurrentlyPlaying) // Ensure flick was actually started and game is active
        {
            IsFlicking = false; // Reset just in case
            return;
        }

        IsFlicking = false;
        FlickEndPosition = GetPointerPositionInWorld();
        // Debug.Log($"Flick Press Canceled at Screen: {Pointer.current.position.ReadValue()} World: {FlickEndPosition}");

        if (LevelManager.Instance != null)
        {
            // Pass flick start position from where it was recorded, not current aim position.
            LevelManager.Instance.RequestLaunchActiveSeed(FlickStartPosition, FlickEndPosition);
        }
        OnFlickEnd?.Invoke(FlickStartPosition, FlickEndPosition);
    }

    private void OnResetActionPerformed(InputAction.CallbackContext context)
    {
        if (!_isGameCurrentlyPlaying || LevelManager.Instance == null) return;

        Debug.Log("Reset Action Performed (Right Mouse Button).");
        LevelManager.Instance.RequestManualReset();
    }

    public Vector2 GetPointerPositionInWorld()
    {
        if (_mainCamera == null || _playerControls == null)
        {
            Debug.LogError("InputManager: MainCamera or PlayerControls not initialized in GetPointerPositionInWorld.");
            return Vector2.zero;
        }
        Vector3 screenPos = _playerControls.Gameplay.PointerPosition.ReadValue<Vector2>();
        screenPos.z = -_mainCamera.transform.position.z; // Set z to distance from camera to world plane
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
        return new Vector2(worldPos.x, worldPos.y);
    }
}