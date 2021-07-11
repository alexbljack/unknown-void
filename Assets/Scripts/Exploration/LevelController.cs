using System;
using Lib;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelController : Singleton<LevelController>
{
    public bool paused;

    public static event Action<bool> PauseEvent;

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