using UnityEngine;

namespace IdleRPG
{
    public class PlayerSMBIdle : SceneLinkedSMB<Player>
    {
        float p_CheckTimer;

        public float p_AutoTargetCheckTimeMin = 0.1f;
        public float p_AutoTargetCheckTimeMax = 0.5f;
        public Transform newTarget;
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            p_CheckTimer = Random.Range(p_AutoTargetCheckTimeMin, p_AutoTargetCheckTimeMax);
            tMonoBehaviour.isMoving = false;
            tMonoBehaviour.isAttacking = false;
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            p_CheckTimer -= Time.deltaTime;
            if (p_CheckTimer <= 0)
            {
                newTarget = AutoFindNearestMonster();
                if (newTarget != null)
                {
                    tMonoBehaviour.target = newTarget.gameObject;
                    animator.CrossFade("Move", 0.05f);
                    return;
                }
                p_CheckTimer = Random.Range(p_AutoTargetCheckTimeMin, p_AutoTargetCheckTimeMax);
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
                if (monsterComp.isDead) continue; // 죽은 몬스터는 제외

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