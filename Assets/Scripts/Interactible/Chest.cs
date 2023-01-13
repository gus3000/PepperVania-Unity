using System;
using UnityEngine;

public class Chest : Interactible
{
    protected Animator _animator;
    private static readonly int OpenTriggerHash = Animator.StringToHash("Open");

    private void Start()
    {
        base.Start();
        Debug.Log("Chest start");
        _animator = GetComponentInChildren<Animator>();
    }

    public override void Interact()
    {
        Debug.Log("Opening !");
        _animator.SetTrigger(OpenTriggerHash);
    }
}