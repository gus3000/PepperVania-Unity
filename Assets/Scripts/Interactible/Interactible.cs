using System;
using UnityEngine;
using UnityEngine.Serialization;


[RequireComponent(typeof(Collider))]

public abstract class Interactible : MonoBehaviour
{
    protected Collider Collider;
    protected Action InteractingCallback;

    public bool CloseEnough { get; private set; }

    public bool CanInteract => CloseEnough && !Used;

    public string Verb => verb;
    // public bool Used
    // {
    //     get => _used;
    //     protected set
    //     {
    //         _used = value;
    //         if (value)
    //             CanInteract = false;
    //     }
    // }

    [SerializeField] protected string verb;
    [field: SerializeField] public bool Used { get; protected set; }

    protected Interactible()
    {
        Used = false;
    }

    protected virtual void Start()
    {
        Collider = GetComponent<Collider>();
        Debug.Log("Interactible start");
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Interactible collider enter");
        CloseEnough = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Interactible collider exit");
        CloseEnough = false;
    }

    public void Interact()
    {
        if (!CanInteract)
            return;
        OnInteract();
    }
    
    public abstract void OnInteract();
}