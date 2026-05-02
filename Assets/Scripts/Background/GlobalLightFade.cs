using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class GlobalLightFade : MonoBehaviour
{
    private Light2D globalLight;
    [SerializeField] float endIntensity = 0.005f;
    [SerializeField] float duration = 3f;

    void Awake()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeToDark());
        }
    }

    IEnumerator FadeToDark()
    {
        float startIntesity = globalLight.intensity;
        Color endColor = Color.darkBlue;
        
        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            globalLight.intensity = Mathf.Lerp(startIntesity, endIntensity, time / duration);
            yield return null;
        }
        globalLight.intensity = endIntensity;
    }
}
