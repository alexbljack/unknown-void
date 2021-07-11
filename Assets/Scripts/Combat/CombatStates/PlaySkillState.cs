using System.Collections;
using Lib;
using UnityEngine;

public class PlaySkillState : State<CombatManager>
{
    public PlaySkillState(CombatManager owner) : base(owner) {}

    public override void Enter()
    {
        var skill = owner.ActiveSkill;
        owner.ShowSkillNameBox(skill.name);
        
        owner.ActiveUnit.ResetReadyCounter();
        owner.ActiveUnit.UseSkill(skill);
        
        switch (owner.ActiveSkill.skill)
        {
            case ActionType.Attack:
                owner.TargetUnit.Hit(skill.damage);
                break;
            case ActionType.Heal:
                owner.ActiveUnit.Heal(skill.heal);
                break;
        }

        owner.StartCoroutine(WaitForAnimationEnd());
    }

    IEnumerator WaitForAnimationEnd()
    {
        yield return new WaitForSeconds(1);
        while (!owner.ActiveUnit.IsIdle() || (owner.TargetUnit && !owner.TargetUnit.IsIdle()))
        {
            yield return null;
        }
        owner.OnResume();
    }

    public override void Exit()
    {
        owner.HideSkillNameBox();
    }
}