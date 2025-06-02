using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CircleCollider2D))]
public class GravityWell_Star : MonoBehaviour
{
    [Header("Settings")]
    public float gravityStrength = 50f; 
    public float effectRadius = 5f;    

    [Header("Debug (Read-Only)")]
    [SerializeField] // Show in inspector but not directly editable if we want
    private List<Rigidbody2D> affectedRigidbodies = new List<Rigidbody2D>();
    
    private CircleCollider2D _effectCollider;

    void Awake()
    {
        _effectCollider = GetComponent<CircleCollider2D>();
        if (_effectCollider == null)
        {
            Debug.LogError("GravityWell_Star requires a CircleCollider2D component on " + name, this);
            enabled = false; 
            return;
        }

        if (!_effectCollider.isTrigger)
        {
            Debug.LogWarning("CircleCollider2D on " + name + " is not 'Is Trigger'. Setting now.", this);
            _effectCollider.isTrigger = true;
        }
        
        // Sync collider radius with the editable effectRadius property at start
        _effectCollider.radius = effectRadius;
    }

    void FixedUpdate()
    {
        // Clean up list if any rigidbodies were destroyed (e.g. seed despawned)
        affectedRigidbodies.RemoveAll(item => item == null);

        foreach (Rigidbody2D rb in affectedRigidbodies)
        {
            // Ensure the Rigidbody and its GameObject are still active
            if (rb != null && rb.gameObject.activeInHierarchy) 
            {
                Vector2 directionToWell = ((Vector2)transform.position - rb.position).normalized;
                rb.AddForce(directionToWell * gravityStrength, ForceMode2D.Force);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Seed")) 
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null && !affectedRigidbodies.Contains(rb))
            {
                affectedRigidbodies.Add(rb);
                Debug.Log($"{other.name} entered gravity well of {name}", this);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Seed")) 
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null && affectedRigidbodies.Contains(rb))
            {
                affectedRigidbodies.Remove(rb);
                Debug.Log($"{other.name} exited gravity well of {name}", this);
            }
        }
    }

    // Visualize the effectRadius in the editor when selected
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.25f); // Yellowish, semi-transparent fill
        Gizmos.DrawSphere(transform.position, effectRadius);
        Gizmos.color = Color.yellow; // Outline
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
    
    // Update the collider's radius if effectRadius is changed in the inspector (Editor only)
    void OnValidate()
    {
        // This function is called when the script is loaded or a value is changed in the Inspector.
        if (Application.isPlaying) return; // Don't run OnValidate logic heavily during playmode if it causes issues, rely on Awake

        if (_effectCollider == null)
        {
            // Attempt to get it if we're in editor and it's not set
            _effectCollider = GetComponent<CircleCollider2D>();
        }

        if (_effectCollider != null)
        {
            if (Mathf.Abs(_effectCollider.radius - effectRadius) > 0.001f)
            {
                _effectCollider.radius = effectRadius;
                // Forcing a scene save or reimport might be needed for editor visuals to always catch up
                // if Unity's own gizmo drawing for colliders is being stubborn.
            }
        }
    }
}