using UnityEngine;

namespace IdleRPG
{
    public class PlayerSMBIdle : SceneLinkedSMB<Player>
    {
        public Transform newTarget;

        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            tMonoBehaviour.isMoving = false;
            tMonoBehaviour.isAttacking = false;
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            newTarget = AutoFindNearestMonster();
            if (newTarget != null)
            {
                tMonoBehaviour.target = newTarget.gameObject;

                float dist = Vector3.Distance(tMonoBehaviour.transform.position, newTarget.position);
                float attackRange = tMonoBehaviour.attackRange;

                if (dist <= attackRange)
                {
                    animator.CrossFade("Attack", 0.05f);
                }
                else
                {
                    animator.CrossFade("Move", 0.05f);
                }
            }
        }

        Transform AutoFindNearestMonster()
        {
            var monsters = GameObject.FindGameObjectsWithTag("Monster");
            if (monsters.Length == 0) return null;
            GameObject nearest = null;
            float minDist = float.MaxValue;
            foreach (var m in monsters)
            {
                var monsterComp = m.GetComponent<Monster>();
                if (monsterComp == null) continue;
                if (monsterComp.isDead) continue;

                float d = Vector3.Distance(tMonoBehaviour.transform.position, m.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    nearest = m;
                }
            }
            return nearest?.transform;
        }
    }
}