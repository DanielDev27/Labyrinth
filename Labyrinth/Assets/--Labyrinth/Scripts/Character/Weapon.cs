using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weapon : MonoBehaviour {
    public int damage = 5;
    [SerializeField] float weaponLength;
    public bool canDamage;
    [SerializeField] List<GameObject> hasDealtDamage;
    [SerializeField] LayerMask layerMask;


    void Start () {
        canDamage = false;
        hasDealtDamage = new List<GameObject> ();
    }

    void Update () {
        if (canDamage) {
            StartCoroutine (DamageFunction ());
        }
    }

    IEnumerator DamageFunction () {
        if (canDamage) {
            RaycastHit hit;
            Debug.DrawRay (transform.position, -transform.up, Color.red);
            if (Physics.Raycast (transform.position, -transform.up, out hit, weaponLength, layerMask)) {
                if (!hasDealtDamage.Contains (hit.transform.gameObject)) {
                    Debug.Log ("Damage");
                    hasDealtDamage.Add (hit.transform.gameObject);
                    yield return new WaitForSeconds (0.75f);
                    foreach (GameObject target in hasDealtDamage) {
                        target.TryGetComponent<HealthSystem> (out HealthSystem _healthSystem);
                        _healthSystem.TakeDamage (damage);
                    }

                    hasDealtDamage.Clear ();
                }
            }
        }
    }

    public void StartDamage () {
        canDamage = true;
    }

    public void EndDamage () {
        canDamage = false;
    }

    void OnDrawGizmos () {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine (transform.position, transform.position - transform.forward * weaponLength);
    }
}