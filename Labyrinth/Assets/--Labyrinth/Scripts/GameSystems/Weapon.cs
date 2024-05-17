using UnityEngine;
public class Weapon : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] bool isWeaponOn;
    [Header("References")]
    [SerializeField] GameObject hurtBox;
    [Header("Weapon Settings")]
    public int damage;
    [SerializeField] bool characterWeapon;
    [SerializeField] bool aIWeapon;
    private void Start()
    {
        DisableHurtBox();
    }
    public void EnableHurtBox()//Animation event to turn on collider
    {
        isWeaponOn = true;
        hurtBox.SetActive(true);
    }
    public void DisableHurtBox()//Animation event to turn off collider
    {
        isWeaponOn = false;
        hurtBox.SetActive(false);
    }
    private void OnCollisionEnter(Collision other)//Collision Event
    {
        if (characterWeapon)
        {
            if (other.gameObject.GetComponent<AIBehaviour>() != null && hurtBox.activeSelf == true)
            {//If player weapon collide with AI logic
                AIBehaviour ai = other.gameObject.GetComponent<AIBehaviour>();
                ai.GetComponent<HealthSystem>().TakeDamage(damage);
                DisableHurtBox();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (aIWeapon)
        {
            if (other.gameObject.GetComponent<PlayerController>() != null && hurtBox.activeSelf == true)
            {//if AI weapon collide with player logic
                PlayerController _char = other.gameObject.GetComponent<PlayerController>();
                Debug.Log("Hit Player");
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