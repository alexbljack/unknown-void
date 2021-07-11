using System;
using System.Collections;
using System.Collections.Generic;
using Lib;
using UnityEngine;

public class MapCell : MonoBehaviour
{
    Vector3 _initialScale;
    [SerializeField]
    Vector2 _mapPosition;

    Squad _player;
    Collider2D _collider;

    public Vector2 Position => _mapPosition;

    bool CanMoveSquad => IsNeighbourToPlayer();
    
    bool IsNeighbourToPlayer()
    {
        // foreach (var dir in Helpers.Directions())
        // {
        //     if (_player.mapPosition + dir == _mapPosition)
        //     {
        //         return true;
        //     }
        // }
        return false;
    }

    void Start()
    {
        _player = FindObjectOfType<Squad>();
        _collider = GetComponent<Collider2D>();
        _initialScale = transform.localScale;
    }

    public void Init(Vector2 position, GameObject map)
    {
        _mapPosition = position;
        gameObject.name = $"Cell {position.x}x{position.y}";
        transform.SetParent(map.transform);
    }

    void OnMouseDown()
    {
        // if (CanMoveSquad) { _player.MoveToCell(this); }
    }

    void OnMouseEnter()
    {
        if (CanMoveSquad)
        {
            var scale = transform.localScale;
            transform.localScale = new Vector3(scale.x*1.1f, scale.y*1.1f, scale.z);
        }
    }

    void OnMouseExit()
    {
        transform.localScale = _initialScale;
    }
}
