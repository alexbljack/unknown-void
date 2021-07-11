using System;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TerraIncognita : MonoBehaviour
{
    Tilemap _tilemap;
    Squad _squad;

    void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
        _squad = FindObjectOfType<Squad>();
    }

    void Start()
    {
    }

    void Update()
    {
        UpdateVisibleArea();
    }

    void UpdateVisibleArea()
    {
        for (var i=0; i < 60; i++)
        {
            for (var j=0; j < 60; j++)
            {
                var cell = new Vector3Int(i, j, 0);
                var worldPos = _tilemap.CellToWorld(cell);
                if ((worldPos - _squad.transform.position).magnitude < _squad.lookDistance)
                {
                    _tilemap.SetTile(cell, null);
                }
            }
        }
    }
}