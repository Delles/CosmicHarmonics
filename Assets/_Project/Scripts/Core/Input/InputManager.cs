using UnityEngine; 
using UnityEngine.InputSystem; // Import the new Input System namespace

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; } // Singleton instance

    private PlayerControls _playerControls;
    private Camera _mainCamera;

    public SeedController testSeedToLaunch; // Assign this in the Inspector

    public Vector2 FlickStartPosition { get; private set; }
    public Vector2 FlickEndPosition { get; private set; }
    public bool IsFlicking { get; private set; }

    // Optional: Events for other systems to subscribe to
    public event System.Action<Vector2> OnFlickStart; // Parameter: World start position
    public event System.Action<Vector2, Vector2> OnFlickEnd; // Parameters: World start position, World end position

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        try
        {
            _playerControls = new PlayerControls();
            if (_playerControls == null)
            {
                Debug.LogError("InputManager: _playerControls is NULL immediately after new PlayerControls()!", this);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"InputManager: Exception during new PlayerControls(): {e.Message}\n{e.StackTrace}", this);
        }
        
        _mainCamera = Camera.main; // Cache the main camera
        if (_mainCamera == null)
        {
            Debug.LogWarning("InputManager: Camera.main is null in Awake(). Will retry later.", this);
        }
    }

    private void OnEnable()
    {
        if (_playerControls == null)
        {
            Debug.LogError("InputManager: _playerControls is NULL at the start of OnEnable()!", this);
            return; // Exit OnEnable if _playerControls is null to prevent further errors
        }

        try
        {
            _playerControls.Gameplay.Enable();

            // Subscribe to the actions
            _playerControls.Gameplay.FlickPress.started += OnFlickPressStarted;
            _playerControls.Gameplay.FlickPress.canceled += OnFlickPressCanceled; // Canceled usually means released
        }
        catch (System.Exception e)
        {
            Debug.LogError($"InputManager: Exception during OnEnable(): {e.Message}\n{e.StackTrace}", this);
        }
    }

    private void OnDisable()
    {
        if (_playerControls != null)
        {
            _playerControls.Gameplay.FlickPress.started -= OnFlickPressStarted;
            _playerControls.Gameplay.FlickPress.canceled -= OnFlickPressCanceled;
            _playerControls.Gameplay.Disable();
        }
    }

    private void OnFlickPressStarted(InputAction.CallbackContext context)
    {
        if (_playerControls == null) { // Extra safety
            Debug.LogError("InputManager: OnFlickPressStarted called but _playerControls is null!");
            return;
        }
        IsFlicking = true;
        // Read the pointer position AT THE START of the flick
        FlickStartPosition = GetPointerPositionInWorld();
        OnFlickStart?.Invoke(FlickStartPosition);

        // For debugging:
        Debug.Log($"Flick Started at Screen: {_playerControls.Gameplay.PointerPosition.ReadValue<Vector2>()}, World: {FlickStartPosition}");
    }

    private void OnFlickPressCanceled(InputAction.CallbackContext context)
    {
        if (_playerControls == null) { // Extra safety
            Debug.LogError("InputManager: OnFlickPressCanceled called but _playerControls is null!");
            return;
        }
        if (IsFlicking) // Ensure we were actually flicking
        {
            IsFlicking = false;
            // Read the pointer position AT THE END of the flick
            FlickEndPosition = GetPointerPositionInWorld();
            OnFlickEnd?.Invoke(FlickStartPosition, FlickEndPosition); // Re-enabled this event

            // For debugging:
            Debug.Log($"Flick Ended at Screen: {_playerControls.Gameplay.PointerPosition.ReadValue<Vector2>()}, World: {FlickEndPosition}");
            
            Vector2 flickVector = FlickEndPosition - FlickStartPosition;
            Debug.Log($"Flick Vector (World): {flickVector}");

            // --- TEMPORARY CODE TO LAUNCH THE SEED ---
            if (testSeedToLaunch != null)
            {
                // Optional: Reset the seed to the start position before launching
                // This makes repeated testing easier from the same spot
                testSeedToLaunch.transform.position = FlickStartPosition; 
                testSeedToLaunch.ResetSeed(FlickStartPosition); // Reset its state and velocity

                testSeedToLaunch.Launch(flickVector);
            }
            else
            {
                Debug.LogWarning("No Test Seed assigned to InputManager to launch.");
            }
            // --- END OF TEMPORARY CODE ---
        }
    }

    // Public method to get the current pointer position in world space
    public Vector2 GetPointerPositionInWorld()
    {
        if (_mainCamera == null)
        {
            // It's possible Awake hasn't run yet if this is called too early from elsewhere
            // or if Camera.main was not available.
            _mainCamera = Camera.main; 
            if(_mainCamera == null) {
                 Debug.LogError("Main Camera is not assigned and Camera.main is null!");
                 return Vector2.zero;
            }
        }
        if (_playerControls == null) { // Extra safety
            Debug.LogError("InputManager: GetPointerPositionInWorld called but _playerControls is null!");
            return Vector2.zero;
        }

        Vector2 screenPosition = _playerControls.Gameplay.PointerPosition.ReadValue<Vector2>();
        Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, _mainCamera.nearClipPlane));
        
        // For a 2D orthographic camera, you might want to ensure Z is 0, 
        // or whatever your gameplay plane is. Since ScreenToWorldPoint gives a Z based on nearClipPlane
        // for an orthographic camera, and our gameplay elements are likely at Z=0:
        return new Vector2(worldPosition.x, worldPosition.y);
    }

    // Example of how another script might access the flick data (if not using events)
    // public Vector2 GetFlickVectorWorld()
    // {
    //     if (!IsFlicking && FlickEndPosition != Vector2.zero) // Check if a flick just ended
    //     {
    //         return FlickEndPosition - FlickStartPosition;
    //     }
    //     return Vector2.zero;
    // }
}