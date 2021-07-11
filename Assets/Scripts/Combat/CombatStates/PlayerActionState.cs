using Lib;

public class PlayerActionState : State<CombatManager>
{
    public PlayerActionState(CombatManager owner) : base(owner) {}
    
    public override void Enter()
    {
        owner.ShowActionMenu();
    }
    
    public override void Update()
    {
        
    }

    public override void Exit()
    {
        owner.HideActionMenu();
    }
}