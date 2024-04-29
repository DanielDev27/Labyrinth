using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public static HealthSystem Instance;
    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;
    [SerializeField] bool dead;
    [SerializeField] Slider healthBar;
    void Start()
    {
        Instance = this;
        GetHealth();
        maxHealth = currentHealth;
        healthBar.value = 100 * currentHealth / maxHealth;
    }
    void Update()
    {
        GetHealth();
        healthBar.value = 100 * currentHealth / maxHealth;
    }
    // Update is called once per frame
    public int GetHealth() { return currentHealth; }
    public void TakeDamage(int damage) { currentHealth -= damage; }
    public void UpdateHealth(int health) { currentHealth = health; }
    public void EnemyDie() { this.gameObject.SetActive(false); }
    public void PlayerDie() { this.gameObject.SetActive(false); }
}