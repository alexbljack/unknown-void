using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lib;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CombatManager : Singleton<CombatManager>
{
    public GameObject avatarPrefab;
    
    [Header("Battle UI")]
    public GameObject actionMenu;
    public GameObject skillNameBox;
    public GameObject actionButtonPrefab;
    public GameObject winBattleResult;
    
    List<Avatar> _units;
    Avatar _activeUnit;
    UnitSkill _activeSkill;
    Avatar _targetUnit;

    public List<Avatar> Units => _units;
    public Avatar ActiveUnit => _activeUnit;
    public UnitSkill ActiveSkill => _activeSkill;
    public Avatar TargetUnit => _targetUnit;
    
    public List<Avatar> Players => _units.Where(unit => unit.Side == UnitSide.Player).ToList();
    public List<Avatar> Enemies => _units.Where(unit => unit.Side == UnitSide.Enemy).ToList();

    public bool Win => Players.All(u => !u.IsAlive);
    public bool Defeat => Enemies.All(u => !u.IsAlive);

    StateMachine<CombatManager> _fsm;
    State<CombatManager> _startCombatState;
    State<CombatManager> _prepareState;
    State<CombatManager> _playerActionState;
    State<CombatManager> _selectTargetState;
    State<CombatManager> _playSkillState;
    State<CombatManager> _endBattleState;

    new void Awake()
    {
        InitFSM();
    }

    void Start()
    {
        StartCoroutine(StartLoopRoutine());
    }

    IEnumerator StartLoopRoutine()
    {
        yield return Helpers.WaitForSceneBecomeActive(LevelManager.CombatScene);
        _fsm.Start();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            EndCombat();
        }
        _fsm?.Update();
    }

    void InitFSM()
    {
        _startCombatState = new StartCombatState(this);
        _prepareState = new PrepareState(this);
        _playerActionState = new PlayerActionState(this);
        _selectTargetState = new SelectTargetState(this);
        _playSkillState = new PlaySkillState(this);
        _endBattleState = new EndBattleState(this);
        
        _fsm = new StateMachine<CombatManager>(_startCombatState);
    }

    public void InitAvatars(List<Avatar> units)
    {
        _units = units;
    }

    public void ResetUnits()
    {
        _activeUnit = null;
        _targetUnit = null;
        _activeSkill = null;
    }

    public void OnResume()
    {
        _fsm.ChangeState(_prepareState);
    }

    public void ResetSelection()
    {
        _fsm.ChangeState(_playerActionState);
    }

    public void OnUnitReady(Avatar unit)
    {
        switch (unit.Side)
        {
            case UnitSide.Player:
                OnPlayerReady(unit);
                break;
            case UnitSide.Enemy:
                OnEnemyReady(unit);
                break;
            default:
                Debug.Log("Wrong side, dude!");
                break;
        }
    }
    
    void OnPlayerReady(Avatar player)
    {
        _activeUnit = player;
        _fsm.ChangeState(_playerActionState);
    }

    void OnEnemyReady(Avatar enemy)
    {
        _activeUnit = enemy;
        _activeSkill = enemy.EnemyChooseSkill();
        _targetUnit = Players.Where(p => p.IsAlive).ToList()[Random.Range(0, Players.Count-1)];
        _fsm.ChangeState(_playSkillState);
    }

    public void OnSelectTarget(Avatar target)
    {
        _targetUnit = target;
        _fsm.ChangeState(_playSkillState);
    }

    public void CheckForBattleEnd()
    {
        if (Win || Defeat)
        {
            EndCombat();
        }
    }

    public void EndCombat()
    {
        _fsm.ChangeState(_endBattleState);
    }

    public void Continue()
    {
        LevelManager.Instance.ReturnFromBattle();
    }

    public void ShowActionMenu()
    {
        actionMenu.SetActive(true);
        
        foreach (var skill in _activeUnit.Skills)
        {
            var obj = Instantiate(actionButtonPrefab, Vector3.zero, Quaternion.identity);
            obj.transform.SetParent(actionMenu.transform);
            obj.GetComponent<Image>().sprite = skill.icon;
            obj.GetComponent<Button>().onClick.AddListener(() =>
            {
                _activeSkill = skill;
                _fsm.ChangeState(skill.needTarget ? _selectTargetState : _playSkillState);
            });
        }
    }

    public void HideActionMenu()
    {
        foreach (var skillButton in actionMenu.GetComponentsInChildren<Button>())
        {
            Destroy(skillButton.gameObject);
        }
        actionMenu.SetActive(false);
    }
    
    public void ShowSkillNameBox(string skillName)
    {
        skillNameBox.SetActive(true);
        skillNameBox.GetComponentInChildren<Text>().text = skillName;
    }

    public void HideSkillNameBox()
    {
        skillNameBox.SetActive(false);
    }

    public void ShowBattleResults()
    {
        
        winBattleResult.SetActive(true);
    }

    public void HideBattleResults()
    {
        winBattleResult.SetActive(false);
    }
}