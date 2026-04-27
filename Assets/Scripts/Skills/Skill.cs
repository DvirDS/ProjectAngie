using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill Tree/Skill")]
public class Skill : ScriptableObject
{
    public string skillName; 
    public int cost;         
    public Sprite icon;
    public Skill[] previousSkills;
    public Skill[] nextSkills;

    [HideInInspector] public bool isUnlocked; 
    [HideInInspector] public bool isPurchased; 
}