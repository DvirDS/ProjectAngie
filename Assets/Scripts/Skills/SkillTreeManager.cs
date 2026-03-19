using UnityEngine;
using TMPro;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager instance;

    [Header("UI References")]
    public GameObject skillTreeWindow;
    public TextMeshProUGUI pointsText;
    public SkillButton[] allSkillButtons;

    [Header("Data")]
    public int playerSkillPoints = 10;

    private InputSystem inputActions;

    private void Awake()
    {
        if (instance == null) instance = this;

        inputActions = new InputSystem();

        ResetSkills();
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Start()
    {
        UpdateUI();

        if (skillTreeWindow != null)
            skillTreeWindow.SetActive(false);
    }

    private void Update()
    {
        if (inputActions.Player.SkillMenu.triggered)
            ToggleWindow();
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

    private void ToggleWindow()
    {
        if (skillTreeWindow == null) return;

        bool isActive = !skillTreeWindow.activeSelf;
        skillTreeWindow.SetActive(isActive);

        Time.timeScale = isActive ? 0f : 1f;

        if (isActive)
            UpdateUI();
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
            HealthDrainSystem health = FindFirstObjectByType<HealthDrainSystem>();
            if (health != null)
                health.ApplyHpUpgrade();
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