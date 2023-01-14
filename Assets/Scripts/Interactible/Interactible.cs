using System;
using UnityEngine;


[RequireComponent(typeof(Collider))]

public abstract class Interactible : MonoBehaviour
{
    protected Collider Collider;
    public bool CanInteract { get; private set; }
    [SerializeField] protected string name;
    [SerializeField] protected string verb;

    protected virtual void Start()
    {
        Collider = GetComponent<Collider>();
        Debug.Log("Interactible start");
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Interactible collider enter");
        CanInteract = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Interactible collider exit");
        CanInteract = false;
    }

    public abstract void Interact();
}