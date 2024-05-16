using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;
    [Header("Debug")]
    [SerializeField] List<Vector2> spawnPositions;
    [Header("References")]
    [SerializeField] GameObject aiPrefab;
    [SerializeField] PCGDungeonGenerator mazeGenerator;
    [Header("Settings")]
    [SerializeField] Transform aiParent;
    [SerializeField] Vector2 mazeOffset;
    [SerializeField] int numEnemies;
    private void Awake()
    {
        Instance = this;
        aiParent = this.transform;
        aiPrefab.GetComponent<HealthBarsRotation>().mainCamera = FindObjectOfType<Camera>().transform;
    }
    private void OnEnable()
    {
        mazeGenerator.mazeComplete.AddListener(SpawnEnemies);
    }
    public void SpawnEnemies(Dictionary<Vector2, SpaceOccupied> occupiedDict)
    {
        Vector2 _currentSpawnPostion = Vector2.zero;
        Vector2 _previousSpawnPostion = Vector2.zero;
        foreach (Vector2 gridPosition in occupiedDict.Keys)
        {
            spawnPositions.Add(gridPosition);
        }
        //Debug.Log("SpawnEnemies");
        for (int i = 0; i < numEnemies; i++)
        {
            _currentSpawnPostion = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count)];
            if (Vector2.Distance(_currentSpawnPostion, _previousSpawnPostion) < 5 && _previousSpawnPostion != Vector2.zero)
            {
                Debug.Log("Enemies spawned too close to each other");
            }
            GameObject aiEnemy = Instantiate(aiPrefab, new Vector3(_currentSpawnPostion.x * PCGDungeonGenerator.Instance.dungeon.roomWidth -
            PCGDungeonGenerator.Instance.dungeon.roomWidth * mazeOffset.x, transform.position.y, _currentSpawnPostion.y *
            PCGDungeonGenerator.Instance.dungeon.roomHeight - PCGDungeonGenerator.Instance.dungeon.roomHeight * mazeOffset.y),
            Quaternion.identity);
            aiEnemy.transform.parent = aiParent;
            _previousSpawnPostion = _currentSpawnPostion;
        }
    }
    public void RemoveEnemies()
    {
        for (int i = 0; i < aiParent.childCount; i++)
        {
            Destroy(aiParent.GetChild(i).gameObject);
        }
        spawnPositions.Clear();
    }
}
