using UnityEngine;

public class PlayerHiddenCollision : MonoBehaviour
{
    private PlayerStealth playerStealth;
    private Collider2D[] playerColliders;

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("PlayerHiddenCollision: no Player found in scene - disabling.");
            enabled = false;
            return;
        }

        playerStealth = playerObj.GetComponent<PlayerStealth>();
        playerColliders = playerObj.GetComponents<Collider2D>();
    }

    private void OnEnable()
    {
        if (playerStealth == null) return;

        playerStealth.OnDarkZoneChanged += HandleDarkZoneChanged;

        SetIgnoreEnemyCollisions(playerStealth.IsInDarkZone);
    }

    private void OnDisable()
    {
        if (playerStealth == null) return;

        playerStealth.OnDarkZoneChanged -= HandleDarkZoneChanged;

        if (playerStealth.IsInDarkZone) SetIgnoreEnemyCollisions(false);
    }

    private void HandleDarkZoneChanged(bool isHidden)
    {
        SetIgnoreEnemyCollisions(isHidden);
    }

    private void SetIgnoreEnemyCollisions(bool ignore)
    {
        if (playerColliders == null) return;

        EnemyAI[] enemies = Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        foreach (EnemyAI enemy in enemies)
        {
            foreach (Collider2D enemyCollider in enemy.GetComponentsInChildren<Collider2D>())
            {
                foreach (Collider2D pc in playerColliders)
                {
                    Physics2D.IgnoreCollision(pc, enemyCollider, ignore);
                }
            }
        }
    }
}
