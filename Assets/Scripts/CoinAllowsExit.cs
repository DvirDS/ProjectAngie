using UnityEngine;

public class CoinAllowsExit : MonoBehaviour
{
    [Header("Exit")]
    [SerializeField] private GameObject exit;
    private string playerTag = "Player";

    void Awake()
    {
        if (exit == null)
        {
            Debug.LogWarning($"{name}: exit is null.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) 
            exit.gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
