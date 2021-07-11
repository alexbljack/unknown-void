using System;
using System.Collections;
using System.Collections.Generic;
using Lib;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum UnitSide
{
    Player,
    Enemy
}


public class Avatar : MonoBehaviour
{
    [Header("Unit UI")]
    [SerializeField] Slider hpBar;
    [SerializeField] Slider readyBar;
    
    UnitSide _side;
    BaseUnit _unit;
    
    Animator _animator;
    SpriteRenderer _renderer;

    float _readyCounter;
    static readonly int GetHit = Animator.StringToHash("GetHit");
    static readonly int Dead = Animator.StringToHash("Dead");

    public UnitSide Side => _side;
    public bool IsAlive => _unit.hp > 0;
    public bool IsReady => _readyCounter >= 100;
    public List<UnitSkill> Skills => _unit.skills;
    
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        ResetReadyCounter();
    }

    void Update()
    {
        UpdateUI();
        HighlightUnit();
    }

    void UpdateUI()
    {
        hpBar.maxValue = _unit.maxHp;
        readyBar.maxValue = 100;
        hpBar.value = _unit.hp;
        readyBar.value = _readyCounter;
    }

    void HideUI()
    {
        GetComponentInChildren<Canvas>().gameObject.SetActive(false);
    }

    public void UseSkill(UnitSkill skill)
    {
        _animator.SetTrigger(skill.animationTrigger);
    }

    public void Hit(int damage)
    {
        _unit.ChangeHealth(-damage);
        
        if (IsAlive)
        {
            GetDamage();
        }
        else
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        _unit.ChangeHealth(amount);
    }

    void GetDamage()
    {
        _animator.SetTrigger(GetHit);
    }

    void Die()
    {
        HideUI();
        _animator.SetTrigger(Dead);
    }

    public UnitSkill EnemyChooseSkill()
    {
        return _unit.skills[Random.Range(0, _unit.skills.Count-1)];
    }

    public bool IsIdle()
    {
        var animState = _animator.GetCurrentAnimatorStateInfo(0);
        return animState.IsName("Idle") || animState.IsName("Death");
    }

    public bool CanBeSkillTarget(UnitSkill skill)
    {
        return IsAlive && skill.targetTypes.Contains(Side);
    }

    void HighlightUnit()
    {
        Helpers.DebugDrawRect(Helpers.GetSpriteBoundsRect(_renderer), Color.green);
    }

    public void Prepare()
    {
        IncreaseReadyCounter();
    }

    void IncreaseReadyCounter()
    {
        _readyCounter += _unit.speed * Time.deltaTime;
        _readyCounter = Mathf.Clamp(_readyCounter, 0, 100);
    }

    public void ResetReadyCounter()
    {
        _readyCounter = 0;
    }

    public void MakeUnit<T>(T unit, UnitSide side) where T : BaseUnit
    {
        _side = side;
        if (side == UnitSide.Enemy)
        {
            _unit = Instantiate(unit.gameObject, Vector2.zero, Quaternion.identity).GetComponent<Enemy>();
        }
        else
        {
            _unit = unit;
        }
        _animator.runtimeAnimatorController = _unit.combatAnimator;
        _renderer.flipX = side == UnitSide.Enemy;
    }
}
