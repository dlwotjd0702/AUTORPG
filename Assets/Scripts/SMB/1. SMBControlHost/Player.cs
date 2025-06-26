using Combat;
using Stats;
using UnityEngine;

namespace IdleRPG
{
   
    public class Player : MonoBehaviour, IDamageable, IAttackStat
    {
        [Header("Base Move & Range")]
        public float moveSpeed = 5f;
        public float attackRange = 2f;

        public Animator animator;
        public Collider weaponCollider;

        [Header("컴포넌트 참조")]
        public PlayerStats playerStats;

        [Header("상태")]
        public bool isDead = false;
        public float currentHp;
        [HideInInspector] public float MaxHp;
        [HideInInspector] public bool isAttacking = false;
        [HideInInspector] public bool isMoving = false;
        [HideInInspector] public GameObject target;
        public float FinalAttack => playerStats.FinalAttack;
        public float FinalAtkSpeed => playerStats.FinalAtkSpeed;
        public float FinalHp => playerStats.FinalHp;
        public float FinalDefense => playerStats.FinalDefense;
        public float FinalCritRate => playerStats.FinalCritRate;
        public float FinalCritDmg => playerStats.FinalCritDmg;
        private HPBarUI hpBarUI;
        void Awake()
        {
            if (!animator) animator = GetComponent<Animator>();
            if (weaponCollider) weaponCollider.enabled = false;
            if (!playerStats) playerStats = GetComponent<PlayerStats>();
            
        }

        private void OnEnable()
        {
            animator.applyRootMotion = false;
            isDead = false;
            if (!animator) animator = GetComponent<Animator>();
            if (!playerStats) playerStats = GetComponent<PlayerStats>();

            SceneLinkedSMB<Player>.Initialise(animator, this);

            playerStats.OnStatsChanged += ApplyStatsToPlayer;
            playerStats.RefreshStats();
            currentHp = MaxHp;
            hpBarUI = HPBarUIPool.Instance.Spawn(this.transform, currentHp, MaxHp);
            if (hpBarUI != null)hpBarUI.SetColor(true);
        }

     

        void OnDisable()
        {
            if (hpBarUI != null)
            {
                hpBarUI.SetColor(false);
                hpBarUI.Release();
            }
            playerStats.OnStatsChanged -= ApplyStatsToPlayer;
        }

        public float GetAttackPower()=>FinalAttack;
        

        void ApplyStatsToPlayer()
        {
            // HP 변화량만큼 currentHp 보정 (체력 증가 시 회복 포함)
            currentHp += (FinalHp - MaxHp);
            MaxHp = FinalHp;
            if (currentHp > MaxHp) currentHp = MaxHp;
            if (hpBarUI != null)hpBarUI.SetHP(currentHp, MaxHp);
        }
        
        public void TakeDamage(float amount, bool isCritical = false)
        {
            float damage = Mathf.Max(amount - playerStats.FinalDefense, 1);
            currentHp -= damage;
            if (hpBarUI != null)
                hpBarUI.SetHP(currentHp, MaxHp);
            
            if (currentHp <= 0)
            {
                if (hpBarUI != null) hpBarUI.Release();
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
            Invoke(nameof(spawn), 5f);
        }
        public void OnDeath()
        {
            var stageManager = FindObjectOfType<StageManager>();
            if (stageManager != null)
                stageManager.OnPlayerDied();
            gameObject.SetActive(false);
        }
        public void spawn()
        {
            gameObject.SetActive(true);
        }


        public void EnableWeaponCollider()
        {
            if (weaponCollider) weaponCollider.enabled = true;
        }

        public void DisableWeaponCollider()
        {
            isAttacking = false;
            if (weaponCollider) weaponCollider.enabled = false;
        }

        
      
    }
}
