using UnityEngine;

public class Checkpoint : Interactible
{
    public Vector3 animStart;
    public override void OnInteract()
    {
        Debug.Log("Checkpoint");
        var player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        // player.PlayAnimation(PlayerAnimation.Rest);

        StartCoroutine(player.StartAnimationAt(transform.position + animStart, PlayerAnimation.Rest));
    }
}