using UnityEngine;

public class JumpForceZone : MonoBehaviour
{
    [SerializeField] private float jumpForce = 8f;
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