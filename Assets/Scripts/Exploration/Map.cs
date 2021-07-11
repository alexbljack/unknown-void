using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    [SerializeField] Tilemap overlay;
    [SerializeField] Tile pathTile;

    void OnEnable()
    {
        Squad.BuildPathEvent += OnBuildPath;
        Squad.ReachedEvent += OnSquadReachedPoint;
    }

    void OnDisable()
    {
        Squad.BuildPathEvent -= OnBuildPath;
        Squad.ReachedEvent -= OnSquadReachedPoint;
    }

    void OnBuildPath(List<Vector3> points)
    {
        overlay.ClearAllTiles();
        var tiles = points.Select(p => overlay.WorldToCell(p)).ToList();
        DrawPath(tiles);
    }

    void OnSquadReachedPoint(Vector3 point)
    {
        overlay.SetTile(overlay.WorldToCell(point), null);
    }

    void DrawPath(List<Vector3Int> points)
    {
        foreach (Vector3Int point in points)
        {
            overlay.SetTile(point, pathTile);
        }
    }
}
