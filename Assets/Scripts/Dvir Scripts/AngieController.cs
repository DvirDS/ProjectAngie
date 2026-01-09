using UnityEngine;

public class AngieController : MonoBehaviour
{
    private MineDisplay[] hiddenMines;

    void Start()
    {
        hiddenMines = FindObjectsByType<MineDisplay>(FindObjectsSortMode.None);
    }

    void Update()
    {
        bool isSniffing = Input.GetKey(KeyCode.Q);

        foreach (MineDisplay mine in hiddenMines)
        {
            if (mine != null)
            {
                mine.SetSmellVisible(isSniffing);
            }
        }
    }
}