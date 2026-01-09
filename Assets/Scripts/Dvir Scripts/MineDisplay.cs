using UnityEngine;

public class MineDisplay : MonoBehaviour
{
    public GameObject redAuraObject;

    void Start()
    {
 
        if (redAuraObject != null)
        {
            redAuraObject.SetActive(false);
        }
    }

    public void SetSmellVisible(bool canSmell)
    {
        if (redAuraObject != null)
        {

            redAuraObject.SetActive(canSmell);
        }
    }
}