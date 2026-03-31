using UnityEngine;
using System;

public class PlayerSniff : MonoBehaviour
{
    public static event Action<bool> OnNormalSniff;
    public static event Action<bool> OnSuperSniff;

    [Header("Skill Connection")]
    [SerializeField] private Skill _superSniffSkillData;

    [Header("References")]
    [SerializeField] private PlayerInputReader input;

    private bool isSniffing = false;

    private void Update()
    {
        bool isHoldingButton = input.SniffHeld;

        if (isHoldingButton != isSniffing)
        {
            isSniffing = isHoldingButton;

            OnNormalSniff?.Invoke(isSniffing);

            bool hasSuperSniff = _superSniffSkillData != null && _superSniffSkillData.isPurchased;

            if (hasSuperSniff)
            {
                OnSuperSniff?.Invoke(isSniffing);
            }
            else
            {
                OnSuperSniff?.Invoke(false);
            }
        }
    }
}