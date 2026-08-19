using UnityEngine;

public class ScentLineController : MonoBehaviour
{
    private const int DefaultPointsCount = 50;
    private const float DefaultWaveHeight = 0.2f;
    private const float DefaultWaveSpeed = 1.5f;
    private const float DefaultFrequency = 2.0f;
    private const float DefaultScrollSpeed = 0.4f;
    private const float DefaultPulseSpeed = 2.0f;
    private const float DefaultPulseIntensity = 0.3f;
    private const float DefaultFadeSpeed = 5f;
    private const float InitialAlpha = 0f;
    private const float ActiveAlpha = 1f;
    private const float InactiveAlpha = 0f;
    private const float AlphaVisibilityThreshold = 0.01f;
    private const int FirstPointIndex = 0;
    private const int PointsCountOffset = 1;
    private const float PulseFrequencyMultiplier = 0.15f;
    private const float HorizontalMovementFactor = 0.5f;
    private const float ZeroTextureOffsetY = 0f;

    [Header("Targets")]
    public Transform ownerTransform;

    [Header("Scent Vibe Settings")]
    [SerializeField] private int pointsCount = DefaultPointsCount;
    [SerializeField] private float waveHeight = DefaultWaveHeight;
    [SerializeField] private float waveSpeed = DefaultWaveSpeed;
    [SerializeField] private float frequency = DefaultFrequency;

    [Header("Movement & Pulse")]
    [SerializeField] private float scrollSpeed = DefaultScrollSpeed;
    [SerializeField] private float pulseSpeed = DefaultPulseSpeed;
    [SerializeField] private float pulseIntensity = DefaultPulseIntensity;

    [Header("Fade Settings")]
    [SerializeField] private float fadeSpeed = DefaultFadeSpeed;
    [SerializeField] private float currentAlpha = InitialAlpha;
    [SerializeField] private float targetAlpha = InitialAlpha;

    private LineRenderer lineRenderer;
    private Material scentMaterial;

    private bool isSniffActive = false;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
        lineRenderer.positionCount = pointsCount;

        if (lineRenderer.material != null)
        {
            scentMaterial = lineRenderer.material;
            SetMaterialAlpha(InactiveAlpha);
        }
    }

    private void OnEnable() => PlayerSniff.OnNormalSniff += HandleSniffState;
    private void OnDisable() => PlayerSniff.OnNormalSniff -= HandleSniffState;

    private void HandleSniffState(bool active)
    {
        isSniffActive = active;
    }

    private void Update()
    {
        UpdateLine(isSniffActive);
    }

    public void UpdateLine(bool active)
    {
        targetAlpha = active ? ActiveAlpha : InactiveAlpha;

        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
        SetMaterialAlpha(currentAlpha);

        if (currentAlpha <= AlphaVisibilityThreshold)
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.enabled = true;

        if (ownerTransform != null)
        {
            if (lineRenderer.positionCount != pointsCount)
            {
                lineRenderer.positionCount = pointsCount;
            }

            DrawScentTrail();
            AnimateScentMovement();
        }
    }

    void SetMaterialAlpha(float alpha)
    {
        if (scentMaterial != null)
        {
            if (scentMaterial.HasProperty("_BaseColor"))
            {
                Color color = scentMaterial.GetColor("_BaseColor");
                color.a = alpha;
                scentMaterial.SetColor("_BaseColor", color);
            }
            else if (scentMaterial.HasProperty("_Color"))
            {
                Color color = scentMaterial.GetColor("_Color");
                color.a = alpha;
                scentMaterial.SetColor("_Color", color);
            }
        }
    }

    void DrawScentTrail()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = ownerTransform.position;
        float totalDistance = Vector3.Distance(startPos, endPos);

        for (int i = FirstPointIndex; i < pointsCount; i++)
        {
            float t = (float)i / (pointsCount - PointsCountOffset);
            Vector3 pointPos = Vector3.Lerp(startPos, endPos, t);

            float pulseFrequency = Mathf.Sin(Time.time * pulseSpeed) * PulseFrequencyMultiplier;
            float wavePhase = (t * totalDistance) * (frequency + pulseFrequency);

            float movement = Mathf.Sin(Time.time * waveSpeed + wavePhase) * waveHeight;

            pointPos.y += movement;
            pointPos.x += movement * HorizontalMovementFactor;

            lineRenderer.SetPosition(i, pointPos);
        }
    }

    void AnimateScentMovement()
    {
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
        float offset = (Time.time * scrollSpeed) + pulse;
        scentMaterial.mainTextureOffset = new Vector2(-offset, ZeroTextureOffsetY);
    }
}