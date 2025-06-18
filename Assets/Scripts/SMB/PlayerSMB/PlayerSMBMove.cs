using UnityEngine;

namespace IdleRPG
{
    public class PlayerSMBMove : SceneLinkedSMB<Player>
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

            // 대상 몬스터가 죽었으면 Idle로
            var targetMonster = tMonoBehaviour.target.GetComponent<Monster>();
            if (targetMonster == null || targetMonster.currentHp <= 0)
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