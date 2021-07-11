using Lib;

public class EndBattleState : State<CombatManager>
{
    public EndBattleState(CombatManager owner) : base(owner) {}

    public override void Enter()
    {
        owner.ShowBattleResults();
    }
}