using UnityEngine;
using TMPro;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager instance;

    [Header("UI References")]
    public GameObject skillTreeWindow;
    public TextMeshProUGUI pointsText;
    public SkillButton[] allSkillButtons;

    [Header("References")]
    [SerializeField] private HealthDrainSystem playerHealth;
    [SerializeField] private PlayerInputReader inputReader;

    [Header("Data")]
    public int playerSkillPoints = 10;

    private void Awake()
    {
        if (instance == null) instance = this;
        ResetSkills();
    }

    private void OnEnable()
    {
        if (inputReader != null)
        {
            inputReader.OnSkillMenuPressed += ToggleWindow;
        }
    }

    private void OnDisable()
    {
        if (inputReader != null)
        {
            inputReader.OnSkillMenuPressed -= ToggleWindow;
        }
    }

    private void Start()
    {
        UpdateUI();

        if (skillTreeWindow != null)
            skillTreeWindow.SetActive(false);
    }

    private void ToggleWindow()
    {
        if (GameManager.I == null) return;
        if (GameManager.I.State != GameManager.GameState.Play &&
            GameManager.I.State != GameManager.GameState.Pause) return;

        if (skillTreeWindow == null) return;

        if (GameManager.I.State == GameManager.GameState.Pause)
            GameManager.I.ResumeGame();
        else
            GameManager.I.PauseGame();
    }

    private void ResetSkills()
    {
        if (allSkillButtons == null) return;

        foreach (SkillButton btn in allSkillButtons)
        {
            if (btn != null && btn.skillData != null)
            {
                btn.skillData.isPurchased = false;
                btn.skillData.isUnlocked = (btn.skillData.previousSkills.Length == 0);
            }
        }
    }

    public void TryUnlockSkill(Skill skill, SkillButton buttonRef)
    {
        if (skill.isPurchased) return;
        if (playerSkillPoints < skill.cost) return;

        foreach (Skill parentSkill in skill.previousSkills)
        {
            if (!parentSkill.isPurchased) return;
        }

        playerSkillPoints -= skill.cost;
        skill.isPurchased = true;
        skill.isUnlocked = true;

        if (skill.skillName == "HP Upgrade")
        {
            if (playerHealth != null)
                playerHealth.ApplyHpUpgrade();
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (pointsText != null)
            pointsText.text = "Points: " + playerSkillPoints;

        if (allSkillButtons != null)
        {
            foreach (SkillButton btn in allSkillButtons)
            {
                if (btn != null)
                    btn.UpdateVisual();
            }
        }
    }

    public void AddSkillPoints(int amount)
    {
        playerSkillPoints += amount;
        UpdateUI();
    }
}