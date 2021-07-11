using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfo : MonoBehaviour
{
    Marine _unit;
    Image _portrait;
    Slider _hpBar;

    void Awake()
    {
        _hpBar = transform.Find("HP").GetComponent<Slider>();
        _portrait = transform.Find("Portrait").GetComponent<Image>();
    }

    public void Init(Marine unit)
    {
        _unit = unit;
        _hpBar.maxValue = _unit.maxHp;
        _hpBar.value = _unit.hp;
        _portrait.sprite = _unit.avatar;
        _unit.ChangedHpEvent += OnChangeHp;
    }

    void OnChangeHp(int hp)
    {
        _hpBar.value = hp;
    }

    void OnDisable()
    {
        _unit.ChangedHpEvent -= OnChangeHp;
    }
}
