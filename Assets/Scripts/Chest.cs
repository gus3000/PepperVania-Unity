using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Chest : MonoBehaviour
{
    private Collider _collider;
    public bool Interactable { get; private set; }

    private void Start()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Chest collider enter");
        Interactable = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Chest collider exit");
        Interactable = false;
    }
}