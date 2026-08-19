using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AngiesLightZone : MonoBehaviour
{
    private const float DefaultTargetIntensity = 2f;
    private const float DefaultTransitionSpeed = 2f;
    private const float UnassignedIntensity = -1f;
    private const float IntensityThreshold = 0f;
    private const float LerpTolerance = 0.01f;

    [SerializeField] private float targetIntensity = DefaultTargetIntensity;
    [SerializeField] private float transitionSpeed = DefaultTransitionSpeed;

    private static float defaultIntensity = UnassignedIntensity;
    private bool playerInside;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (defaultIntensity < IntensityThreshold)
            defaultIntensity = LightFollowAngie.DefaultIntensity;

        playerInside = true;
        StopAllCoroutines();
        StartCoroutine(LerpLight(targetIntensity));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        ResetLight();
    }

    private void OnDisable()
    {
        if (playerInside) ResetLight();
    }

    private void ResetLight()
    {
        StopAllCoroutines();

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(LerpLight(defaultIntensity));
        }
        else if (LightFollowAngie.Instance != null)
        {
            LightFollowAngie.Instance.intensity = defaultIntensity;
        }
    }

    private System.Collections.IEnumerator LerpLight(float target)
    {
        Light2D light = LightFollowAngie.Instance;
        while (Mathf.Abs(light.intensity - target) > LerpTolerance)
        {
            light.intensity = Mathf.Lerp(light.intensity, target, Time.deltaTime * transitionSpeed);
            yield return null;
        }
        light.intensity = target;
    }
}