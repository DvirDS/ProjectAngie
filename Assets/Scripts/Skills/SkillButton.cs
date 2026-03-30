using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButton : MonoBehaviour
{
    public Skill skillData;
    public Image skillIcon; // התמונה של הסקיל (הכלב וכו')
    public Button myButton; // הכפתור עצמו (הרקע)
    public TextMeshProUGUI nameText; // השם של הסקיל

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

        // עדכון הטקסט ושינוי הצבע שלו לשחור
        if (nameText != null)
        {
            nameText.text = skillData.skillName;
            nameText.color = Color.black;
        }

        // מצב 1: כבר קנינו - אפור
        if (skillData.isPurchased)
        {
            if (myButton != null) myButton.interactable = false;
            if (skillIcon != null) skillIcon.color = new Color(0.6f, 0.6f, 0.6f, 1f);
            if (myButton != null && myButton.image != null) myButton.image.color = Color.gray;
        }
        // מצב 2: אפשר לקנות - ירוק
        else if (CanBePurchased())
        {
            if (myButton != null) myButton.interactable = true;
            if (skillIcon != null) skillIcon.color = Color.white;
            if (myButton != null && myButton.image != null) myButton.image.color = Color.green;
        }
        // מצב 3: נעול - אדום אטום ועמוק יותר
        else
        {
            if (myButton != null) myButton.interactable = false;
            if (skillIcon != null) skillIcon.color = new Color(0.3f, 0.3f, 0.3f, 1f);

            // יצרתי פה אדום כהה ואטום (המספר האחרון 1f אומר שהוא 100% אטום ולא שקוף)
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