using UnityEngine;

public class RespawnPointProvider : MonoBehaviour
{
    [SerializeField] private GameObject respawnPoint;

    private void Awake()
    {
        RespawnManager.instance.RespawnPoint = respawnPoint.transform;
    }
}
