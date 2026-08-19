using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private const float DefaultMoveSpeed = 100f;
    private const float DefaultFadeDuration = 0.8f;
    private const float InitialElapsed = 0f;
    private const float FullyOpaque = 1f;
    private const float FullyTransparent = 0f;

    [SerializeField] private TextMeshProUGUI textElement;
    [SerializeField] private float moveSpeed = DefaultMoveSpeed;
    [SerializeField] private float fadeDuration = DefaultFadeDuration;

    private RectTransform rectTransform;
    private Camera mainCamera;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    public void Initialize(string text, Vector3 worldPosition)
    {
        if (textElement != null) textElement.text = text;

        if (mainCamera != null)
        {
            Vector2 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            rectTransform.position = screenPosition;
        }

        StartCoroutine(FadeAndMoveRoutine());
    }

    private IEnumerator FadeAndMoveRoutine()
    {
        float elapsed = InitialElapsed;
        Color startColor = textElement.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            rectTransform.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;

            float alpha = Mathf.Lerp(FullyOpaque, FullyTransparent, elapsed / fadeDuration);
            textElement.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        Destroy(gameObject);
    }
}