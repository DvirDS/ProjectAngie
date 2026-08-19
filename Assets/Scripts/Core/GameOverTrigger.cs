using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GameOverTrigger : MonoBehaviour
{
    private const float DefaultDuration = 6f;

    [SerializeField] private float duration = DefaultDuration;

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