using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textElement;
    [SerializeField] private float moveSpeed = 100f; 
    [SerializeField] private float fadeDuration = 0.8f;

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
        float elapsed = 0f;
        Color startColor = textElement.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            rectTransform.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            textElement.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        Destroy(gameObject);
    }
}