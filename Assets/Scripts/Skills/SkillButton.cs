using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButton : MonoBehaviour
{
    public Skill skillData;
    public Image skillIcon;
    public Button myButton; 
    public TextMeshProUGUI nameText; 

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
        if (SkillTreeManager.instance != null)
        {
            SkillTreeManager.instance.TryUnlockSkill(skillData, this);
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
            if (skillIcon != null) skillIcon.color = new Color(0.6f, 0.6f, 0.6f, 1f);
            if (myButton != null && myButton.image != null) myButton.image.color = Color.gray;
        }
        else if (CanBePurchased())
        {
            if (myButton != null) myButton.interactable = true;
            if (skillIcon != null) skillIcon.color = Color.white;
            if (myButton != null && myButton.image != null) myButton.image.color = Color.green;
        }
        else
        {
            if (myButton != null) myButton.interactable = false;
            if (skillIcon != null) skillIcon.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            if (myButton != null && myButton.image != null) myButton.image.color = new Color(0.7f, 0.2f, 0.2f, 1f);
        }
    }

    bool CanBePurchased()
    {
        if (skillData.previousSkills == null || skillData.previousSkills.Length == 0) return true;

        foreach (Skill parent in skillData.previousSkills)
        {
            if (parent != null && !parent.isPurchased) return false;
        }
        return true;
    }
}