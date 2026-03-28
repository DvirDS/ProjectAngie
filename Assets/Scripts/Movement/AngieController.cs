using UnityEngine;

public class AngieController : MonoBehaviour
{
    [Header("Skills")]
    [SerializeField] private Skill sniffSkill;

    [Header("Runtime Set")]
    [SerializeField] private MineRuntimeSet mineSet;

    private ScentLineController scentLine;
    private InputSystem inputActions;

    private void Awake()
    {
        inputActions = new InputSystem();
        scentLine = GetComponentInChildren<ScentLineController>();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }


    private void Update()
    {
        bool isKeyDown = inputActions.Player.Sniff.IsPressed();
        bool hasSkill = sniffSkill != null && sniffSkill.isPurchased;
        bool isSniffing = isKeyDown && hasSkill;

        if (scentLine != null) scentLine.UpdateLine(isSniffing);

        foreach (MineController mine in mineSet.items)
            if (mine != null)
                mine.SetSmellVisible(isSniffing);
    }
}