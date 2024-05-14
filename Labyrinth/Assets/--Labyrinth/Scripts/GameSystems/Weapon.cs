using UnityEngine;
public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Collider hurtBox;
    [Header("Weapon Settings")]
    public int damage;
    [SerializeField] bool charcterWeapon;
    [SerializeField] bool aIWeapon;
    private void Start()
    {
        DisableHurtBox();
    }
    public void EnableHurtBox()//Animation event to turn on collider
    {
        hurtBox.enabled = true;
    }
    public void DisableHurtBox()//Animation event to turn off collider
    {
        hurtBox.enabled = false;
    }
    private void OnCollisionEnter(Collision other)//Collision Event
    {
        if (charcterWeapon)
        {
            if (other.gameObject.GetComponent<AIBehaviour>() != null && hurtBox.enabled == true)
            {//If player weapon collide with AI logic
                AIBehaviour ai = other.gameObject.GetComponent<AIBehaviour>();
                ai.GetComponent<HealthSystem>().TakeDamage(damage);
                DisableHurtBox();
            }
        }
        if (aIWeapon)
        {
            if (other.gameObject.GetComponent<PlayerController>() != null && hurtBox.enabled == true)
            {//if AI weapon collide with player logic
                PlayerController _char = other.gameObject.GetComponent<PlayerController>();
                if (_char.IsBlock)
                {//Does nothing if PLayer is blocking
                    DisableHurtBox();
                }
                else
                {//Does damage if player is not blocking
                    _char.GetComponent<HealthSystem>().TakeDamage(damage);
                    DisableHurtBox();
                }
            }
        }
    }
}