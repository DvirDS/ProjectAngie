using UnityEngine;

public class LightBeamDetector : MonoBehaviour
{

    [SerializeField] private SoldierEnemy[] allSoldiers;
    private string unlitLayer = "Unlit";
    private bool alerted;

    private void OnTriggerEnter2D(Collider2D other)
    {
        alerted = false; 
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || alerted) 
            return;

        SpriteRenderer playerSprite = other.GetComponent<SpriteRenderer>();
        if (playerSprite.sortingLayerName == unlitLayer) 
            return;

        alerted = true;
        SoldierEnemy closest = FindClosestEnemyToLeft(other.transform);
        if (closest != null)
            closest.Alert();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            alerted = false;
    }

    private SoldierEnemy FindClosestEnemyToLeft(Transform playerTransform)
    {

        SoldierEnemy closest = null;
        float closestDist = Mathf.Infinity;

        foreach (SoldierEnemy soldier in allSoldiers)
        {
            float soldierX = soldier.transform.position.x;
            float playerX = playerTransform.position.x;

            if (soldierX >= playerX) 
                continue;

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

