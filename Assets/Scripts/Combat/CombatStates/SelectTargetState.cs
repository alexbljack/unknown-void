using Lib;
using UnityEngine;

internal class SelectTargetState : State<CombatManager>
{
    public SelectTargetState(CombatManager owner) : base(owner) {}

    public override void Update()
    {
        if (Helpers.ClickedOnScreen())
        {
            var (_, obj) = Helpers.GetClickedTarget2D(Camera.main);
            if (obj == null)
            {
                owner.ResetSelection();
                return;
            }
            
            var target = obj.GetComponent<Avatar>();
            if (target != null && target.CanBeSkillTarget(owner.ActiveSkill))
            {
                owner.OnSelectTarget(target);
            }
        }
    }
}