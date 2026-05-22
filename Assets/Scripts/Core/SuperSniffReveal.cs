using UnityEngine;

public class SuperSniffReveal : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D objectCollider;

    [Header("Settings")]
    [Tooltip("If true, the object cannot be touched/collected when invisible.")]
    [SerializeField] private bool disableCollisionWhenHidden = true;

    private void OnEnable()
    {
        PlayerSniff.OnSuperSniff += HandleSuperSniff;
    }

    private void OnDisable()
    {
        PlayerSniff.OnSuperSniff -= HandleSuperSniff;
    }

    private void Start()
    {
        HandleSuperSniff(false);
    }

    private void HandleSuperSniff(bool isSniffing)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = isSniffing;
        }

        if (disableCollisionWhenHidden && objectCollider != null)
        {
            objectCollider.enabled = isSniffing;
        }
    }
}