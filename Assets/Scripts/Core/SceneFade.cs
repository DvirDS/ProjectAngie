using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SceneFade : MonoBehaviour
{
    private const float AlphaOpaque = 1f;
    private const float AlphaTransparent = 0f;
    private const float InitialElapsed = 0f;
    private const float CompletePercentage = 1f;

    private Image fadeScreen;

    private void Awake()
    {
        fadeScreen = GetComponent<Image>();
        fadeScreen.raycastTarget = false;
    }

    public IEnumerator HoldColorDuration(Color color, float holdDuration)
    {
        fadeScreen.color = color;
        yield return new WaitForSecondsRealtime(holdDuration);
    }

    public IEnumerator FadeInCoroutine(float duration)
    {
        Color startColor = new(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, AlphaOpaque);
        Color targetColor = new(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, AlphaTransparent);

        yield return fadeCoroutine(startColor, targetColor, duration);
        gameObject.SetActive(false);
    }

    public IEnumerator FadeOutCoroutine(float duration)
    {
        Color startColor = new(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, AlphaTransparent);
        Color targetColor = new(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, AlphaOpaque);

        gameObject.SetActive(true);
        yield return fadeCoroutine(startColor, targetColor, duration);
    }

    private IEnumerator fadeCoroutine(Color startColor, Color targetColor, float duration)
    {
        float elapsedTime = InitialElapsed;
        float elapsedPercentage = InitialElapsed;

        while (elapsedPercentage < CompletePercentage)
        {
            elapsedPercentage = elapsedTime / duration;
            fadeScreen.color = Color.Lerp(startColor, targetColor, elapsedPercentage);

            yield return null;
            elapsedTime += Time.unscaledDeltaTime;
        }
    }
}