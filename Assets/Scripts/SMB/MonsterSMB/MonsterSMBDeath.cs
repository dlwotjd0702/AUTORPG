using UnityEngine;

namespace IdleRPG
{
    public class MonsterSMBDeath : SceneLinkedSMB<Monster>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            tMonoBehaviour.isMoving = false;
            tMonoBehaviour.isAttacking = false;
            animator.applyRootMotion = true;
        }
    }
}