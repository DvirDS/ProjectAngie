using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private int pointsValue = 10;

    [Header("Persistence")]
    [Tooltip("Must be unique across the whole game. E.g. 'watchtower_coin_01'")]
    [SerializeField] private string coinID;

    private void Start()
    {
        if (CoinsManager.Instance != null &&
            CoinsManager.Instance.IsCollected(coinID))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CollectCoin();
        }
    }

    private void CollectCoin()
    {
        if (CoinsManager.Instance != null)
            CoinsManager.Instance.MarkCollected(coinID);

        if (SkillTreeManager.Instance != null)
        {
            SkillTreeManager.Instance.AddSkillPoints(pointsValue);
        }

        Destroy(gameObject);
    }
}