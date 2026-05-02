using UnityEngine;

[DefaultExecutionOrder(500)]
public class ParallaxLayer : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float parallaxEffectMultiplier = 0.3f;

    private Transform cameraTransform;
    private Vector3 startPosition;
    private Vector3 cameraStartPosition;

    private void OnEnable()
    {
        TryFindCamera();
    }

    private void LateUpdate()
    {
        if (cameraTransform == null)
        {
            TryFindCamera();
            return;
        }

        Vector3 cameraDelta = cameraTransform.position - cameraStartPosition;

        transform.position = startPosition + new Vector3(
            cameraDelta.x * parallaxEffectMultiplier,
            0f,
            0f
        );
    }

    private void TryFindCamera()
    {
        Camera cam = Camera.main;

        if (cam == null)
            return;

        cameraTransform = cam.transform;

        startPosition = transform.position;
        cameraStartPosition = cameraTransform.position;
    }
}