using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ActionType
{
    Attack,
    Heal,
    Buff,
    Block
}

[CreateAssetMenu(fileName = "new_skill", menuName = "Unit skill", order = 0)]
public class UnitSkill : ScriptableObject
{
    public ActionType skill;
    public Sprite icon;
    public bool needTarget;
    public List<UnitSide> targetTypes;

    public string animationTrigger;
    public int damage;
    public int heal;
}