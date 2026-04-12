using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "TutorialSO", menuName = "Scriptable Objects/TutorialSO")]
public class TutorialSO : ScriptableObject
{
    [SerializeField] private string tutorialTitle;
    [TextArea(1, 3)]
    [SerializeField] public string tutorialDescription;
    [SerializeField] public InputActionReference keyInput;
}
