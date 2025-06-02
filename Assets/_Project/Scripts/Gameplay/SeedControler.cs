using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class SeedController : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool _isLaunched = false; // To prevent applying launch force multiple times

    // Basic properties (can be expanded later, e.g., from a ScriptableObject)
    public float launchForceMultiplier = 10f; // Adjust this to get the right "feel" for the flick

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("SeedController requires a Rigidbody2D component.", this);
        }
    }

    // Public method to launch the seed
    // Expects a world-space direction and magnitude for the flick
    public void Launch(Vector2 flickVector)
    {
        if (_isLaunched) return; // Only launch once

        if (rb != null)
        {
            if (rb.bodyType == RigidbodyType2D.Kinematic) // Only change if it's currently Kinematic
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
            rb.AddForce(flickVector * launchForceMultiplier, ForceMode2D.Impulse);
            _isLaunched = true;
            Debug.Log($"Seed launched with force: {flickVector * launchForceMultiplier}", this);
        }
        else
        {
            Debug.LogError("Cannot launch seed: Rigidbody2D is missing.", this);
        }
    }

    // Example of resetting the seed (useful for a spawner or restart)
    public void ResetSeed(Vector2 startPosition)
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            // rb.bodyType = RigidbodyType2D.Kinematic; // Optional: make it kinematic until launched again
        }
        transform.position = startPosition;
        _isLaunched = false;
        Debug.Log("Seed has been reset.", this);
    }

    // For now, we'll manually trigger launch from another script for testing.
    // In the future, this might listen to an event from InputManager or be called by a LevelManager/SeedSpawner.
}