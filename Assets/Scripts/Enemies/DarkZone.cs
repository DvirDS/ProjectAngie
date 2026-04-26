using UnityEngine;

public class DarkZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStealth stealth = other.GetComponent<PlayerStealth>();
            if (stealth != null) stealth.IsInDarkZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStealth stealth = other.GetComponent<PlayerStealth>();
            if (stealth != null) stealth.IsInDarkZone = false;
        }
    }
}