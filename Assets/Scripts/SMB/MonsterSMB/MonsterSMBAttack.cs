using UnityEngine;

namespace IdleRPG
{
    public class MonsterSMBAttack : SceneLinkedSMB<Monster>
    {
        float tAttackTimer;
        bool waitingForAttackAnimEnd = false;

        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            tMonoBehaviour.isAttacking = true;
            tAttackTimer = tMonoBehaviour.attackCooldown;
            waitingForAttackAnimEnd = false;
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // 타겟이 없거나 죽었으면 Idle로
            if (tMonoBehaviour.target == null)
            {
                animator.CrossFade("Idle", 0.05f);
                return;
            }
            var targetPlayer = tMonoBehaviour.target.GetComponent<Player>();
            if (targetPlayer == null || targetPlayer.isDead)
            {
                animator.CrossFade("Idle", 0.05f);
                return;
            }

            // 공격 애니메이션이 끝났다면 Idle로(연속 공격 방지)
            if (waitingForAttackAnimEnd && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                animator.CrossFade("Idle", 0.05f);
                waitingForAttackAnimEnd = false;
                return;
            }

            // 타겟 방향으로 회전
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

            tAttackTimer -= Time.deltaTime;

            // 공격 쿨타임 도달시 애니메이션 플레이 + 애니메이션 끝날 때까지 대기
            if (tAttackTimer <= 0 && !waitingForAttackAnimEnd)
            {
                tAttackTimer = tMonoBehaviour.attackCooldown;
                animator.Play("Attack", 0, 0f);
                waitingForAttackAnimEnd = true;
            }
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            tMonoBehaviour.DisableWeaponCollider();
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
