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

    private Transform target;
    private Vector3 startPosition;

    private float attackCooldown;
    private bool isFighting = false;

    private EnemyMovement enemyMovement;
    private EnemyHealth enemyHealth;

    public float leashDistance = 10f;

    public float CurrentHealth => health;
    void OnEnable()
    {
        health = maxHealth;
        startPosition = transform.position;

        target = null;
        isFighting = false;
        attackCooldown = 0f;

        enemyMovement = null;
        enemyHealth = null;
    }

    void Update()
    {
        if (isFighting)
        {
            HandleCombat();
        }
        else
        {
            ReturnToPosition();

            if (Vector3.Distance(transform.position, startPosition) < 0.2f)
            {
                FindTarget();
            }
        }
    }

    void FindTarget()
    {
        EnemyManager.instance.Cleanup();
        float closestDistance = Mathf.Infinity;
        Transform bestTarget = null;
        foreach (Transform enemy in EnemyManager.instance.enemies)
        {
            if (enemy == null)
                continue;

            if (!enemy.gameObject.activeInHierarchy)
                continue;

            EnemyHealth eh = enemy.GetComponent<EnemyHealth>();

            if (eh == null)
                continue;

            if (eh.CurrentHealth <= 0)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                enemy.position
            );

            if (distance <= range && distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = enemy;
            }
        }

        if (bestTarget != null)
        {
            target = bestTarget;
            isFighting = true;

            enemyMovement = target.GetComponent<EnemyMovement>();
            enemyHealth = target.GetComponent<EnemyHealth>();

            if (enemyMovement != null)
                enemyMovement.SetCombat(true);

            if (enemyHealth != null)
                enemyHealth.SetTarget(transform);
        }
    }

    void HandleCombat()
    {
        if (!IsTargetValid())
        {
            EndCombat();
            return;
        }

        float distanceFromHome =
    Vector3.Distance(
        transform.position,
        startPosition
    );

        if (distanceFromHome > leashDistance)
        {
            EndCombat();
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > attackRange)
        {
            MoveTo(target.position);
        }
        else
        {
            Attack();
        }
    }

    void MoveTo(Vector3 position)
    {
        Vector3 dir =
            (position - transform.position).normalized;

        transform.position +=
            dir * moveSpeed * Time.deltaTime;

        if (dir.sqrMagnitude > 0.001f)
        {
            transform.rotation =
                Quaternion.LookRotation(dir);
        }
    }

    void Attack()
    {
        attackCooldown -= Time.deltaTime;

        if (attackCooldown > 0f)
            return;

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage, isMagic, false);
        }

        attackCooldown = 1f / attackRate;
    }

    void EndCombat()
    {
        if (enemyMovement != null)
            enemyMovement.SetCombat(false);

        target = null;
        enemyMovement = null;
        enemyHealth = null;

        isFighting = false;

        ReturnToPosition();
    }

    void ReturnToPosition()
    {
        if (Vector3.Distance(transform.position, startPosition) > 0.2f)
        {
            MoveTo(startPosition);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        ShowDamagePopup((int)amount);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {

        if (target != null)
        {
            EnemyMovement em = target.GetComponent<EnemyMovement>();
            if (em != null)
                em.SetCombat(false);
        }

        gameObject.SetActive(false);
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
        if (target == null)
            return false;

        if (!target.gameObject.activeInHierarchy)
            return false;

        if (enemyHealth == null)
            return false;

        if (enemyHealth.CurrentHealth <= 0)
            return false;

        return true;
    }
}
