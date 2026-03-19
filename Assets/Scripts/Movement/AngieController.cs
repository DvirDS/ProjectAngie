using UnityEngine;

public class AngieController : MonoBehaviour
{
    [Header("Skills")]
    [SerializeField] private Skill sniffSkill;

    private MineController[] allMines;
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
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        RefreshMines();
    }

    private void Start()
    {
        RefreshMines();
    }

    private void RefreshMines()
    {
        allMines = FindObjectsByType<MineController>(FindObjectsSortMode.None);
    }

    private void Update()
    {
        bool isKeyDown = inputActions.Player.Sniff.IsPressed();
        bool hasSkill = sniffSkill != null && sniffSkill.isPurchased;
        bool isSniffing = isKeyDown && hasSkill;

        if (scentLine != null) scentLine.UpdateLine(isSniffing);

        foreach (MineController mine in allMines)
        {
            if (mine != null)
                mine.SetSmellVisible(isSniffing);
        }
    }
}