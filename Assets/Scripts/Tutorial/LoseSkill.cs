using UnityEngine;

public class LoseSkill : TutorialTrigger
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        SkillTreeManager.instance.ResetSkills();
        TutorialManager.instance.Show(tutorial.tutorialDescription, null);
    }
}