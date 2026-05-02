using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SceneFade : MonoBehaviour
{
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
        Color startColor = new(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 1);
        Color targetColor = new(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 0);

        yield return fadeCoroutine(startColor, targetColor, duration);
        gameObject.SetActive(false);
    }

    public IEnumerator FadeOutCoroutine(float duration)
    {
        Color startColor = new(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 0);
        Color targetColor = new(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 1);

        gameObject.SetActive(true);
        yield return fadeCoroutine(startColor, targetColor, duration);
    }

    private IEnumerator fadeCoroutine(Color startColor, Color targetColor, float duration)
    {
        float elapsedTime = 0;
        float elapsedPercentag = 0;

        while (elapsedPercentag < 1)
        {
            elapsedPercentag = elapsedTime / duration;
            fadeScreen.color = Color.Lerp(startColor, targetColor, elapsedPercentag);

            yield return null;
            elapsedTime += Time.deltaTime;
        }
    }
}
