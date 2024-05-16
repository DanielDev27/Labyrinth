using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;
    [Header("References")]
    [SerializeField] GameObject aiPrefab;
    [SerializeField] PCGDungeonGenerator mazeGenerator;
    [Header("Settings")]
    [SerializeField] Transform aiParent;
    private void Awake()
    {
        Instance = this;
        aiParent = this.transform;
    }
    private void OnEnable()
    {
        mazeGenerator.mazeComplete.AddListener(SpawnEnemies);
    }

    public void SpawnEnemies(Dictionary<Vector2, SpaceOccupied> occupiedDict)
    {
        Debug.Log("SpawnEnemies");
    }
}
