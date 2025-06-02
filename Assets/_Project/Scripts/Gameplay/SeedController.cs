using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class SeedController : MonoBehaviour
{
    [Header("Launch Settings")]
    public float launchForceMultiplier = 10f; // Adjust this to get the right "feel" for the flick

    [Header("Stabilization Settings")]
    public float stabilizationTimeRequired = 2f; // Time in seconds to become stable
    public float quasiStationaryVelocityThreshold = 0.1f; // Max velocity magnitude to be considered "quasi-stationary"

    [Header("State Feedback (Temporary)")]
    public Color flyingColor = Color.white;
    public Color stabilizingColor = Color.yellow;
    public Color stableColor = Color.green;

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private bool _isLaunched = false; // To prevent applying launch force multiple times
    
    private StabilityZone _currentStabilityZone = null;
    private float _stabilizationTimer = 0f;
    private bool _isStabilizing = false;
    private bool _isStable = false;
    private bool _isInStabilityZone = false; // More explicit flag

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
                    _isStable = true;
                    _isStabilizing = false; // No longer "stabilizing", it IS stable
                    Debug.Log($"{name} HAS BECOME STABLE in {_currentStabilityZone.name}!", this);
                    SetColor(stableColor);
                    // Here you would typically notify a LevelManager or trigger other game events
                    // For now, we just log it and change color.
                    // Optionally, make the Rigidbody kinematic once stable
                    // _rb.bodyType = RigidbodyType2D.Kinematic; 
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

    // Public method to launch the seed
    // Expects a world-space direction and magnitude for the flick
    public void Launch(Vector2 flickVector)
    {
        if (_isLaunched && !_isStable) // Allow re-launch if not stable? Or always reset first?
        {
            // For now, let's assume ResetSeed is called before any new launch.
            // If you want to re-launch an already flying seed, this logic might need adjustment.
            Debug.LogWarning($"{name} is already launched. Reset before launching again.", this);
            // return; // Or, reset and launch: ResetSeed(transform.position);
        }
        if (_isStable)
        {
            Debug.Log($"{name} is stable and cannot be launched. Reset first.", this);
            return;
        }

        if (_rb != null)
        {
            if (_rb.bodyType == RigidbodyType2D.Kinematic) // Only change if it's currently Kinematic
            {
                _rb.bodyType = RigidbodyType2D.Dynamic;
            }
            _rb.AddForce(flickVector * launchForceMultiplier, ForceMode2D.Impulse);
            _isLaunched = true;
            SetColor(flyingColor); // Set color on launch
            Debug.Log($"{name} launched with vector {flickVector} and force multiplier {launchForceMultiplier}. Velocity: {_rb.linearVelocity}", this);
        }
        else
        {
            Debug.LogError("Cannot launch seed: Rigidbody2D is missing.", this);
        }
    }

    public void EnterStabilityZone(StabilityZone zone)
    {
        Debug.Log($"{name} notified: Entered zone {zone.name}", this);
        _isInStabilityZone = true;
        _currentStabilityZone = zone;
        _isStabilizing = false; // Reset stabilizing flag, will be re-evaluated in Update/FixedUpdate
        _stabilizationTimer = 0f; // Reset timer on entering a new zone or re-entering
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
        if (_rb == null) _rb = GetComponent<Rigidbody2D>(); // Ensure rb is assigned

        _rb.bodyType = RigidbodyType2D.Dynamic; // Ensure it's dynamic for launch
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;
        transform.position = startPosition;

        _isLaunched = false;
        _isStabilizing = false;
        _isStable = false;
        _isInStabilityZone = false;
        _currentStabilityZone = null;
        _stabilizationTimer = 0f;

        SetColor(flyingColor); // Reset to default flying color
        Debug.Log($"{name} has been reset at {startPosition}.", this);
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