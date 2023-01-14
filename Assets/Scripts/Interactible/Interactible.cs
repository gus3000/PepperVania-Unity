using System;
using UnityEngine;
using UnityEngine.Serialization;


[RequireComponent(typeof(Collider))]

public abstract class Interactible : MonoBehaviour
{
    protected Collider Collider;
    public bool CanInteract { get; private set; }

    public bool Used
    {
        get => _used;
        protected set
        {
            _used = value;
            if (value)
                CanInteract = false;
        }
    }

    [SerializeField] protected string verb;
    [SerializeField] private bool _used;

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
        if (Used)
            return;
        Debug.Log("Interactible collider enter");
        CanInteract = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (Used)
            return;
        Debug.Log("Interactible collider exit");
        CanInteract = false;
    }

    public abstract void Interact();
}