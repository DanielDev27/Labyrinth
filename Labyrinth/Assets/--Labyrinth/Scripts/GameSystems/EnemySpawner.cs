using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;
    [Header("Debug")]
    [SerializeField] Vector2 _currentSpawnPosition;
    [SerializeField] List<Vector2> spawnPositions;
    [SerializeField] List<Vector2> possiblePlacements;
    [ShowInInspector] Dictionary<Vector2, bool> enemyPlacements = new Dictionary<Vector2, bool>();
    [Header("References")]
    [SerializeField] GameObject aiPrefab;
    [SerializeField] PCGDungeonGenerator mazeGenerator;
    [Header("Settings")]
    [SerializeField] Transform aiParent;
    [SerializeField] Vector2 mazeOffset;
    [SerializeField] int numEnemies;
    [SerializeField] int enemyDRadius;
    private void Awake()
    {
        Instance = this;
        aiParent = transform;
        aiPrefab.GetComponent<HealthBarsRotation>().mainCamera = FindObjectOfType<Camera>().transform;
    }
    private void OnEnable()
    {
        mazeGenerator.mazeComplete.AddListener(SpawnEnemies);
    }
    public void SpawnEnemies(Dictionary<Vector2, SpaceOccupied> occupiedDict)
    {
        foreach (Vector2 gridPosition in occupiedDict.Keys)
        {
            spawnPositions.Add(gridPosition);
            enemyPlacements.Add(gridPosition, false);
        }
        //Debug.Log("SpawnEnemies");
        for (int i = 0; i < numEnemies; i++)
        {
            _currentSpawnPosition = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count)];
            //Check space
            for (int x = -enemyDRadius; x < enemyDRadius; x++)
            {
                for (int y = -enemyDRadius; y < enemyDRadius; y++)
                {
                    Vector2 positionCheck = _currentSpawnPosition + new Vector2(x, y);
                    if (spawnPositions.Contains(positionCheck))
                    {
                        possiblePlacements.Add(positionCheck);
                    }
                }
            }
            GameObject aiEnemy = Instantiate(aiPrefab, new Vector3(_currentSpawnPosition.x * PCGDungeonGenerator.Instance.dungeon.roomWidth -
            PCGDungeonGenerator.Instance.dungeon.roomWidth * mazeOffset.x, transform.position.y, _currentSpawnPosition.y *
            PCGDungeonGenerator.Instance.dungeon.roomHeight - PCGDungeonGenerator.Instance.dungeon.roomHeight * mazeOffset.y),
            Quaternion.identity);
            enemyPlacements[_currentSpawnPosition] = true;
            aiEnemy.transform.parent = aiParent;
            //possiblePlacements.Clear();
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
