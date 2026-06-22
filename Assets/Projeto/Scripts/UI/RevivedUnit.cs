using UnityEngine;

public class RevivedUnit : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    private float health;

    public float damage = 10f;
    public float moveSpeed = 3f;
    public float attackRate = 1f;
    public float attackRange = 1.5f;
    public float range = 5f;

    public bool isRanged;
    public bool isMagic;

    [Header("Damage Popup")]
    public GameObject damagePopupPrefab;

    [Header("Range Visual")]
    public GameObject rangeIndicatorPrefab;

    private GameObject rangeIndicatorInstance;

    private Transform target;
    private Vector3 startPosition;

    private float attackCooldown;
    private bool isFighting = false;

    private EnemyMovement enemyMovement;
    private EnemyHealth enemyHealth;
    private UnitAnimatorController anim;

    public float leashDistance = 5f;
    public float detectRange = 5f;
    private bool isAttacking;
    private bool canAttack = false;

    public float CurrentHealth => health;
    void OnEnable()
    {
        health = maxHealth;
        startPosition = transform.position;

        target = null;
        enemyHealth = null;
        attackCooldown = 0f;

        transform.position = new Vector3(
            transform.position.x,
            1.5f,
            transform.position.z
        );
    }
    void Start()
    {
        health = maxHealth;
        startPosition = transform.position;

    }
    void Update()
    {
        if (anim == null)
            anim = GetComponentInChildren<UnitAnimatorController>();

        if (target == null)
        {
            FindTarget();
            ReturnToPosition();
            return;
        }

        if (!IsTargetValid())
        {
            ClearTarget();
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > attackRange)
        {
            MoveTo(target.position);
            return;
        }

        Attack();
    }

    void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        Transform best = null;
        float closest = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy"))
                continue;

            EnemyHealth eh = hit.GetComponent<EnemyHealth>();

            if (eh == null || eh.CurrentHealth <= 0)
                continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);

            if (dist < closest)
            {
                closest = dist;
                best = hit.transform;
            }
        }

        if (best == null)
            return;

        target = best;
        enemyHealth = target.GetComponent<EnemyHealth>();
        enemyMovement = target.GetComponent<EnemyMovement>();

        if (enemyMovement != null)
            enemyMovement.SetCombat(true);
    }
    void Attack()
    {
        attackCooldown -= Time.deltaTime;

        if (attackCooldown > 0f)
            return;

        if (enemyHealth == null)
            return;

        anim.PlayAttack();

        attackCooldown = 1f / attackRate;
    }
    void ClearTarget()
    {
        if (enemyMovement != null)
            enemyMovement.SetCombat(false);

        target = null;
        enemyMovement = null;
        enemyHealth = null;
    }

    void MoveTo(Vector3 position)
    {
        Vector3 dir = (position - transform.position).normalized;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(dir);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rot,
                10f * Time.deltaTime
            );
        }
    }
    void EndCombat()
    {
        if (enemyMovement != null)
            enemyMovement.SetCombat(false);

        target = null;
        enemyMovement = null;
        enemyHealth = null;
        attackCooldown = 0f;
    }
    void ReturnToPosition()
    {
        if (Vector3.Distance(transform.position, startPosition) > 0.2f)
            MoveTo(startPosition);
    }
    public void TakeDamage(float amount)
    {
        health -= amount;

        ShowDamagePopup((int)amount);

        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    void Die()
    {
        if (enemyMovement != null)
            enemyMovement.SetCombat(false);

        enemyMovement = null;
        enemyHealth = null;
        target = null;

        attackCooldown = 0f;

        enabled = false;

        if (anim != null)
            anim.PlayDeath();
    }
    void ShowDamagePopup(int damage)
    {
        if (damagePopupPrefab == null) return;

        GameObject popupGO = ObjectPool.instance.GetObject(damagePopupPrefab);

        popupGO.transform.position = transform.position + Vector3.up * 2f;

        DamagePopup popup = popupGO.GetComponent<DamagePopup>();

        popup.Setup(damage, Color.gray, damagePopupPrefab);
    }
    bool IsTargetValid()
    {
        return target != null &&
               target.gameObject.activeInHierarchy &&
               enemyHealth != null &&
               enemyHealth.CurrentHealth > 0;
    }
    public void SetHomePosition(Vector3 pos)
    {
        startPosition = pos;
    }
   
    public void OnAnimationAttackHit()
    {
        if (enemyHealth != null)
            enemyHealth.TakeDamage(damage, isMagic, false);
    }
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
    }
}
