using UnityEngine;
using System;
using Combat;

namespace IdleRPG
{
    [DefaultExecutionOrder(100)]
    public class Monster : MonoBehaviour, IDamageable,IAttackStat
    {
        public float moveSpeed = 3f;
        public float attackRange = 1.5f;
        public float attackCooldown = 1.2f;
        public float maxHp = 5;
        public int expReward = 10;
        public int goldReward = 20;
        public string equipmentDropId;
        public string skillDropId;

        public Animator animator;
        public Collider weaponCollider; // 몬스터가 무기 콜라이더 갖는 경우

        public float currentHp;
        [HideInInspector] public bool isAttacking = false;
        [HideInInspector] public bool isMoving = false;
        [HideInInspector] public GameObject target;
        public bool isDead;

        public float attackPower = 5f; // 또는 계산된 값
        public float GetAttackPower() => attackPower;
        // ⭐️ 이벤트 선언 (풀링/스테이지 매니저에서 구독)
        public event Action<Monster> OnMonsterDeath;

        void Awake()
        {
            if (!animator) animator = GetComponent<Animator>();
            if (weaponCollider) weaponCollider.enabled = false;
            // 풀링 구조라면 currentHp 초기화는 Spawn/reset에서!
        }
        private void OnEnable()
        {
            animator.applyRootMotion = false;
            isDead=false;
            if (animator == null)
                animator = GetComponent<Animator>();

            SceneLinkedSMB<Monster>.Initialise(animator, this);
        }

        void Start()
        {
            SceneLinkedSMB<Monster>.Initialise(animator, this);
        }

        public void TakeDamage(float amount)
        {
            Debug.Log($"{amount}TakeDamage");
            currentHp -= amount;
            if (currentHp <= 0)
            {
                currentHp = 0;
                animator.CrossFade("Death", 0.05f);
                Deathmotion();
            }
            // else { animator.SetTrigger("Hit"); }
        }

        public void Deathmotion()
        {
            animator.applyRootMotion = true;
            isDead=true;
            Invoke(nameof(OnDeath), 2f);
        }
        public void OnDeath()
        {
            OnMonsterDeath?.Invoke(this);
            
        }

        // StageManager/MonsterPool에서 호출할 함수
        public void ResetMonster()
        {
            currentHp = maxHp;
            isAttacking = false;
            isMoving = false;
            // 위치/애니메이션 초기화 등 필요시 추가
            gameObject.SetActive(true);
        }

        // ----------- [콜라이더 ON/OFF] -----------
        public void EnableWeaponCollider()
        {
            if (weaponCollider) weaponCollider.enabled = true;
        }

        public void DisableWeaponCollider()
        {
            if (weaponCollider) weaponCollider.enabled = false;
        }
    }
}
