using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButton : MonoBehaviour
{
    public Skill skillData; // גרור לכאן את הקובץ שיצרת בשלב 1
    public Image skillIcon;
    public Button myButton;
    public Image lockOverlay; // תמונה כהה/מנעול שמופיעה כשהסקיל נעול

    void Start()
    {
        // תוודא שזה לא בהערה
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

            // תוודא שזה לא בהערה
            if (skillIcon != null) skillIcon.color = Color.white;
        }
        else if (CanBePurchased())
        {
            lockOverlay.gameObject.SetActive(false);
            myButton.interactable = true;

            // תוודא שזה לא בהערה
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