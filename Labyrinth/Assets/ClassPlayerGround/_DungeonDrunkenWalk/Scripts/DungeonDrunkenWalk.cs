using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using EasyButtons;

public class DungeonDrunkenWalk : MonoBehaviour {
    [SerializeField] int width = 50;
    [SerializeField] int height = 50;
    [SerializeField] int walkIterations = 1000;
    [SerializeField] new Camera camera;

    TileType[,] dungeon;
    [SerializeField] Vector2Int currentPosition;

    [SerializeField] Tilemap tilemap;
    [SerializeField] Tile wallTile;
    [SerializeField] Tile floorTile;

    enum Direction {
        Up,
        Down,
        Left,
        Right
    }

    enum TileType {
        Wall,
        Floor
    }

    Direction _direction;
    Vector2Int offset;
    [SerializeField] Vector2Int newPosition;

    [Button]
    void Generate () {
        RepositionCamera ();
        // Step 1: Call the GenerateDungeon method
        GenerateDungeon ();
        // Step 2: Call the DrawDungeon method
        DrawDungeon ();
    }

    void RepositionCamera () {
        camera.transform.position = new Vector3 (width / 2, height / 2, -10);
        camera.orthographicSize = width / 2;
    }

    void GenerateDungeon () {
        dungeon = new TileType[width, height];
        Debug.Log ("Generate Dungeon");
        // Step 1: Initialize the dungeon array
        // (Create a new array, set initial values as walls)
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < width; y++) {
                dungeon[x, y] = TileType.Wall;
            }
        }

        // Step 2: Choose a random starting point
        currentPosition = new Vector2Int (Random.Range (1, width), Random.Range (1, height));
        Debug.Log (currentPosition);

        // Step 3: Perform a random walk
        // (Update the dungeon array based on the random walk)
        for (int i = 0; i < walkIterations; i++) {
            Direction nextDirection = (Direction) Random.Range (0, 4);
            if (nextDirection == Direction.Up) {
                currentPosition.y = Mathf.Min (currentPosition.y + 1, height - 1);
            }

            if (nextDirection == Direction.Right) {
                currentPosition.x = Mathf.Min (currentPosition.x + 1, width - 1);
            }

            if (nextDirection == Direction.Down) {
                currentPosition.y = Mathf.Max (currentPosition.y - 1, height + 1);
            }

            if (nextDirection == Direction.Left) {
                currentPosition.x = Mathf.Max (currentPosition.x + 1, width + 1);
            }
        }
    }

    void DrawDungeon () {
        // Step 4: Iterate through the dungeon array and draw the corresponding tiles
        // (Use wallTile for walls and floorTile for floors)
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Tile tile = dungeon[x, y] == TileType.Wall ? wallTile : floorTile;
                tilemap.SetTile (new Vector3Int (x, y, 0), tile);
            }
        }
    }
}