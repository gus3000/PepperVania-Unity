using System;
using UnityEngine;
using UnityEngine.Events;

public class Chest : Interactible
{
    public int itemId;

    protected Animator _animator;
    private static readonly int OpenTriggerHash = Animator.StringToHash("Open");

    // [SerializeField] private string interactingCallbackName;
    // [SerializeField] private object[] _callbackParameters;

    protected override void Start()
    {
        base.Start();
        // Debug.Log("Chest start");
        _animator = GetComponentInChildren<Animator>();
        // InteractingCallback = () => typeof(Chest).GetMethod(interactingCallbackName)?.Invoke(this, null);
        InteractingCallback = () => GiveItem(itemId);
    }

    public override void OnInteract()
    {
        Debug.Log("Opening !");
        _animator.SetTrigger(OpenTriggerHash);
        Used = true;
        InteractingCallback();
    }

    public void GiveItem(int itemId)
    {
        Debug.Log($"You got an item ! (item {itemId})");
    }

    public void SetItemId(int newItemId)
    {
        Debug.Log($"setting item id to {newItemId}");
        itemId = newItemId;
    }

    public void GiveNothing()
    {
        Debug.Log("You got scammed !");
    }
}