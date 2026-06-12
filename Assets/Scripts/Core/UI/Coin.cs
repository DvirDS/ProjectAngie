using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private int pointsValue = 10;

    [Header("UI Effects")]
    [SerializeField] private GameObject floatingTextPrefab;

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
        if (floatingTextPrefab != null)
        {
            Canvas screenCanvas = Object.FindAnyObjectByType<Canvas>();

            if (screenCanvas != null)
            {
                GameObject textInstance = Instantiate(floatingTextPrefab, screenCanvas.transform);

                FloatingText floatingTextScript = textInstance.GetComponent<FloatingText>();
                if (floatingTextScript != null)
                {
                    floatingTextScript.Initialize($"+{pointsValue}", transform.position);
                }
            }
        }

        if (CoinsManager.Instance != null)
            CoinsManager.Instance.MarkCollected(coinID);

        if (SkillTreeManager.Instance != null)
        {
            SkillTreeManager.Instance.AddSkillPoints(pointsValue);
        }

        Destroy(gameObject);
    }
}