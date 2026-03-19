using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class GlobalLightFade : MonoBehaviour
{
    private Light2D globalLight;
    private SpriteRenderer background;
    [SerializeField] float endIntensity = 0.005f;
    [SerializeField] float duration = 7f;

    void Awake()
    {
        globalLight = FindFirstObjectByType<Light2D>();
        if (globalLight == null)
        {
            Debug.LogError("No Light2D component found on this GameObject.");
        }

        background = GameObject.FindGameObjectWithTag("Background").GetComponent<SpriteRenderer>();
        if (background == null)
        {
            Debug.LogError("No SpriteRenderer with tag 'Background' found in the scene.");
        }
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
        float startIntesity = 1f;
        Color startColor = background.color;
        Color endColor = Color.darkBlue;
        
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            background.color = Color.Lerp(startColor, endColor, time / duration);
            globalLight.intensity = Mathf.Lerp(startIntesity, endIntensity, time / duration);
            yield return null;
        }
        globalLight.intensity = endIntensity;
    }
}
