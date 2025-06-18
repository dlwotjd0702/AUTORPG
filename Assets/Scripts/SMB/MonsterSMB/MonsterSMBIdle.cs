using UnityEngine;

namespace IdleRPG
{
    public class MonsterSMBIdle : SceneLinkedSMB<Monster>
    {
        float tCheckTimer;
        public float tAutoTargetCheckTimeMin = 0.7f;
        public float tAutoTargetCheckTimeMax = 2.0f;

        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            tCheckTimer = Random.Range(tAutoTargetCheckTimeMin, tAutoTargetCheckTimeMax);
            tMonoBehaviour.isMoving = false;
            tMonoBehaviour.isAttacking = false;
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            tCheckTimer -= Time.deltaTime;
            if (tCheckTimer <= 0)
            {
                var newTarget = AutoFindNearestPlayer();
                if (newTarget != null)
                {
                    tMonoBehaviour.target = newTarget.gameObject;
                    animator.CrossFade("Move", 0.05f);
                    return;
                }
                tCheckTimer = Random.Range(tAutoTargetCheckTimeMin, tAutoTargetCheckTimeMax);
            }
        }

        Transform AutoFindNearestPlayer()
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length == 0) return null;
            GameObject nearest = null;
            float minDist = float.MaxValue;
            foreach (var p in players)
            {
                var playerComp = p.GetComponent<Player>();
                if (playerComp == null) continue;
                if (playerComp.isDead) continue; // << 죽은 플레이어는 제외

                float d = Vector3.Distance(tMonoBehaviour.transform.position, p.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    nearest = p;
                }
            }
            return nearest?.transform;
        }
    }
}