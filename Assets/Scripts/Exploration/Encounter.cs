using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Encounter : MonoBehaviour
{
    public List<Enemy> enemies;
    AIRoaming _ai;
    
    void Awake()
    {
        _ai = GetComponent<AIRoaming>();
    }

    void OnEnable()
    {
        LevelManager.PauseEvent += OnPause;
    }

    void OnDisable()
    {
        LevelManager.PauseEvent -= OnPause;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Squad>())
        {
            LevelManager.Instance.EnterBattle(enemies);
            Destroy(gameObject);
        }
    }

    void OnPause(bool value)
    {
        _ai.enabled = !value;
    }
}
