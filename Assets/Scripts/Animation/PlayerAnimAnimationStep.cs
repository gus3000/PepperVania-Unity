using UnityEngine;

namespace Animation
{
    public class PlayerAnimAnimationStep : AnimationStep
    {
        // private PlayerAnimationController _playerAnimationController;
        private string _animationName;
        
        public override void Init()
        {
            // _playerAnimationController = GameObject.FindWithTag("Player").GetComponent<PlayerAnimationController>();
        }

        public override Vector3 GetPoint(float timeSinceStart) => Vector3.zero;
    }
}