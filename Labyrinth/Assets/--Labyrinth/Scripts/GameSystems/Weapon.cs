using UnityEngine;
public class Weapon : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] bool isWeaponOn;
    [Header("References")]
    [SerializeField] Collider hurtBox;
    [SerializeField] Collider rLHurtBox;
    [SerializeField] Collider lLHurtBox;
    [Header("Weapon Settings")]
    public int damage;
    [SerializeField] bool characterWeapon;
    [SerializeField] bool aIWeapon;
    private void Start()
    {
        DisableWeaponHurtBox();
        if (aIWeapon)
        {
            DisableRLHurtBox();
            DisableLLHurtBox();
        }
    }
    public void EnableWeaponHurtBox()//Animation event to turn on collider
    {
        isWeaponOn = true;
        hurtBox.enabled = true;
    }
    public void DisableWeaponHurtBox()//Animation event to turn off collider
    {
        isWeaponOn = false;
        hurtBox.enabled = false;
    }
    public void EnableRLHurtBox()//Animation event to turn on collider
    {
        isWeaponOn = true;
        rLHurtBox.enabled = true;
    }
    public void DisableRLHurtBox()//Animation event to turn off collider
    {
        isWeaponOn = false;
        rLHurtBox.enabled = false;
    }
    public void EnableLLHurtBox()//Animation event to turn on collider
    {
        isWeaponOn = true;
        lLHurtBox.enabled = true;
    }
    public void DisableLLHurtBox()//Animation event to turn off collider
    {
        isWeaponOn = false;
        lLHurtBox.enabled = false;
    }
    private void OnCollisionEnter(Collision other)//Collision Event
    {
        if (characterWeapon)
        {
            if (other.gameObject.GetComponent<AIBehaviour>() != null && hurtBox.enabled == true)
            {//If player weapon collide with AI logic
                AIBehaviour ai = other.gameObject.GetComponent<AIBehaviour>();
                ai.GetComponent<HealthSystem>().TakeDamage(damage);
                DisableWeaponHurtBox();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null && aIWeapon)
        {
            if (hurtBox.enabled == true || lLHurtBox.enabled == true || rLHurtBox.enabled == true)
            {//if AI weapon collide with player logic
                PlayerController _char = other.gameObject.GetComponent<PlayerController>();
                //Debug.Log("Hit Player");
                if (_char.IsBlock)
                {//Does nothing if PLayer is blocking
                    DisableWeaponHurtBox();
                    DisableRLHurtBox();
                    DisableLLHurtBox();
                }
                else
                {//Does damage if player is not blocking
                    _char.GetComponent<HealthSystem>().TakeDamage(damage);
                    DisableRLHurtBox();
                    DisableLLHurtBox();
                    DisableWeaponHurtBox();
                }
            }
        }
    }
}