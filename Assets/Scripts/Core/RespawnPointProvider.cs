using UnityEngine;

public class RespawnPointProvider : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        RespawnManager.Instance.RespawnPoint = respawnPoint;
    }
}