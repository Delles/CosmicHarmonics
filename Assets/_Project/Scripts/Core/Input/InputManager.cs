using UnityEngine;
using UnityEngine.InputSystem; // Import the new Input System namespace

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; } // Singleton instance

    private PlayerControls _playerControls;
    private Camera _mainCamera;

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
        // DontDestroyOnLoad(gameObject); // Consider if this manager should persist across scenes

        _playerControls = new PlayerControls();
        _mainCamera = Camera.main; // Cache the main camera
    }

    private void OnEnable()
    {
        _playerControls.Gameplay.Enable();

        // Subscribe to the actions
        _playerControls.Gameplay.FlickPress.started += OnFlickPressStarted;
        _playerControls.Gameplay.FlickPress.canceled += OnFlickPressCanceled; // Canceled usually means released
    }

    private void OnDisable()
    {
        _playerControls.Gameplay.FlickPress.started -= OnFlickPressStarted;
        _playerControls.Gameplay.FlickPress.canceled -= OnFlickPressCanceled;
        
        _playerControls.Gameplay.Disable();
    }

    private void OnFlickPressStarted(InputAction.CallbackContext context)
    {
        IsFlicking = true;
        // Read the pointer position AT THE START of the flick
        FlickStartPosition = GetPointerPositionInWorld();
        OnFlickStart?.Invoke(FlickStartPosition);

        // For debugging:
        Debug.Log($"Flick Started at Screen: {_playerControls.Gameplay.PointerPosition.ReadValue<Vector2>()}, World: {FlickStartPosition}");
    }

    private void OnFlickPressCanceled(InputAction.CallbackContext context)
    {
        if (IsFlicking) // Ensure we were actually flicking
        {
            IsFlicking = false;
            // Read the pointer position AT THE END of the flick
            FlickEndPosition = GetPointerPositionInWorld();
            OnFlickEnd?.Invoke(FlickStartPosition, FlickEndPosition);

            // For debugging:
            Debug.Log($"Flick Ended at Screen: {_playerControls.Gameplay.PointerPosition.ReadValue<Vector2>()}, World: {FlickEndPosition}");
            Debug.Log($"Flick Vector (World): {FlickEndPosition - FlickStartPosition}");
        }
    }

    // Public method to get the current pointer position in world space
    public Vector2 GetPointerPositionInWorld()
    {
        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera is not assigned in InputManager!");
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