using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    [Header("Visual")]
    public Sprite avatar;
    public AnimatorOverrideController combatAnimator;

    public int hp;
    public int maxHp;

    [Header("Attributes")]
    public int speed;
    
    public int defence;
    public int agility;
    public int strength;
    public int science;

    [Header("Skills")]
    public List<UnitSkill> skills;

    public event Action<int> ChangedHpEvent;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeHealth(int amount)
    {
        hp += amount;
        hp = Mathf.Clamp(hp, 0, maxHp);
        ChangedHpEvent?.Invoke(hp);
    }
}
