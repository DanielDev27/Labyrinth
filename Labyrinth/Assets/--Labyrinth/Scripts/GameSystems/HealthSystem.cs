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
    public UnityEvent takeDamagePlayer = new UnityEvent();
    public UnityEvent takeDamageEnemy = new UnityEvent();
    private void Awake()
    {
        Instance = this;
        GetHealth();
        maxHealth = currentHealth;
        healthBar.value = 100 * currentHealth / maxHealth;
    }
    public int UpdateHealth()//Update Health Logic
    {
        //Update the healthbar UI
        healthBar.value = 100 * currentHealth / maxHealth;
        return currentHealth;
    }
    public int GetHealth()//Logic to get the health from a character
    {
        //Player Health
        if (this.gameObject.GetComponent<PlayerController>() != null)
        {
            currentHealth = GetComponent<PlayerController>().maxHealth;
        }
        //AI Health
        if (this.gameObject.GetComponent<AIBehaviour>() != null)
        {
            currentHealth = GetComponent<AIBehaviour>().maxHealth;
        }
        return currentHealth;
    }
    public void TakeDamage(int damage)//Take damage Logic
    {
        currentHealth -= damage;
        if (this.gameObject.GetComponent<PlayerController>() != null)
        {
            takeDamagePlayer.Invoke();
            //Debug.Log("Player Take Damage");
        }
        if (this.gameObject.GetComponent<AIBehaviour>() != null && !this.gameObject.GetComponent<AIBehaviour>().immune)
        {
            takeDamageEnemy.Invoke();
            //Debug.Log("Enemy Take Damage");
        }
    }

}