using UnityEngine;
public class Weapon : MonoBehaviour
{
    public int damage;
    [SerializeField] Collider hurtBox;
    [Header("Weapon Settings")]
    [SerializeField] bool charcterWeapon;
    [SerializeField] bool aIWeapon;
    private void Start()
    {
        DisableHurtBox();
    }
    public void EnableHurtBox()
    {
        hurtBox.enabled = true;
    }
    public void DisableHurtBox()
    {
        hurtBox.enabled = false;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (charcterWeapon)
        {
            if (other.gameObject.GetComponent<AIBehaviour>() != null && hurtBox.enabled == true)
            {
                AIBehaviour ai = other.gameObject.GetComponent<AIBehaviour>();
                ai.GetComponent<HealthSystem>().TakeDamage(damage);
                DisableHurtBox();
            }
        }
        if (aIWeapon)
        {
            if (other.gameObject.GetComponent<CharacterController>() != null && hurtBox.enabled == true)
            {
                CharacterController _char = other.gameObject.GetComponent<CharacterController>();
                if (_char.IsBlock)
                {
                    DisableHurtBox();
                }
                else
                {
                    _char.GetComponent<HealthSystem>().TakeDamage(damage);
                    DisableHurtBox();
                }
            }
        }
    }
}