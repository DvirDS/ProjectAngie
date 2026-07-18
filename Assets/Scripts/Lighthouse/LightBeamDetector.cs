using UnityEngine;

public class LightBeamDetector : MonoBehaviour
{
    [SerializeField] private EnemyAI soldier;

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

        PlayerStealth stealth = other.GetComponent<PlayerStealth>();
        if (stealth != null && stealth.IsInDarkZone) return;

        alerted = true;
        soldier?.Alert();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            alerted = false;
    }
}