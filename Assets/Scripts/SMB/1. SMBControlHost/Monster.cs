using System;
using Combat;
using UnityEngine;

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
        private HPBarUI hpBarUI;
        // 상태이상 관련
        private bool isFrozen = false;
        private float freezeTimer = 0f;

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
            isFrozen = false;
            freezeTimer = 0f;
            if (animator == null)
                animator = GetComponent<Animator>();
            SceneLinkedSMB<Monster>.Initialise(animator, this);
        }

        void Start()
        {
            SceneLinkedSMB<Monster>.Initialise(animator, this);
        }

        public void SetPrefabIndex(int idx) => prefabIndex = idx;

        public void ApplyStats(float maxHp, int expReward, int goldReward, float attackPower)
        {
            this.maxHp = maxHp;
            this.currentHp = maxHp;
            this.expReward = expReward;
            this.goldReward = goldReward;
            this.attackPower = attackPower;
            hpBarUI = HPBarUIPool.Instance.Spawn(gameObject, currentHp, maxHp);
            hpBarUI.SetHP(currentHp, maxHp);
        }

        public void TakeDamage(float amount, bool isCritical = false)
        {
            currentHp -= amount;
            Vector3 pos = transform.position + Vector3.up * 2f;
            Color textColor = isCritical ? Color.yellow : Color.white;
            DamageText3DPool.Instance.Spawn(pos, (int)amount, textColor);
            if (hpBarUI != null)
                hpBarUI.SetHP(currentHp, maxHp);
            if (currentHp <= 0 && !isDead)
            {
                currentHp = 0;
                if (hpBarUI != null) hpBarUI.Release();
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
            isFrozen = false;
            freezeTimer = 0f;
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

        // ----------- 상태이상 예시 (빙결 등) -----------
        public void ApplyCrowdControl(string type, float duration)
        {
            if (type == "Freeze")
            {
                isFrozen = true;
                freezeTimer = duration;
                // 애니메이션 멈춤/색상 등 연출
                if (animator) animator.speed = 0f;
            }
            // 필요시 다른 상태이상도 추가
        }

        private void Update()
        {
            if (isFrozen)
            {
                freezeTimer -= Time.deltaTime;
                if (freezeTimer <= 0f)
                {
                    isFrozen = false;
                    if (animator) animator.speed = 1f;
                }
            }
        }

    }
}
