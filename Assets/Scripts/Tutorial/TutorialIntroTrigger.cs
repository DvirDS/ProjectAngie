using UnityEngine;

public class TutorialIntroTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TutorialManager.I.ShowIntro();

            Destroy(gameObject);
        }
    }
}