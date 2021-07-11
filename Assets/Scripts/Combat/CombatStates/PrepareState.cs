using System.Linq;
using Lib;
using Unity.Collections;

public class PrepareState : State<CombatManager>
{
    public PrepareState(CombatManager owner) : base(owner) {}

    public override void Enter()
    {
        owner.CheckForBattleEnd();
        owner.ResetUnits();
        owner.HideActionMenu();
        owner.HideSkillNameBox();
    }

    public override void Update()
    {
        if (owner.ActiveUnit == null)
        {
            foreach (var unit in owner.Units)
            {
                if (unit.IsReady)
                {
                    owner.OnUnitReady(unit);
                    return;
                }

                if (unit.IsAlive)
                {
                    unit.Prepare();
                }
            }
        }
    }
}