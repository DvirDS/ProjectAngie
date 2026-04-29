using UnityEngine;

public class LoseSkill : MonoBehaviour
{
    [SerializeField] private string LoseSkillText;
    [SerializeField] private Skill skillSO;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        TutorialManager.instance.Show(LoseSkillText, null);
        skillSO.isPurchased = false;
        foreach (Skill skill in skillSO.nextSkills)
        {
            skill.isPurchased = false;
            skill.isUnlocked = false;
        }
        SkillTreeManager.instance.UpdateUI();
    }
}