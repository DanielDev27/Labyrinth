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
    public UnityEvent takeDamage = new UnityEvent();
    private void Awake()
    {
        Instance = this;
        GetHealth();
        maxHealth = currentHealth;
        healthBar.value = 100 * currentHealth / maxHealth;
    }
    void FixedUpdate()
    {

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
            //GetComponent<Animator>().SetTrigger("damage");
        }
        if (this.gameObject.GetComponent<AIBehaviour>() != null && !this.gameObject.GetComponent<AIBehaviour>().immune)
        {
            GetComponent<Animator>().SetTrigger("damage");
            Debug.Log("Enemy Take Damage");
            takeDamage.Invoke();
        }
    }
    public int UpdateHealth()//Update Health Logic
    {
        //Update the healthbar UI
        healthBar.value = 100 * currentHealth / maxHealth;
        return currentHealth;
    }
    public void PlayerDie()//Triggering Death for Player
    {
        GetComponent<Animator>().SetBool("Dead", true);
        StartCoroutine(PlayerDeath());
    }
    IEnumerator PlayerDeath()
    {//Wait before removing gameobject
        yield return new WaitForSeconds(5);
        Manager.Instance.GameOver();
        GetComponent<Animator>().SetBool("Dead", false);
        this.gameObject.SetActive(false);
    }
    public void EnemyDie()//Triggering Death for AI
    {
        StartCoroutine(EnemyDespawn());
    }
    IEnumerator EnemyDespawn()
    {//Wait before despawning gameobject
        yield return new WaitForSeconds(5);
        GetComponent<Animator>().SetBool("Dead", false);
        this.gameObject.SetActive(false);
        Manager.Instance.GameWin();
    }
}