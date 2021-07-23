using System;
using System.Collections;
using System.Collections.Generic;
using Lib;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : Singleton<LevelManager>
{
    public List<Marine> players;
    public List<Enemy> facedEnemies;

    public static readonly string CombatScene = "Combat";
    public static readonly string LevelScene = "InnerMap";
    
    public bool paused;
    public static event Action<bool> PauseEvent;

    GameObject _levelRoot;

    new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void EnterBattle(List<Enemy> enemies)
    {
        facedEnemies = enemies;
        _levelRoot = GameObject.Find("Level");
        _levelRoot.SetActive(false);
        StartCoroutine(LoadBattle());
    }

    IEnumerator LoadBattle()
    {
        AsyncOperation battleLoading = SceneManager.LoadSceneAsync(CombatScene, LoadSceneMode.Additive);
        yield return Helpers.WaitAsyncSceneOperation(battleLoading);
        Scene battle = SceneManager.GetSceneByName(CombatScene);
        SceneManager.SetActiveScene(battle);
    }

    public void ReturnFromBattle()
    {
        Scene level = SceneManager.GetSceneByName(LevelScene);
        SceneManager.SetActiveScene(level);
        StartCoroutine(LoadLevelRoutine());
    }

    IEnumerator LoadLevelRoutine()
    {
        yield return Helpers.WaitForSceneBecomeActive(LevelScene);
        AsyncOperation unloadBattle = SceneManager.UnloadSceneAsync(CombatScene);
        yield return Helpers.WaitAsyncSceneOperation(unloadBattle);
        _levelRoot.SetActive(true);
    }
    
    public void SetPause(bool value)
    {
        paused = value;
        PauseEvent?.Invoke(value);
    }
    
    [Button]
    void Pause()
    {
        SetPause(true);
    }
    
    [Button]
    void UnPause()
    {
        SetPause(false);
    }
}
