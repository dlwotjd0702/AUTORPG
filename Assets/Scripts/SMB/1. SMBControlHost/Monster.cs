using UnityEngine;
using System;
using Combat;

namespace IdleRPG
{
    [DefaultExecutionOrder(100)]
    public class Monster : MonoBehaviour, IDamageable, IAttackStat
    {
        public float moveSpeed = 3f;
        public float attackRange = 1.5f;
        public float attackCooldown = 1.2f;

        public float maxHp = 5;
        public int expReward = 10;
        public int goldReward = 20;
        public float attackPower = 5f;

        public int prefabIndex { get; set; }
        public Animator animator;
        public Collider weaponCollider;

        public float currentHp;
        [HideInInspector] public bool isAttacking = false;
        [HideInInspector] public bool isMoving = false;
        [HideInInspector] public GameObject target;
        public bool isDead;

        public float GetAttackPower() => attackPower;
        public event Action<Monster> OnMonsterDeath;

        void Awake()
        {
            if (!animator) animator = GetComponent<Animator>();
            if (weaponCollider) weaponCollider.enabled = false;
        }

        private void OnEnable()
        {
            animator.applyRootMotion = false;
            isDead = false;
            if (animator == null)
                animator = GetComponent<Animator>();
            SceneLinkedSMB<Monster>.Initialise(animator, this);
        }

        void Start()
        {
            SceneLinkedSMB<Monster>.Initialise(animator, this);
        }

        // 풀에서만 호출!
        public void SetPrefabIndex(int idx) => prefabIndex = idx;

        // 스테이지매니저에서 호출!
        public void ApplyStats(float maxHp, int expReward, int goldReward, float attackPower)
        {
            this.maxHp = maxHp;
            this.currentHp = maxHp;
            this.expReward = expReward;
            this.goldReward = goldReward;
            this.attackPower = attackPower;
        }

        public void TakeDamage(float amount)
        {
            currentHp -= amount;
            if (currentHp <= 0 && !isDead)
            {
                currentHp = 0;
                animator.CrossFade("Death", 0.05f);
                Deathmotion();
            }
        }

        public void Deathmotion()
        {
            animator.applyRootMotion = true;
            isDead = true;
            Invoke(nameof(OnDeath), 2f);
        }
        public void OnDeath()
        {
            OnMonsterDeath?.Invoke(this);
        }

        public void ResetMonster()
        {
            isAttacking = false;
            isMoving = false;
            isDead = false;
            currentHp = maxHp;
            if (weaponCollider) weaponCollider.enabled = false;
            gameObject.SetActive(true);
        }

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
