using UnityEngine;

/// <summary>
/// While Angie is hidden in a DarkZone, none of her colliders should physically interact
/// with an enemy's colliders at all - not the solid body (which otherwise pins her in
/// place, since a rock's collider is a trigger and never actually blocks a soldier's
/// path), and not their separate damage trigger either (a soldier that hasn't noticed her
/// shouldn't still land hits just by walking through her). Whether she's "found" is
/// entirely up to the detection logic (already blocked via IsInDarkZone in
/// EnemyAI/LightBeamDetector), not the physics solver.
/// </summary>
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
        if (playerStealth != null) playerStealth.OnDarkZoneChanged += HandleDarkZoneChanged;
    }

    private void OnDisable()
    {
        if (playerStealth == null) return;

        playerStealth.OnDarkZoneChanged -= HandleDarkZoneChanged;

        // Never leave collisions ignored if this object goes away mid-hide
        // (e.g. the room unloads while Angie is still tucked behind a rock).
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
