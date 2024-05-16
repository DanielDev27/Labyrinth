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
        foreach (Vector2 gridPosition in occupiedDict.Keys)
        {
            spawnPositions.Add(gridPosition);
        }
        //Debug.Log("SpawnEnemies");
        for (int i = 0; i < numEnemies; i++)
        {
            Vector2 _spanwPosition = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count)];
            GameObject aiEnemy = Instantiate(aiPrefab, new Vector3(_spanwPosition.x * PCGDungeonGenerator.Instance.dungeon.roomWidth -
            PCGDungeonGenerator.Instance.dungeon.roomWidth * mazeOffset.x, transform.position.y,
            _spanwPosition.y * PCGDungeonGenerator.Instance.dungeon.roomHeight - PCGDungeonGenerator.Instance.dungeon.roomHeight * mazeOffset.y),
            Quaternion.identity);
            aiEnemy.transform.parent = aiParent;
        }
    }
}
