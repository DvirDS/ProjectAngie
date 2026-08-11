using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(500)]
public class ParallaxLayer : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float parallaxEffectMultiplier = 0.3f;

    private Transform cameraTransform;
    private Vector3 startPosition;
    private Vector3 cameraStartPosition;

    private static readonly Dictionary<string, Vector3> sceneCameraAnchors = new();

    private bool hasAnchored;

    public static void ResetAnchors() => sceneCameraAnchors.Clear();

    private void OnEnable()
    {
        TryFindCamera();
        Events.onUnloadCreateBounds += OnRoomActivated;
    }

    private void OnDisable()
    {
        Events.onUnloadCreateBounds -= OnRoomActivated;
    }

    private void OnRoomActivated(string sceneName)
    {
        if (sceneName != gameObject.scene.name) return;
        if (cameraTransform == null) return;
        Anchor();
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
        Anchor();
    }

    private void Anchor()
    {
        if (hasAnchored) return;
        hasAnchored = true;

        startPosition = transform.position;

        string sceneName = gameObject.scene.name;
        if (sceneCameraAnchors.TryGetValue(sceneName, out Vector3 cachedCameraPosition))
        {
            cameraStartPosition = cachedCameraPosition;
        }
        else
        {
            cameraStartPosition = cameraTransform.position;
            sceneCameraAnchors[sceneName] = cameraStartPosition;
        }
    }
}