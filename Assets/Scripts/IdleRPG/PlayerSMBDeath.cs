using UnityEngine;

namespace IdleRPG
{
    public class PlayerSMBDeath : SceneLinkedSMB<Player>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            tMonoBehaviour.isMoving = false;
            tMonoBehaviour.isAttacking = false;
            animator.applyRootMotion = true;
        }
    }
}