using System;
using EasyButtons;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class DungeonCellularAutomata : MonoBehaviour {
    [Header ("Parameters")]
    [SerializeField] int width = 50;

    [SerializeField] int height = 50;
    [SerializeField] int iterations = 5;
    [SerializeField] int seed = 0;
    [SerializeField] bool randomSeed = true;
    [SerializeField] float fillPercentage = 65;
    [SerializeField] int wallThreshold = 4;

    [Header ("References")]
    [SerializeField] new Camera camera;

    TileType[,] dungeon;

    [Header ("Tiles")]
    [SerializeField] Tilemap tilemap;

    [SerializeField] Tile wallTile;
    [SerializeField] Tile floorTile;

    [Button]
    void Generate () {
        RepositionCamera ();
        GenerateDungeon ();
        GenerateAutomata ();
        DrawDungeon ();
    }

    void RepositionCamera () {
        camera.transform.position = new Vector3 (width / 2, height / 2, z: -10);
        camera.orthographicSize = height / 2;
    }

    void GenerateDungeon () {
        dungeon = new TileType[width, height];
        Random.InitState (randomSeed ? Random.Range (0, Int32.MaxValue) : seed);
        tilemap.ClearAllTiles ();
        for (int _x = 0; _x < width; _x++) {
            for (int _y = 0; _y < height; _y++) {
                if (Random.Range (0, 100) < fillPercentage) {
                    dungeon[_x, _y] = TileType.Wall;
                } else {
                    dungeon[_x, _y] = TileType.Floor;
                }
            }
        }
    }

    void GenerateAutomata () {
        for (int i = 0; i < iterations; i++) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    int neighbouringWallCount = GetNeighbouringWalls (x, y);
                    if (neighbouringWallCount > wallThreshold) {
                        dungeon[x, y] = TileType.Wall;
                    } else {
                        dungeon[x, y] = TileType.Floor;
                    }
                }
            }
        }
    }

    int GetNeighbouringWalls (int x, int y) {
        int _wallCount = 0;

        for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++) {
            //
            for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++) {
                //
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
                    //
                    if (neighbourX != x || neighbourY != y) {
                        //
                        if (dungeon[neighbourX, neighbourY] == TileType.Wall) {
                            _wallCount++;
                        }
                        //
                    }
                } else { _wallCount++; }
            }
        }

        return _wallCount;
    }

    void DrawDungeon () {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Tile tile = dungeon[x, y] == TileType.Wall ? wallTile : floorTile;
                tilemap.SetTile (new Vector3Int (x, y, 0), tile);
            }
        }
    }

    enum TileType {
        Wall,
        Floor
    }
}