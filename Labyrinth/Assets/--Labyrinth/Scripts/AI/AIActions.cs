using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class AIActions : MonoBehaviour
{
    [SerializeField] private string aiName;
    [SerializeField] public Transform aiTransform;
    [SerializeField] public Rigidbody aIRigidbody;
    [SerializeField] public Collider aICollider;
    public ActionObject currentActionObject;
    [SerializeField] public Transform rayTransform;
    private void Awake()
    {
        ValidateAndInitActionObject();
    }

    private void OnEnable()
    {
        currentActionObject?.OnEnable();
    }
    void OnDisable()
    {
        currentActionObject?.OnDisable();
    }
    void Start()
    {
        RegisterAndConfigureActionObject();
        currentActionObject?.Start();
    }

    void Update() { }
    void FixedUpdate()
    {
        currentActionObject?.FixedUpdate();
    }
    void OnDrawGizmos()
    {
        currentActionObject?.OnDrawGizmos();
    }
    void ValidateAndInitActionObject()
    {
        if (currentActionObject == null)
        {
            Debug.LogError($"{gameObject.name} disabled");
            gameObject.SetActive(false);
            return;
        }

        currentActionObject = Instantiate(currentActionObject);
        Debug.Log($"{currentActionObject.name} {currentActionObject.GetInstanceID()}");

        currentActionObject?.Awake();
    }
    void RegisterAndConfigureActionObject()
    {
        if (currentActionObject != null)
        {

        }
    }
    void OnCollisionEnter(Collision collision)
    {

    }
}
