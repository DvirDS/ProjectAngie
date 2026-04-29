using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;

    [Header("UI References")]
    public GameObject tooltipWindow;
    public TextMeshProUGUI tooltipText;

    [Header("Settings")]
    public Vector2 offset = new Vector2(15f, -15f);

    private void Awake()
    {
        if (instance == null) instance = this;
        tooltipWindow.SetActive(false);
    }

    private void Update()
    {
        if (tooltipWindow != null && tooltipWindow.activeSelf)
        {
            if (GameManager.I != null && GameManager.I.State != GameManager.GameState.SkillTree)
            {
                HideTooltip();
                return;
            }

            if (Mouse.current != null)
            {
                RectTransform rectTransform = tooltipWindow.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    Vector2 mousePos = Mouse.current.position.ReadValue();
                    rectTransform.position = mousePos + offset;
                }
            }
        }
    }

    public void ShowTooltip(Skill skillData)
    {
        if (skillData == null) return;

        tooltipText.text = $"{skillData.skillDescription}\n<b>Key:</b> {skillData.activationKey}\n<b>Cost:</b> {skillData.cost}";
        tooltipWindow.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipWindow.SetActive(false);
    }
}