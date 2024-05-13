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
    private void Awake()
    {
        Instance = this;
        GetHealth();
        maxHealth = currentHealth;
        healthBar.value = 100 * currentHealth / maxHealth;
    }
    void Update()
    {
        healthBar.value = 100 * currentHealth / maxHealth;
    }
    // Update is called once per frame
    public int GetHealth()
    {
        if (this.gameObject.GetComponent<PlayerController>() != null)
        {
            currentHealth = GetComponent<PlayerController>().maxHealth;
        }
        if (this.gameObject.GetComponent<AIBehaviour>() != null)
        {
            currentHealth = GetComponent<AIBehaviour>().maxHealth;
        }
        return currentHealth;
    }
    public void TakeDamage(int damage) { currentHealth -= damage; }
    public int UpdateHealth() { return currentHealth; }
    public void EnemyDie()
    {
        StartCoroutine(EnemyDespawn());
    }
    IEnumerator EnemyDespawn()
    {
        yield return new WaitForSeconds(5);
        GetComponent<Animator>().SetBool("Dead", false);
        this.gameObject.SetActive(false);
        Manager.Instance.GameWin();
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