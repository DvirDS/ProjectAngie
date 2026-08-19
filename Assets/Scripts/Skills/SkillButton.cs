using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButton : MonoBehaviour
{
    private static readonly Color DefaultPurchasedColor = new Color(0.396f, 0.012f, 0.604f);
    private static readonly Color DefaultNotPurchasedColor = new Color(0.031f, 0.243f, 0.765f);
    private static readonly Color PurchasedIconColor = new Color(0.6f, 0.6f, 0.6f, 1f);
    private static readonly Color LockedIconColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    private const int NoPrerequisitesLength = 0;

    public Skill skillData;
    public Image skillIcon;
    public Button myButton;
    public TextMeshProUGUI nameText;
    public Color isPurchasedColor = DefaultPurchasedColor;
    public Color isNotPurchasedColor = DefaultNotPurchasedColor;

    void Start()
    {
        if (skillData != null)
        {
            if (skillIcon != null) skillIcon.sprite = skillData.icon;
            if (nameText != null) nameText.text = skillData.skillName;
        }

        if (myButton != null)
            myButton.onClick.AddListener(OnSkillButtonClicked);

        UpdateVisual();
    }

    private void OnSkillButtonClicked()
    {
        if (SkillTreeManager.I != null)
        {
            SkillTreeManager.I.TryUnlockSkill(skillData, this);
        }
    }

    public void UpdateVisual()
    {
        if (skillData == null) return;

        if (nameText != null)
        {
            nameText.text = skillData.skillName;
            nameText.color = Color.black;
        }

        if (skillData.isPurchased)
        {
            if (myButton != null) myButton.interactable = false;
            if (skillIcon != null) skillIcon.color = PurchasedIconColor;
            if (myButton != null && myButton.image != null) myButton.image.color = Color.gray;
        }
        else if (CanBePurchased())
        {
            if (myButton != null) myButton.interactable = true;
            if (skillIcon != null) skillIcon.color = Color.white;
            if (myButton != null && myButton.image != null) myButton.image.color = isPurchasedColor;
        }
        else
        {
            if (myButton != null) myButton.interactable = false;
            if (skillIcon != null) skillIcon.color = LockedIconColor;
            if (myButton != null && myButton.image != null) myButton.image.color = isNotPurchasedColor;
        }
    }

    bool CanBePurchased()
    {
        if (skillData.previousSkills == null || skillData.previousSkills.Length == NoPrerequisitesLength) return true;

        foreach (Skill parent in skillData.previousSkills)
        {
            if (parent != null && !parent.isPurchased) return false;
        }
        return true;
    }
}