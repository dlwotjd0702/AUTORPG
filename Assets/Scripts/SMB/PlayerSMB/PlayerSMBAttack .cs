using UnityEngine;

namespace IdleRPG
{
    public class PlayerSMBAutoAttack : SceneLinkedSMB<Player>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            tMonoBehaviour.isAttacking = true;

            // FinalAtkSpeed를 AttackSpeed 파라미터로 넘김 (1.0 = 기본속도)
            animator.SetFloat("AttackSpeed", tMonoBehaviour.playerStats.FinalAtkSpeed);

            // 사거리 내면 바로 타겟 바라보고 Attack (애니메이션 Loop 전제)
            if (tMonoBehaviour.target)
            {
                RotateTowardsTarget(tMonoBehaviour.transform, tMonoBehaviour.target.transform);
            }
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            RotateTowardsTarget(tMonoBehaviour.transform, tMonoBehaviour.target.transform);
            if(tMonoBehaviour.isAttacking) return;
            
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

            // 사거리 밖이면 Move
            float dist = Vector3.Distance(
                tMonoBehaviour.transform.position,
                tMonoBehaviour.target.transform.position
            );
            if (dist > tMonoBehaviour.attackRange)
            {
                animator.CrossFade("Move", 0.05f);
                return;
            }

            // 타겟 바라보기
            
            // 애니메이션이 루프하므로 별도 Play 필요 없음 (Animation Event로 데미지처리)
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            tMonoBehaviour.DisableWeaponCollider();

        }

        void RotateTowardsTarget(Transform self, Transform target, float rotationSpeed = 15f)
        {
            Vector3 direction = (target.position - self.position).normalized;
            direction.y = 0f;
            if (direction == Vector3.zero) return;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            self.rotation = Quaternion.Slerp(self.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
