using UnityEngine;

public class ScentLineController : MonoBehaviour
{
    [Header("Targets")]
    public Transform ownerTransform;

    [Header("Scent Vibe Settings")]
    [SerializeField] private int pointsCount = 50;
    [SerializeField] private float waveHeight = 0.2f;
    [SerializeField] private float waveSpeed = 1.5f;
    [SerializeField] private float frequency = 2.0f;

    [Header("Movement & Pulse")]
    [SerializeField] private float scrollSpeed = 0.4f;
    [SerializeField] private float pulseSpeed = 2.0f;
    [SerializeField] private float pulseIntensity = 0.3f;

    [Header("Fade Settings")]
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private float currentAlpha = 0f;
    [SerializeField] private float targetAlpha = 0f;

    private LineRenderer lineRenderer;
    private Material scentMaterial;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = true; 
        lineRenderer.positionCount = pointsCount;

        if (lineRenderer.material != null)
        {
            scentMaterial = lineRenderer.material;
            SetMaterialAlpha(0f);
        }
    }

    public void UpdateLine(bool active)
    {
        targetAlpha = active ? 1f : 0f;

        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
        SetMaterialAlpha(currentAlpha);

        if (currentAlpha <= 0.01f)
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

        for (int i = 0; i < pointsCount; i++)
        {
            float t = (float)i / (pointsCount - 1);
            Vector3 pointPos = Vector3.Lerp(startPos, endPos, t);

            float pulseFrequency = Mathf.Sin(Time.time * pulseSpeed) * 0.15f;
            float wavePhase = (t * totalDistance) * (frequency + pulseFrequency);

            float movement = Mathf.Sin(Time.time * waveSpeed + wavePhase) * waveHeight;

            pointPos.y += movement;
            pointPos.x += movement * 0.5f;

            lineRenderer.SetPosition(i, pointPos);
        }
    }

    void AnimateScentMovement()
    {
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
        float offset = (Time.time * scrollSpeed) + pulse;
        scentMaterial.mainTextureOffset = new Vector2(-offset, 0);
    }
}