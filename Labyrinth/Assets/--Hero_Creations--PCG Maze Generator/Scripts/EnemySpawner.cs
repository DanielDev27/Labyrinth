using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemySpawner: MonoBehaviour
{
    public static EnemySpawner Instance;
    [Header("Debug")]
    [SerializeField] Vector2 _currentSpawnPosition;
    [SerializeField] List<Vector2> spawnPositions;
    [ShowInInspector] Dictionary<Vector2, bool> enemyPlacements = new Dictionary<Vector2, bool>();
    Dictionary<Vector2, SpaceOccupied> occupiedDict;
    [Header("References")]
    [SerializeField] GameObject aiPrefab;
    [SerializeField] PCGDungeonGenerator mazeGenerator;
    [Header("Settings")]
    [SerializeField] Transform aiParent;
    [SerializeField] Vector2 mazeOffset;
    [SerializeField] int enemyDRadius;
    private void Awake()
    {
        Instance = this;
        aiParent = transform;
        aiPrefab.GetComponent<HealthBarsRotation>().mainCamera = FindObjectOfType<Camera>().transform;
    }
    private void OnEnable()
    {
        mazeGenerator.mazeComplete.AddListener(GetMazeData);
    }
    public void GetMazeData()
    {
        this.occupiedDict = mazeGenerator.occupiedDict;
        foreach (Vector2 gridPosition in occupiedDict.Keys)
        {
            spawnPositions.Add(gridPosition);
            enemyPlacements.Add(gridPosition, false);
        }
        spawnPositions.Remove(new Vector2(mazeOffset.x - 1, 1));
        spawnPositions.Remove(new Vector2(mazeOffset.x + 1, 1));
        spawnPositions.Remove(new Vector2(0, 0));
        SpawnEnemies();
    }
    public void SpawnEnemies()
    {
        _currentSpawnPosition = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count)];
        for (int x = -enemyDRadius; x < enemyDRadius; x++)
        {
            for (int y = -enemyDRadius; y < enemyDRadius; y++)
            {
                if (spawnPositions.Contains(_currentSpawnPosition + new Vector2(x, y)))
                {
                    spawnPositions.Remove(_currentSpawnPosition + new Vector2(x, y));
                }
            }
        }
        GameObject aiEnemy = Instantiate(aiPrefab, new Vector3(_currentSpawnPosition.x * mazeGenerator.dungeon.roomWidth -
        mazeGenerator.dungeon.roomWidth * mazeOffset.x, transform.position.y, _currentSpawnPosition.y *
        mazeGenerator.dungeon.roomHeight - mazeGenerator.dungeon.roomHeight * mazeOffset.y), Quaternion.identity);
        aiEnemy.name = "AI" + _currentSpawnPosition;
        enemyPlacements[_currentSpawnPosition] = true;
        aiEnemy.transform.parent = aiParent;
        aiEnemy.GetComponent<AIBehaviour>().spawnPosition = _currentSpawnPosition;

        if (spawnPositions.Count > 0)
        {
            SpawnEnemies();
        }
        if (spawnPositions.Count<=0 && PlayerController.Instance != null)
        {
            PlayerController.Instance.CheckEnemies();
        }
    }
    public void RemoveEnemies()
    {
        for (int i = 0; i < aiParent.childCount; i++)
        {
            Destroy(aiParent.GetChild(i).gameObject);
        }
        spawnPositions.Clear();
        enemyPlacements.Clear();
    }
}
