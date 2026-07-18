using UnityEngine;

public class LoseSkill : TutorialTrigger
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        SkillTreeManager.I.ResetSkills();
        TutorialManager.I.Show(tutorial.tutorialDescription, null);
    }
}