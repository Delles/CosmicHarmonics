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
        if (!IsFlicking && LevelManager.Instance != null && LevelManager.Instance._activeSeed != null && LevelManager.Instance._activeSeed.IsReadyForAiming)
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
    }

    private void OnDisable()
    {
        if (_playerControls != null)
        {
            _playerControls.Gameplay.FlickPress.started -= OnFlickPressStarted;
            _playerControls.Gameplay.FlickPress.canceled -= OnFlickPressCanceled;
            _playerControls.Gameplay.ResetAction.performed -= OnResetActionPerformed;
            _playerControls.Gameplay.Disable();
        }
    }

    private void OnFlickPressStarted(InputAction.CallbackContext context)
    {
        if (_mainCamera == null) return;

        if (LevelManager.Instance != null && LevelManager.Instance._activeSeed != null &&
            !LevelManager.Instance._activeSeed.IsLaunched && !LevelManager.Instance._activeSeed.IsStable)
        {
            IsFlicking = true;
            FlickStartPosition = GetPointerPositionInWorld();
            OnFlickStart?.Invoke(FlickStartPosition);
            Debug.Log($"InputManager: Flick Started. Seed is at World: {FlickStartPosition}");
        }
        else
        {
            Debug.Log("InputManager: Flick attempt ignored, seed not ready or already processed.");
        }
    }

    private void OnFlickPressCanceled(InputAction.CallbackContext context)
    {
        if (!IsFlicking || _mainCamera == null)
        {
            if (IsFlicking) IsFlicking = false;
            return;
        }

        IsFlicking = false;
        FlickEndPosition = GetPointerPositionInWorld();
        OnFlickEnd?.Invoke(FlickStartPosition, FlickEndPosition);

        if (LevelManager.Instance != null)
        {
            if (LevelManager.Instance._activeSeed != null &&
                !LevelManager.Instance._activeSeed.IsLaunched &&
                !LevelManager.Instance._activeSeed.IsStable)
            {
                LevelManager.Instance.RequestLaunchActiveSeed(FlickStartPosition, FlickEndPosition);
            }
            else
            {
                Debug.Log("InputManager: Flick cancelled, but seed state not appropriate for launch (already launched/stable).");
            }
        }
        else
        {
            Debug.LogError("InputManager: LevelManager.Instance is null. Cannot request seed launch.");
        }
    }

    private void OnResetActionPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("InputManager: Reset Action Performed (Right Mouse Click).");
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RequestManualReset();
        }
        else
        {
            Debug.LogError("InputManager: LevelManager.Instance is null. Cannot request manual reset.");
        }
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