using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GameOverTrigger : MonoBehaviour
{
    [SerializeField] private float duration = 6f;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMovement movement = other.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.SlowToStop(duration);
    }
}