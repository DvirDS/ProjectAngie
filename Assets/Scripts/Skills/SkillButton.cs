using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButton : MonoBehaviour
{
    public Skill skillData; 
    public Image skillIcon;
    public Button myButton;
    public Image lockOverlay;

    [SerializeField] private SkillTreeManager skillTreeManager;

    void Start()
    {
        if (skillIcon != null && skillData != null)
            skillIcon.sprite = skillData.icon;

        myButton.onClick.AddListener(() => SkillTreeManager.instance.TryUnlockSkill(skillData, this));
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (skillData.isPurchased)
        {
            lockOverlay.gameObject.SetActive(false);
            myButton.interactable = false;

            if (skillIcon != null) skillIcon.color = Color.white;
        }
        else if (CanBePurchased())
        {
            lockOverlay.gameObject.SetActive(false);
            myButton.interactable = true;

            if (skillIcon != null) skillIcon.color = Color.gray;
        }
        else
        {
            lockOverlay.gameObject.SetActive(true);
            myButton.interactable = false;
            if (skillIcon != null) skillIcon.color = Color.gray;
        }
    }

    bool CanBePurchased()
    {
        foreach (Skill parent in skillData.previousSkills)
        {
            if (!parent.isPurchased) return false;
        }
        return true;
    }
}