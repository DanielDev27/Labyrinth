using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class HealthSystem : MonoBehaviour
{
    public static HealthSystem Instance;
    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;
    [SerializeField] bool dead;
    [SerializeField] Slider healthBar;
    public UnityEvent AIDeath;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
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
    public void EnemyDie()
    {
        StartCoroutine(EnemyDespawn());
    }
    IEnumerator EnemyDespawn()
    {
        yield return new WaitForSeconds(5);
        GetComponent<Animator>().SetBool("Dead", false);
        this.gameObject.SetActive(false);
        AIDeath.Invoke();
    }
    public void PlayerDie()
    {
        GetComponent<Animator>().SetBool("Dead", true);
        StartCoroutine(PlayerDeath());
    }
    IEnumerator PlayerDeath()
    {
        yield return new WaitForSeconds(5);
        Manager.Instance.GameOver();
        GetComponent<Animator>().SetBool("Dead", false);
        this.gameObject.SetActive(false);
    }
}