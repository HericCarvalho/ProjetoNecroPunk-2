using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    #region === STATS ===

    [Header("Stats")]
    public float maxHealth = 100f;
    public float damage = 5f;
    public float attackRate = 1f;
    public float attackRange = 1.5f;
    public float moveSpeed = 5f;

    #endregion

    #region === RESISTANCES ===

    [Header("Resistances (0 a 1)")]
    [Range(0f, 1f)] public float physicalResistance;
    [Range(0f, 1f)] public float magicalResistance;

    #endregion

    #region === ATTACK TYPE ===

    [Header("Attack Type")]
    public bool isRanged;
    public bool isMagical;

    #endregion

    #region === EFFECTS STATE ===

    private bool isBurning;
    private float burnTimer;
    private float burnDamagePerSecond;

    private bool isSlowed;
    private float slowTimer;
    private float slowMultiplier = 1f;

    private bool isStunned;
    private float stunTimer;

    #endregion

    #region === EFFECT RESISTANCE & IMMUNITY ===

    [Header("Effect Resistances")]
    [Range(0f, 1f)] public float burnResistance;
    [Range(0f, 1f)] public float slowResistance;
    [Range(0f, 1f)] public float stunResistance;

    [Header("Effect Immunities")]
    public bool immuneToBurn;
    public bool immuneToSlow;
    public bool immuneToStun;

    #endregion

    #region === VISUAL EFFECTS ===

    [Header("Visual Effects")]
    public GameObject burnEffect;
    public GameObject slowEffect;
    public GameObject stunEffect;

    #endregion

    #region === ECONOMY / META ===

    [Header("Fragment Drop")]
    public string enemyID;
    [Range(0f, 1f)] public float fragmentDropChance = 0.3f;

    #endregion

    #region === UI ===

    [Header("Damage Popup")]
    public GameObject damagePopupPrefab;

    #endregion

    #region === RUNTIME ===

    private UnitAnimatorController anim;
    private float health;
    private float attackCooldown;
    private bool isAttacking;
    private Transform combatTarget;

    private float burnPopupTimer;
    private GameObject prefabReference;
    bool isDead;

    #endregion

    #region === UNITY EVENTS ===

    void OnEnable()
    {
        if (anim == null)
            anim = GetComponentInChildren<UnitAnimatorController>();

        ResetState();
        RegisterSystems();
    }

    void Update()
    {
        HandleEffects();
        HandleCombat();
    }

    #endregion

    #region === INITIALIZATION ===

    void ResetState()
    {
        isDead = false;

        health = maxHealth;

        isBurning = false;
        isSlowed = false;
        isStunned = false;

        if (burnEffect != null) burnEffect.SetActive(false);
        if (slowEffect != null) slowEffect.SetActive(false);
        if (stunEffect != null) stunEffect.SetActive(false);
    }

    void RegisterSystems()
    {
        if (EnemyManager.instance != null)
            EnemyManager.instance.RegisterEnemy(transform);

        if (EncyclopediaManager.instance != null && !string.IsNullOrEmpty(enemyID))
            EncyclopediaManager.instance.Discover(enemyID);
    }

    #endregion

    #region === DAMAGE SYSTEM ===

    public void TakeDamage(float amount, bool isMagicDamage, bool isTrueDamage = false)
    {
        float finalDamage = amount;

        if (!isTrueDamage)
        {
            finalDamage *= isMagicDamage
                ? (1f - magicalResistance)
                : (1f - physicalResistance);

            ShowDamagePopup((int)finalDamage, isMagicDamage, false);
        }
        else
        {
            ShowDamagePopup((int)finalDamage, false, true);
        }

        health -= finalDamage;

        if (health <= 0)
            Die();
    }

    #endregion

    #region === COMBAT ===

    public void SetTarget(Transform target)
    {
        combatTarget = target;
    }
    public void OnAnimationAttackHit()
    {
        if (combatTarget == null) return;

        RevivedUnit unit = combatTarget.GetComponent<RevivedUnit>();

        if (unit != null)
            unit.TakeDamage(damage);
    }
    void HandleCombat()
    {
        if (combatTarget == null)
            return;

        float distance = Vector3.Distance(transform.position, combatTarget.position);

        if (distance > attackRange)
        {
            isAttacking = false;
            return;
        }

        attackCooldown -= Time.deltaTime;

        if (attackCooldown > 0f)
            return;

        if (!isAttacking)
        {
            StartAttack();
        }
    }
    void Attack()
    {
        if (anim != null)
            anim.PlayAttack();

        RevivedUnit unit = combatTarget.GetComponent<RevivedUnit>();

        if (unit != null)
            unit.TakeDamage(damage);

        attackCooldown = 1f / attackRate;
    }
    void StartAttack()
    {
        isAttacking = true;
        Debug.Log("ATAQUE INICIADO");
        if (anim != null)
            anim.PlayAttack();

        attackCooldown = 1f / attackRate;
    }

    #endregion

    #region === EFFECT SYSTEM ===

    void HandleEffects()
    {
        HandleBurn();
        HandleSlow();
        HandleStun();
    }

    #endregion

    #region === BURN ===

    void HandleBurn()
    {
        if (!isBurning)
            return;

        burnTimer -= Time.deltaTime;

        float damageThisFrame = burnDamagePerSecond * Time.deltaTime;

        health -= damageThisFrame;

        if (health <= 0)
        {
            Die();
            return;
        }

        burnPopupTimer -= Time.deltaTime;

        if (burnPopupTimer <= 0f)
        {
            ShowDamagePopup((int)burnDamagePerSecond, false, true, true);
            burnPopupTimer = 0.5f;
        }

        if (burnTimer <= 0f)
        {
            isBurning = false;

            if (burnEffect != null)
                burnEffect.SetActive(false);
        }
    }

    public void ApplyBurn(float duration, float dps)
    {
        if (immuneToBurn) return;

        if (Random.value > (1f - burnResistance))
            return;

        isBurning = true;
        burnTimer = duration;
        burnDamagePerSecond = dps;

        if (burnEffect != null) burnEffect.SetActive(true);
    }

    #endregion

    #region === SLOW ===

    void HandleSlow()
    {
        if (!isSlowed) return;

        slowTimer -= Time.deltaTime;

        if (slowTimer <= 0f)
        {
            isSlowed = false;
            slowMultiplier = 1f;

            if (slowEffect != null) slowEffect.SetActive(false);
        }
    }

    public void ApplySlow(float duration, float multiplier)
    {
        if (immuneToSlow) return;

        if (Random.value > (1f - slowResistance))
            return;

        isSlowed = true;
        slowTimer = duration;
        slowMultiplier = multiplier;

        if (slowEffect != null) slowEffect.SetActive(true);
    }

    public float GetSlowMultiplier()
    {
        return slowMultiplier;
    }

    #endregion

    #region === STUN ===

    void HandleStun()
    {
        if (!isStunned) return;

        stunTimer -= Time.deltaTime;

        if (stunTimer <= 0f)
        {
            isStunned = false;

            if (stunEffect != null) stunEffect.SetActive(false);
        }
    }

    public void ApplyStun(float duration)
    {
        if (immuneToStun) return;

        if (Random.value > (1f - stunResistance))
            return;

        isStunned = true;
        stunTimer = duration;

        if (stunEffect != null) stunEffect.SetActive(true);
    }

    public bool IsStunned()
    {
        return isStunned;
    }

    #endregion

    #region === UI ===

    void ShowDamagePopup(int damage, bool isMagic, bool isTrue, bool isBurn = false)
    {
        if (damagePopupPrefab == null || ObjectPool.instance == null)
            return;

        GameObject popupGO = ObjectPool.instance.GetObject(damagePopupPrefab);
        popupGO.transform.position = transform.position + Vector3.up * 2f;

        DamagePopup popup = popupGO.GetComponent<DamagePopup>();

        Color color = Color.gray;

        if (isBurn) color = Color.red;
        else if (isTrue) color = Color.white;
        else if (isMagic) color = Color.blue;

        popup.Setup(damage, color, damagePopupPrefab);
    }

    #endregion

    #region === DEATH ===

    void Die()
    {
        if (isDead) return;

        isDead = true;

        health = 0;

        if (EnemyManager.instance != null)
            EnemyManager.instance.UnregisterEnemy(transform);

        if (WaveManager.instance != null)
            WaveManager.instance.RegisterEnemyDeath();

        if (EncyclopediaManager.instance != null)
            EncyclopediaManager.instance.RegisterKill(enemyID);

        EnemyReward reward = GetComponent<EnemyReward>();
        if (reward != null)
            reward.GiveReward();

        TryDropFragment();

        var move = GetComponent<EnemyMovement>();
        if (move != null)
        {
            move.enabled = false;
        }

        if (anim != null)
        {
            anim.SetSpeed(0f);
            anim.PlayDeath();
        }
    }
    public void OnDeathAnimationFinished()
    {
        if (ObjectPool.instance == null || prefabReference == null)
            return;

        ObjectPool.instance.ReturnObject(gameObject, prefabReference);
    }

    #endregion

    #region === DROP SYSTEM ===

    void TryDropFragment()
    {
        if (FragmentManager.instance == null)
            return;

        if (string.IsNullOrEmpty(enemyID))
            return;

        if (Random.value <= fragmentDropChance)
        {
            FragmentManager.instance.AddFragment(enemyID);
        }
    }

    #endregion

    #region === HELPERS ===

    public float CurrentHealth => health;

    public void SetPrefabReference(GameObject prefab)
    {
        prefabReference = prefab;
    }

    public GameObject GetPrefab()
    {
        return prefabReference;
    }

    #endregion
}