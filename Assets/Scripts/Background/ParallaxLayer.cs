using UnityEngine;

[DefaultExecutionOrder(50)]
public class ParallaxLayer : MonoBehaviour
{
    [Header("Camera Reference")]
    public Transform mainCamera;

    [Header("Parallax Settings")]
    [Tooltip("0 = לא זז (קרוב), 1 = זז בדיוק עם המצלמה (שמיים רחוקים)")]
    [Range(0f, 1f)]
    public float parallaxEffectMultiplier;

    private Vector3 lastCameraPosition;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main.transform;
        }

        lastCameraPosition = mainCamera.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = mainCamera.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier, 0, 0);
        lastCameraPosition = mainCamera.position;
    }
}