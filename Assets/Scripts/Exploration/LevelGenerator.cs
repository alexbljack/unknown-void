using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Global")]
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] float cellSize;
    [SerializeField] float cellOffset;

    [Header("Cells")]
    [SerializeField] GameObject regularCell;
    
    MapCell[,] _levelMatrix;
    
    [Button]
    void CreateLevel()
    {
        ClearCells();
        _levelMatrix = new MapCell[width, height];
        for (var i=0; i < width; i++)
        {
            for (var j=0; j < height; j++)
            {
                var worldX = i * (cellSize + cellOffset);
                var worldY = j * (cellSize + cellOffset);
                var worldPosition = new Vector2(worldX, worldY);
                var obj = Instantiate(regularCell, worldPosition, Quaternion.identity);
                var cell = obj.GetComponent<MapCell>();
                cell.Init(new Vector2(i, j), gameObject);
                _levelMatrix[i, j] = cell;
            }
        }
    }
    
    [Button]
    void ClearCells()
    {
        foreach (var cell in GetComponentsInChildren<MapCell>())
        {
            DestroyImmediate(cell.gameObject);
        }
    }
}
