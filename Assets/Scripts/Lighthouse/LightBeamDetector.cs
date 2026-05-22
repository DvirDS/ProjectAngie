using UnityEngine;

public class LightBeamDetector : MonoBehaviour
{
    [SerializeField] private EnemyAI[] allSoldiers;

    private const string UnlitLayer = "Unlit";
    private bool alerted;

    private void OnTriggerEnter2D(Collider2D other)
    {
        alerted = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || alerted) return;

        SpriteRenderer playerSprite = other.GetComponent<SpriteRenderer>();
        if (playerSprite != null && playerSprite.sortingLayerName == UnlitLayer) return;

        alerted = true;
        EnemyAI closest = FindClosestEnemyToLeft(other.transform);
        closest?.Alert();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            alerted = false;
    }

    private EnemyAI FindClosestEnemyToLeft(Transform playerTransform)
    {
        EnemyAI closest = null;
        float closestDist = Mathf.Infinity;

        foreach (EnemyAI soldier in allSoldiers)
        {
            if (soldier == null) continue;

            float soldierX = soldier.transform.position.x;
            float playerX = playerTransform.position.x;

            if (soldierX >= playerX) continue;

            float dist = playerX - soldierX;
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = soldier;
            }
        }

        return closest;
    }
}