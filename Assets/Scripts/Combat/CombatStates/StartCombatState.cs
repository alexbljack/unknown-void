using System.Collections.Generic;
using System.Linq;
using Lib;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartCombatState : State<CombatManager>
{
    public StartCombatState(CombatManager owner) : base(owner) {}

    public override void Enter()
    {
        InitCombat();
        SetAvatars();
        owner.HideBattleResults();
        owner.OnResume();
    }

    void SetAvatars()
    {
        owner.InitAvatars(Object.FindObjectsOfType<Avatar>().ToList());
    }
    
    void InitCombat()
    {
        SpawnUnits(LevelManager.Instance.players, UnitSide.Player);
        SpawnUnits(LevelManager.Instance.facedEnemies, UnitSide.Enemy);
    }

    void SpawnUnits<T>(List<T> units, UnitSide side) where T : BaseUnit
    {
        var count = units.Count;
        var points = Object.FindObjectsOfType<SpawnPoint>().ToList();
        var actualPoints = points.FindAll(
            p => p.unitsCount == count && p.side == side
        );
        
        for (var i=0; i < count; i++)
        {
            var unit = units[i];
            var point = actualPoints[i];
            var obj = Object.Instantiate(owner.avatarPrefab, point.transform.position, Quaternion.identity);
            var avatar = obj.GetComponent<Avatar>();
            avatar.MakeUnit(unit, side);
        }

        foreach (var point in points)
        {
            Object.Destroy(point.gameObject);
        }
    }
}