using UnityEngine;

public class TutorialOutroTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TutorialManager.instance.ShowOutro();

            Destroy(gameObject);
        }
    }
}