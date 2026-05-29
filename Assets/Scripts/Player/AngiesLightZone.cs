using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AngiesLightZone : MonoBehaviour
{
    [SerializeField] private float targetIntensity = 2f;
    [SerializeField] private float transitionSpeed = 2f;

    private static float defaultIntensity = -1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (defaultIntensity < 0f)
            defaultIntensity = LightFollowAngie.DefaultIntensity;

        StopAllCoroutines();
        StartCoroutine(LerpLight(targetIntensity));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        StopAllCoroutines();
        StartCoroutine(LerpLight(defaultIntensity));
    }

    private System.Collections.IEnumerator LerpLight(float target)
    {
        Light2D light = LightFollowAngie.Instance;
        while (Mathf.Abs(light.intensity - target) > 0.01f)
        {
            light.intensity = Mathf.Lerp(light.intensity, target, Time.deltaTime * transitionSpeed);
            yield return null;
        }
        light.intensity = target;
    }
}
