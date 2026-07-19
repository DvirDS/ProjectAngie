using UnityEngine;

public class TutorialOutroTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (TutorialManager.I == null)
            {
                Debug.LogError("TutorialManager.I is null!");
                return;
            }

            TutorialManager.I.ShowOutro();
            Destroy(gameObject);
        }
    }
}