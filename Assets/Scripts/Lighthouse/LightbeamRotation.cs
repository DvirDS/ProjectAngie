using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightbeamRotation : MonoBehaviour
{
    [Header("Motion")]
    [SerializeField] float maxLightbeamAngle = 65.0f;
    [SerializeField] float speed = 1.0f;
    [SerializeField] float switchAngle = 64f;

    // Ground/cave-wall/etc. sprites that should get an extra highlight while the beam is "in
    // front" of them, without ever going dark otherwise - toggled directly between "Unlit" and
    // "Default" rather than via the beam's own target-layer list, since "Unlit" is also where
    // things that must NEVER react to the beam (e.g. the cave background) permanently live.
    // Toggling "Unlit" itself on the light would sweep those over too - this list keeps the two
    // concerns separate.
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
        transform.rotation = Quaternion.Euler(0, 0, -180 + angle);

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
