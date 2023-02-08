using UnityEngine;

namespace Animation
{
    public class PlayerAnimAnimationStep : AnimationStep
    {
        // private PlayerAnimationController _playerAnimationController;
        [SerializeField] private string _animationName;
        
        public override void Init()
        {
            // _playerAnimationController = GameObject.FindWithTag("Player").GetComponent<PlayerAnimationController>();
        }
    }
}