using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "TutorialSO", menuName = "Scriptable Objects/TutorialSO")]
public class TutorialSO : ScriptableObject
{
    private const int MinTextAreaLines = 1;
    private const int MaxTextAreaLines = 3;

    [SerializeField] private string tutorialTitle;
    [TextArea(MinTextAreaLines, MaxTextAreaLines)]
    [SerializeField] public string tutorialDescription;
    [SerializeField] public InputActionReference keyInput;
}