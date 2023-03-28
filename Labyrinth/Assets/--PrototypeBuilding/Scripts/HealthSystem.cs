using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour {
    public static HealthSystem instance;
    [SerializeField] int currentHealth;
    [SerializeField] bool dead;
    void Start () { instance = this; }

    // Update is called once per frame
    void Update () { }

    public void TakeDamage () { }

    public void UpdateHealth (int health) { currentHealth = health; }

    public void EnemyDie () { }

    public void PlayerDie () { }
}