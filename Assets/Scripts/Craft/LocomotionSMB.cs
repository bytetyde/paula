using UnityEngine;

namespace Assets.Scripts.Craft
{
    public class LocomotionSMB : StateMachineBehaviour
    {

        public float _damping = 0.15f;

        private readonly int _horizontal = Animator.StringToHash("Horizontal");
        private readonly int _vertical = Animator.StringToHash("Vertical");

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector2 input = new Vector2(horizontal, vertical).normalized;

            animator.SetFloat(_horizontal, input.x, _damping, Time.deltaTime);
            animator.SetFloat(_vertical, input.y, _damping, Time.deltaTime);
        }
    }
}
