using System.Collections.Generic;
using System.Linq;
using Lib;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NtGenerator : MonoBehaviour
{
    [Header("Tilemap settings")]
    public Tilemap levelTilemap;
    [SerializeField] Tile floorTile;
    [SerializeField] Tile wallTile;
    [SerializeField] Tile emptyTile;
    
    [Header("Level generator")]
    [SerializeField] int gridWidth;
    [SerializeField] int gridHeight;
    
    [Range(0, 1)]
    [SerializeField] float fillPercent;
    [SerializeField] int maxWalkers;
    
    [Header("Walkers settings")]
    [SerializeField] int destroyWalkerChance = 5;
    [SerializeField] int changeDirectionChance = 50;
    [SerializeField] int spawnWalkerChance = 5;
    [SerializeField] float directionChanceIncreaseStep = 0.3f;

    Dictionary<CellType, Tile> TileMapping => new Dictionary<CellType, Tile>
    {
        {CellType.Floor, floorTile},
        {CellType.Wall, wallTile},
        {CellType.Empty, null}
    };
    
    CellType[,] _grid;
    List<Walker> _walkers;
    
    [Button]
    void GenerateLevel()
    {
        ClearMap();
        InitGrid();
        InitWalkers();
        Generate();
        DrawLevel();
    }

    void DrawLevel()
    {
        for (var i = 0; i < gridWidth; i++)
        {
            for (var j = 0; j < gridHeight; j++)
            {
                levelTilemap.SetTile(new Vector3Int(i, j, 0), TileMapping[_grid[i, j]]);
            }
        }
    }

    [Button]
    void ClearMap()
    {
        levelTilemap.ClearAllTiles();
    }
    
    void Generate()
    {
        var iterations = 0;
        
        do
        {
            foreach (var walker in _walkers)
            {
                _grid[walker.pos.x, walker.pos.y] = CellType.Floor;
            }
            
            if (Helpers.DiceRoll(destroyWalkerChance) && _walkers.Count > 1)
            {
                _walkers.RemoveAt(Random.Range(0, _walkers.Count));
            }

            foreach (var walker in _walkers.Where(walker => Helpers.DiceRoll(changeDirectionChance)))
            {
                var rawChance = (int) (changeDirectionChance * walker.DirectionChanceMultiplier);
                var changeDirChance = Mathf.Clamp(rawChance, 0, 100);
                if (Helpers.DiceRoll(changeDirChance))
                {
                    walker.dir = Helpers.RandomDirection();
                }
                else
                {
                    walker.DirectionChanceMultiplier += directionChanceIncreaseStep;
                }
            }

            for (var i=0; i < _walkers.Count; i++)
            {
                if (Helpers.DiceRoll(spawnWalkerChance) && _walkers.Count < maxWalkers)
                {
                    _walkers.Add(new Walker(_walkers[i].pos));
                }
            }

            foreach (var walker in _walkers)
            {
                walker.Move(gridWidth, gridHeight);
            }
            
            if ((float)FloorsCount() / (gridWidth * gridHeight) > fillPercent)
            {
                break;
            }

            iterations++;
        } while (iterations < 10000);
        
        for (var i = 0; i < gridWidth; i++)
        {
            for (var j = 0; j < gridHeight; j++)
            {
                var cell = _grid[i, j];
                foreach (var dir in Helpers.Directions())
                {
                    var nx = Mathf.Clamp(i + dir.x, 0, gridWidth-1);
                    var ny = Mathf.Clamp(j + dir.y, 0, gridHeight-1);
                    var neighbour = _grid[nx, ny];
                    if (cell == CellType.Floor && neighbour == CellType.Empty)
                    {
                        _grid[nx, ny] = CellType.Wall;
                    }
                }
            }
        }
    }

    void InitGrid()
    {
        _grid = new CellType[gridWidth, gridHeight];
        for (var i = 0; i < gridWidth; i++)
        {
            for (var j = 0; j < gridHeight; j++)
            {
                _grid[i, j] = CellType.Empty;
            }
        }
    }

    void InitWalkers()
    {
        _walkers = new List<Walker>();
        var gridCenter = new Vector2Int(gridWidth / 2, gridHeight / 2);
        var walker = new Walker(gridCenter);
        _walkers.Add(walker);
    }

    int FloorsCount()
    {
        var count = 0;
        for (var i=0; i < gridWidth; i++)
        {
            for (var j=0; j < gridHeight; j++)
            {
                if (_grid[i, j] == CellType.Floor)
                {
                    count++;
                }
            }
        }
        return count;
    }
}

public enum CellType
{
    Floor,
    Wall,
    Empty
}

internal class Walker
{
    public Vector2Int dir;
    public Vector2Int pos;
    public float DirectionChanceMultiplier { get; set; }

    public Walker(Vector2Int position)
    {
        pos = position;
        DirectionChanceMultiplier = 1f;
        dir = Helpers.RandomDirection();
    }

    public void Move(int width, int height)
    {
        pos += dir;
        var x = Mathf.Clamp(pos.x, 1, width - 2);
        var y = Mathf.Clamp(pos.y, 1, height - 2);
        pos = new Vector2Int(x, y);
    }
}