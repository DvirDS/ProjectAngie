using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager instance;

    public Transform respawnPoint { get; set; }

    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private float holdDuration = 1f;
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private Transform player;

    private void Awake()
    {
        if (instance != null && instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        instance = this;
    }

    public void Respawn()
    {
        StartCoroutine(RespawnRoutine());
    }

    public IEnumerator FadeIn()
    {
        yield return StartCoroutine(Fade(1f, 0f));
    }

    public void SetFadeAlpha(float alpha)
    {
        fadePanel.gameObject.SetActive(true);
        fadePanel.alpha = alpha;
    }

    private IEnumerator RespawnRoutine()
    {
        yield return StartCoroutine(Fade(0f, 1f));

        player.position = respawnPoint.position;
        yield return new WaitForSecondsRealtime(holdDuration);

        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float from, float to)
    {
        fadePanel.gameObject.SetActive(true);
        fadePanel.alpha = from;

        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            fadePanel.alpha = Mathf.Lerp(from, to, time / fadeDuration);
            yield return null;
        }

        fadePanel.alpha = to;

        if (to == 0f)
        {
            fadePanel.blocksRaycasts = false;
            fadePanel.gameObject.SetActive(false);
        }
    }
}