using UnityEngine;
using System; // For Action

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class SeedController : MonoBehaviour
{
    [Header("Launch Settings")]
    public float launchForceMultiplier = 10f; // Adjust this to get the right "feel" for the flick

    [Header("Stabilization Settings")]
    public float stabilizationTimeRequired = 2f; // Time in seconds to become stable
    public float quasiStationaryVelocityThreshold = 0.1f; // Max velocity magnitude to be considered "quasi-stationary"

    [Header("Physics Settings")]
    [SerializeField] private float defaultDrag = 0.1f; // Base drag when not in a stability zone (can be 0)
    [SerializeField] private float dragInStabilityZone = 2f; // Increased drag when inside a zone

    [Header("State Feedback (Temporary)")]
    public Color flyingColor = Color.white;
    public Color stabilizingColor = Color.yellow;
    public Color stableColor = Color.green;

    // New public events
    public event Action<SeedController> OnSeedStabilized;
    public event Action<SeedController> OnSeedLaunched; // To notify when Launch() is called

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private bool _isLaunched = false; // To prevent applying launch force multiple times
    
    private StabilityZone _currentStabilityZone = null;
    private float _stabilizationTimer = 0f;
    private bool _isStabilizing = false;
    private bool _isStable = false;
    private bool _isInStabilityZone = false; // More explicit flag

    // Public getters for state access
    public bool IsLaunched => _isLaunched;
    public bool IsStable => _isStable;
    // NEW: Add a property to check if the seed is ready for aiming/positioning by mouse
    public bool IsReadyForAiming => !_isLaunched && !_isStable && (_rb != null && _rb.bodyType == RigidbodyType2D.Kinematic);

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_rb == null)
        {
            Debug.LogError("SeedController requires a Rigidbody2D component.", this);
        }
        if (_spriteRenderer == null)
        {
            Debug.LogError("SeedController requires a SpriteRenderer component.", this);
        }
        
        if (_rb != null)
        {
            _rb.linearDamping = defaultDrag; // Set initial drag
        }
        SetColor(flyingColor); // Initial color
    }

    void Update() // Or FixedUpdate if velocity checks need to be in sync with physics
    {
        if (_isLaunched && !_isStable && _isInStabilityZone && _currentStabilityZone != null)
        {
            // Check if velocity is below threshold
            if (_rb.linearVelocity.magnitude < quasiStationaryVelocityThreshold)
            {
                if (!_isStabilizing)
                {
                    _isStabilizing = true;
                    Debug.Log($"{name} is now quasi-stationary. Starting stabilization timer.", this);
                    SetColor(stabilizingColor);
                }
                
                _stabilizationTimer += Time.deltaTime;
                // Debug.Log($"Stabilization progress: {_stabilizationTimer / stabilizationTimeRequired * 100f}%");

                if (_stabilizationTimer >= stabilizationTimeRequired)
                {
                    if (!_isStable) // Only invoke if changing state to stable
                    {
                        _isStable = true;
                        _isStabilizing = false; // No longer "stabilizing", it IS stable
                        Debug.Log($"{name} HAS BECOME STABLE in {_currentStabilityZone.name}!", this);
                        SetColor(stableColor);
                        OnSeedStabilized?.Invoke(this); // Invoke the event
                        // Here you would typically notify a LevelManager or trigger other game events
                        // For now, we just log it and change color.
                        // Optionally, make the Rigidbody kinematic once stable
                        // _rb.bodyType = RigidbodyType2D.Kinematic; 
                    }
                }
            }
            else
            {
                // If it was stabilizing but moved too fast
                if (_isStabilizing)
                {
                    Debug.Log($"{name} moved too fast, stabilization paused/reset.", this);
                    _isStabilizing = false;
                    SetColor(flyingColor); // Or a "zone but moving" color
                }
                _stabilizationTimer = 0f; // Reset timer if it moves too fast
            }
        }
        else if (_isStabilizing && !_isInStabilityZone) // If it was stabilizing but left the zone
        {
             _isStabilizing = false;
             _stabilizationTimer = 0f;
             SetColor(flyingColor);
             Debug.Log($"{name} left zone while stabilizing, stabilization reset.", this);
        }
    }

    // NEW Method: Called by LevelManager to prepare the seed for mouse aiming
    public void PrepareForAiming(Vector2 initialPosition)
    {
        transform.position = initialPosition;
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        if (_rb != null)
        {
            _rb.bodyType = RigidbodyType2D.Kinematic; // Make it kinematic
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.linearDamping = defaultDrag; // Ensure drag is reset if it was previously in a zone
        }
        
        _isLaunched = false;
        _isStable = false;
        _isStabilizing = false;
        _isInStabilityZone = false;
        _stabilizationTimer = 0f;
        _currentStabilityZone = null;
        SetColor(flyingColor); // Or a specific "aiming" color
        Debug.Log($"{name} prepared for aiming at {initialPosition}.", this);
    }

    // NEW Method: To update position while aiming (kinematic)
    public void UpdateAimPosition(Vector2 worldPosition)
    {
        if (IsReadyForAiming) // Only move if kinematic and ready for aiming
        {
            transform.position = worldPosition;
        }
    }

    // Public method to launch the seed
    // Expects a world-space direction and magnitude for the flick
    public void Launch(Vector2 flickVector)
    {
        if (_isStable) // Don't launch if already stable
        {
            Debug.Log("SeedController: Seed is stable, cannot launch.");
            return;
        }

        if (_rb == null)
        {
            Debug.LogError("SeedController: Rigidbody2D is null in Launch. Caching again.");
            _rb = GetComponent<Rigidbody2D>();
            if (_rb == null) return; // Still null, can't proceed
        }
        
        _rb.bodyType = RigidbodyType2D.Dynamic; // Make it Dynamic to react to forces
        // Ensure drag is set to default when launched, in case it was in a zone and reset kinematically
        _rb.linearDamping = defaultDrag;
        _rb.AddForce(flickVector * launchForceMultiplier, ForceMode2D.Impulse);
        
        _isLaunched = true; // Now it's officially launched
        _isStable = false; // Ensure it's not stable when launched
        _isStabilizing = false; // Reset stabilizing state on new launch
        _stabilizationTimer = 0f; // Reset timer on new launch

        SetColor(flyingColor); // Set color on launch
        Debug.Log($"{name} launched with vector {flickVector} and force multiplier {launchForceMultiplier}. Velocity: {_rb.linearVelocity}", this);
        
        OnSeedLaunched?.Invoke(this); // Invoke the new event
    }

    public void EnterStabilityZone(StabilityZone zone)
    {
        Debug.Log($"{name} notified: Entered zone {zone.name}", this);
        _isInStabilityZone = true;
        _currentStabilityZone = zone;
        _isStabilizing = false; // Reset stabilizing flag, will be re-evaluated in Update/FixedUpdate
        _stabilizationTimer = 0f; // Reset timer on entering a new zone or re-entering

        if (_rb != null && _rb.bodyType == RigidbodyType2D.Dynamic) // Only apply drag change if dynamic
        {
            _rb.linearDamping = dragInStabilityZone;
            Debug.Log($"{name} drag increased to {dragInStabilityZone} in zone.", this);
        }
    }

    public void ExitStabilityZone(StabilityZone zone)
    {
        // Ensure we are exiting the zone we thought we were in
        if (_currentStabilityZone == zone)
        {
            Debug.Log($"{name} notified: Exited zone {zone.name}", this);
            _isInStabilityZone = false;
            _currentStabilityZone = null;
            _isStabilizing = false;
            _stabilizationTimer = 0f;

            if (_rb != null && _rb.bodyType == RigidbodyType2D.Dynamic) // Only revert drag if dynamic
            {
                _rb.linearDamping = defaultDrag;
                Debug.Log($"{name} drag reverted to {defaultDrag} after exiting zone.", this);
            }

            if (!_isStable) // If it wasn't stable, revert color
            {
                SetColor(flyingColor);
            }
            // If it was stable and exits, should it lose stability? For now, let's say yes.
            // This depends on game design. Maybe stable seeds are "locked in".
            // For this basic implementation, exiting makes it not stable anymore if it's not desired to be permanent.
            // if (_isStable) _isStable = false; // Decide on this rule later. For now, let's not reset _isStable here.
        }
    }

    // Example of resetting the seed (useful for a spawner or restart)
    public void ResetSeed(Vector2 startPosition)
    {
        // Ensure drag is set to default here too, as it calls PrepareForAiming
        if (_rb != null) _rb.linearDamping = defaultDrag;
        // This effectively calls PrepareForAiming
        PrepareForAiming(startPosition);
        Debug.Log($"{name} has been reset at {startPosition} and is ready for aiming. Drag set to {defaultDrag}.", this);
    }

    private void SetColor(Color newColor)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = newColor;
        }
    }

    // For now, we'll manually trigger launch from another script for testing.
    // In the future, this might listen to an event from InputManager or be called by a LevelManager/SeedSpawner.
}