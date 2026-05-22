using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private int pointsValue = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CollectCoin();
        }
    }

    private void CollectCoin()
    {
        if (SkillTreeManager.instance != null)
        {
            SkillTreeManager.instance.AddSkillPoints(pointsValue);
        }

        Destroy(gameObject);
    }
}