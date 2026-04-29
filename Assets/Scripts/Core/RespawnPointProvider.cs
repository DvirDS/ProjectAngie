using UnityEngine;

public class RespawnPointProvider : MonoBehaviour
{
    [SerializeField] private GameObject respawnPoint;

    private void Awake()
    {
        RespawnManager.instance.respawnPoint = respawnPoint.transform;
    }
}
