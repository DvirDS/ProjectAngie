using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightbeamRotation : MonoBehaviour
{
    private const float DefaultMaxAngle = 65.0f;
    private const float DefaultSpeed = 1.0f;
    private const float DefaultSwitchAngle = 64f;
    private const float BaseRotationAngle = -180f;
    private const float ZeroEulerAxis = 0f;

    [Header("Motion")]
    [SerializeField] float maxLightbeamAngle = DefaultMaxAngle;
    [SerializeField] float speed = DefaultSpeed;
    [SerializeField] float switchAngle = DefaultSwitchAngle;
    [SerializeField] private List<SpriteRenderer> highlightSprites;

    private const string NoBeamLightLayer = "NoBeamLight";
    private const string UnlitLayer = "Unlit";
    private const string DefaultLayer = "Default";

    private Light2D beamLight;
    private bool lightBackwards = true;

    private void Awake()
    {
        beamLight = GetComponent<Light2D>();
    }

    void FixedUpdate()
    {
        float angle = maxLightbeamAngle * Mathf.Sin(Time.time * speed);
        transform.rotation = Quaternion.Euler(ZeroEulerAxis, ZeroEulerAxis, BaseRotationAngle + angle);

        if (angle > switchAngle && lightBackwards)
        {
            beamLight.RemoveTargetSortingLayer(NoBeamLightLayer);
            SetHighlighted(false);
            lightBackwards = false;
        }
        else if (angle < -switchAngle && !lightBackwards)
        {
            beamLight.AddTargetSortingLayer(NoBeamLightLayer);
            SetHighlighted(true);
            lightBackwards = true;
        }
    }

    private void SetHighlighted(bool highlighted)
    {
        if (highlightSprites == null) return;

        string layerName = highlighted ? DefaultLayer : UnlitLayer;
        foreach (SpriteRenderer sprite in highlightSprites)
        {
            if (sprite != null) sprite.sortingLayerName = layerName;
        }
    }
}