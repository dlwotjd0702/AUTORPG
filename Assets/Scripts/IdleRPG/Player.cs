using UnityEngine;

namespace IdleRPG
{
    [DefaultExecutionOrder(100)]
    public class Player : MonoBehaviour, IDamageable
    {
        public float moveSpeed = 5f;
        public float attackRange = 2f;
        public float attackCooldown = 1f;

        public Animator animator;
        public Collider weaponCollider;

        [Header("Base Stats")]
        public int baseAttack = 5;
        public float baseAtkSpeed = 1.0f;
        public int baseDefense = 0;
        public int baseHp = 10;
        public float baseCritRate = 0.05f;   // 5%
        public float baseCritDmg = 1.5f;   

        [Header("레벨/경험치")]
        public int level = 1;
        public int exp = 0;
        public int expToLevelUp = 100;

        [Header("상태")]
        public bool isDead = false;
        [HideInInspector] public float currentHp;
        [HideInInspector] public float MaxHp;
        [HideInInspector] public bool isAttacking = false;
        [HideInInspector] public bool isMoving = false;
        [HideInInspector] public GameObject target;

        // 스탯 연동 시스템
        public PlayerStats playerStats;

        void Awake()
        {
            if (!animator) animator = GetComponent<Animator>();
            if (weaponCollider) weaponCollider.enabled = false;
            if (!playerStats) playerStats = GetComponent<PlayerStats>();
            currentHp = baseHp;
        }

        private void OnEnable()
        {
            animator.applyRootMotion = false;
            isDead = false;
            if (!animator) animator = GetComponent<Animator>();
            if (!playerStats) playerStats = GetComponent<PlayerStats>();

            SceneLinkedSMB<Player>.Initialise(animator, this);

            playerStats.RefreshStats();
            ApplyStatsToPlayer();
            playerStats.OnStatsChanged += ApplyStatsToPlayer;
        }

        void Start()
        {
            playerStats.RefreshStats();
            ApplyStatsToPlayer();
        }

        // 최종 스탯 PlayerStats에서 직접 참조하여 반영
        void ApplyStatsToPlayer()
        {
            currentHp += (FinalHp - MaxHp);
            MaxHp = FinalHp;
        }

        public void AddExp(int amount)
        {
            exp += amount;
            while (exp >= expToLevelUp)
            {
                exp -= expToLevelUp;
                LevelUp();
            }
        }

        void LevelUp()
        {
            level++;
            expToLevelUp += 50;
            baseHp += 5;
            baseAttack += 1;
            baseDefense += 1;

            playerStats.RefreshStats();
            ApplyStatsToPlayer();
            currentHp = playerStats.FinalHp;
        }

        public void TakeDamage(int amount)
        {
            float damage = Mathf.Max(amount - playerStats.FinalDefense, 1);
            currentHp -= damage;
            if (currentHp <= 0)
            {
                currentHp = 0;
                animator.CrossFade("Death", 0.05f);
                Deathmotion();
            }
        }

        public void Deathmotion()
        {
            isDead = true;
            animator.applyRootMotion = true;
            Invoke(nameof(OnDeath), 2f);
        }
        public void OnDeath()
        {
            gameObject.SetActive(false);
        }

        public void EnableWeaponCollider()
        {
            if (weaponCollider) weaponCollider.enabled = true;
        }

        public void DisableWeaponCollider()
        {
            if (weaponCollider) weaponCollider.enabled = false;
        }

      
        public float FinalAttack => playerStats.FinalAttack;
        public float FinalAtkSpeed => playerStats.FinalAtkSpeed;
        public float FinalHp => playerStats.FinalHp;
        public float FinalDefense => playerStats.FinalDefense;
        public float FinalCritRate => playerStats.FinalCritRate;
        public float FinalCritDmg => playerStats.FinalCritDmg;
    }
}
