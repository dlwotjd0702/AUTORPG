using UnityEngine;

namespace IdleRPG
{
    public class MonsterSMBMove : SceneLinkedSMB<Monster>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            tMonoBehaviour.isMoving = true;
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (tMonoBehaviour.target == null)
            {
                animator.CrossFade("Idle", 0.05f);
                return;
            }

            // 대상 플레이어가 죽었으면 Idle로
            var targetPlayer = tMonoBehaviour.target.GetComponent<Player>();
            if (targetPlayer == null )
            {
                animator.CrossFade("Idle", 0.05f);
                return;
            }

            Vector3 dir = tMonoBehaviour.target.transform.position - tMonoBehaviour.transform.position;
            dir.y = 0;
            float dist = dir.magnitude;

            if (dist > tMonoBehaviour.attackRange - 0.5f)
            {
                tMonoBehaviour.transform.position += dir.normalized * tMonoBehaviour.moveSpeed * Time.deltaTime;
                tMonoBehaviour.transform.forward = dir.normalized;
            }
            else
            {
                animator.CrossFade("Attack", 0.05f);
            }
        }
    }
}