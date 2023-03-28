using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour {
    public static HealthSystem instance;
    [SerializeField] int currentHealth;
    [SerializeField] bool dead;
    void Start () { instance = this; }

    // Update is called once per frame
    public int GetHealth () { return currentHealth; }

    public void TakeDamage (int damage) { currentHealth -= damage; }

    public void UpdateHealth (int health) { currentHealth = health; }

    public void EnemyDie () { }

    public void PlayerDie () { }
}