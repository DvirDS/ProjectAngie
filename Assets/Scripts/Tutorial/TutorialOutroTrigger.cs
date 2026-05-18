using UnityEngine;

public class TutorialOutroTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Outro trigger hit!");

            if (TutorialManager.instance == null)
            {
                Debug.LogError("TutorialManager.instance is null!");
                return;
            }

            TutorialManager.instance.ShowOutro();
            Destroy(gameObject);
        }
    }
}