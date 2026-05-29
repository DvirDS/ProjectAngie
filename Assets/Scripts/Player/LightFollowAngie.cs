using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFollowAngie : MonoBehaviour
{
    public static Light2D Instance { get; private set; }
    public static float DefaultIntensity { get; private set; }

    private void Awake()
    {
        Instance = GetComponent<Light2D>();
        DefaultIntensity = Instance.intensity;
    }
}
