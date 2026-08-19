using UnityEngine;

public class JumpForceZone : MonoBehaviour
{
    private const float DefaultJumpForce = 8f;

    [SerializeField] private float jumpForce = DefaultJumpForce;
    private float originalJumpForce;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
        originalJumpForce = playerMovement.JumpForce;
        playerMovement.JumpForce = jumpForce;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<PlayerMovement>().JumpForce = originalJumpForce;
    }
}