using UnityEngine;

namespace IdleRPG
{
    public class PlayerSMBAutoAttack : SceneLinkedSMB<Player>
    {
        float p_AttackTimer;
        bool waitingForAttackAnimEnd = false;

        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            tMonoBehaviour.isAttacking = true;
            p_AttackTimer = tMonoBehaviour.attackCooldown;
            waitingForAttackAnimEnd = false;
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // 타겟 없거나 비활성화/죽었으면 Idle로
            if (tMonoBehaviour.target == null || !tMonoBehaviour.target.gameObject.activeSelf)
            {
                animator.CrossFade("Idle", 0.05f);
                return;
            }

            var targetMonster = tMonoBehaviour.target.GetComponent<Monster>();
            if (targetMonster == null || targetMonster.currentHp <= 0)
            {
                animator.CrossFade("Idle", 0.05f);
                return;
            }

            // 공격 애니메이션 끝났으면 Idle로
            if (waitingForAttackAnimEnd && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                animator.CrossFade("Idle", 0.05f);
                waitingForAttackAnimEnd = false;
                return;
            }

            // 타겟 방향 회전
            RotateTowardsTarget(tMonoBehaviour.transform, tMonoBehaviour.target.transform);

            float dist = Vector3.Distance(
                tMonoBehaviour.transform.position,
                tMonoBehaviour.target.transform.position
            );
            if (dist > tMonoBehaviour.attackRange)
            {
                animator.CrossFade("Move", 0.05f);
                return;
            }

            p_AttackTimer -= Time.deltaTime;

            if (p_AttackTimer <= 0 && !waitingForAttackAnimEnd)
            {
                p_AttackTimer = tMonoBehaviour.attackCooldown;
                animator.Play("Attack", 0, 0f);
                waitingForAttackAnimEnd = true;
            }
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            tMonoBehaviour.isAttacking = false;
            waitingForAttackAnimEnd = false;
        }

        void RotateTowardsTarget(Transform self, Transform target, float rotationSpeed = 10f)
        {
            Vector3 direction = (target.position - self.position).normalized;
            direction.y = 0f;
            if (direction == Vector3.zero) return;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            self.rotation = Quaternion.Slerp(self.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
