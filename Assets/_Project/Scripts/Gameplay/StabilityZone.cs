using UnityEngine;

public class StabilityZone : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Color activeColor = new Color(0.5f, 1f, 0.5f, 0.5f); // Light green, semi-transparent
    [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 1f, 0.3f); // Light blue, more transparent

    private SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError("StabilityZone requires a SpriteRenderer component.", this);
        }
        SetZoneColor(false); // Initial color
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Seed"))
        {
            SeedController seed = other.GetComponent<SeedController>();
            if (seed != null)
            {
                Debug.Log($"{other.name} entered Stability Zone: {gameObject.name}", this);
                seed.EnterStabilityZone(this);
                // Potentially change zone visual or sound cue
                SetZoneColor(true); 
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Seed"))
        {
            SeedController seed = other.GetComponent<SeedController>();
            if (seed != null)
            {
                Debug.Log($"{other.name} exited Stability Zone: {gameObject.name}", this);
                seed.ExitStabilityZone(this);
                // Potentially revert zone visual or sound cue
                SetZoneColor(false);
            }
        }
    }

    private void SetZoneColor(bool isActive)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = isActive ? activeColor : inactiveColor;
        }
    }

    // Optional: Gizmo for editor visualization
    void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = new Color(0, 1, 1, 0.3f); // Cyan, semi-transparent
            if (col is BoxCollider2D boxCollider)
            {
                // Ensure Gizmos use the GameObject's transform for position, rotation, and scale
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.DrawCube(boxCollider.offset, boxCollider.size);
            }
            else if (col is CircleCollider2D circleCollider)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position + (Vector3)circleCollider.offset, transform.rotation, transform.lossyScale);
                Gizmos.DrawSphere(Vector3.zero, circleCollider.radius);
            }
            Gizmos.matrix = Matrix4x4.identity; // Reset matrix
        }
    }
}