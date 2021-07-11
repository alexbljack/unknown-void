using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lib;
using UnityEngine;

public class Squad : MonoBehaviour
{
    public float lookDistance;

    LerpMove _lerp;
    AStar _pathfind;
    List<Vector3> _path = new List<Vector3>();
    
    public static event Action<List<Vector3>> BuildPathEvent;
    public static event Action<Vector3> ReachedEvent;

    void Awake()
    {
        _lerp = GetComponent<LerpMove>();
    }

    void OnEnable()
    {
        _lerp.ReachedPointEvent += OnReachTile;
    }

    void OnDisable()
    {
        _lerp.ReachedPointEvent -= OnReachTile;
    }

    void Start()
    {
        _pathfind = new AStar(
            new Vector3(0.5f, 0.5f, 0), 
            new Vector2(100, 100), 
            0.5f
            );
    }

    void Update()
    {
        _pathfind.DebugDraw();
        UpdateInput();
    }

    void UpdateInput()
    {
        if (Helpers.ClickedOnScreen())
        {
            (Vector3 position, GameObject obj) = Helpers.GetClickedTarget2D(Camera.main);
            var path = _pathfind.FindPath(transform.position, position);
            path.RemoveAt(0);
            _lerp.MoveAlongPoints(path);
            // if (path.SequenceEqual(_path))
            // {
            //     _lerp.MoveAlongPoints(path);
            //     _path = new List<Vector3>();
            // }
            // else
            // {
            //     _path = path;
            //     BuildPathEvent?.Invoke(_path);
            // }
        }
    }

    void OnReachTile(Vector3 point)
    {
        ReachedEvent?.Invoke(point);
    }
}
