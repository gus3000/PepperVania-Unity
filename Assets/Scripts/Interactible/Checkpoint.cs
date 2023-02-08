using Animation;
using Unity.VisualScripting;
using UnityEngine;

public class Checkpoint : Interactible
{
    public Vector3 animStart; //TODO change

    private AnimationSequence _animationSequence;
    
    protected virtual void Start()
    {
        base.Start();

        Debug.Log("Checkpoint start");
        _animationSequence = GetComponentInChildren<AnimationSequence>();
    }
    

    public override void OnInteract()
    {
        Debug.Log("Checkpoint");
        var player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        // player.PlayAnimation(PlayerAnimation.Rest);

        player.FollowSequence(_animationSequence);
    }
}