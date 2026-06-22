using UnityEngine;

public enum EnemyState
{
    Moving,
    Combat,
    Dead
}

public class EnemyMovement : MonoBehaviour
{
    public EnemyState state = EnemyState.Moving;
    public bool IsDead => state == EnemyState.Dead;

    private EnemyHealth stats;
    private UnitAnimatorController anim;

    private Vector3 lastPosition;
    private Vector3 velocity;
    private Transform combatTarget;

    private int waypointIndex = 0;

    void Awake()
    {
        stats = GetComponent<EnemyHealth>();
        anim = GetComponentInChildren<UnitAnimatorController>();
    }

    void OnEnable()
    {
        ResetEnemy();

        waypointIndex = 0;
        state = EnemyState.Moving;

        velocity = Vector3.zero;
        lastPosition = transform.position;

        if (anim != null)
        {
            anim.SetMoving(false);
            anim.SetSpeed(0f);
        }
    }
    void Update()
    {
        if (state == EnemyState.Dead)
            return;
        if (state == EnemyState.Combat)
        {
            anim.PlayAttack();
            return;
        }

        HandleMovement();
        UpdateAnimation();

        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }
    void ReachEnd()
    {
        Debug.Log("INIMIGO CHEGOU AO FINAL");

        if (WaveManager.instance != null)
            WaveManager.instance.RegisterEnemyDeath();

        if (BaseHealth.instance != null)
            BaseHealth.instance.TakeDamage(1);

        EnemyHealth eh = GetComponent<EnemyHealth>();

        if (eh == null)
        {
            Debug.LogError("EnemyHealth NULL");
            gameObject.SetActive(false);
            return;
        }

        GameObject prefab = eh.GetPrefab();

        if (prefab == null)
        {
            Debug.LogError("PrefabReference NULL");
            gameObject.SetActive(false);
            return;
        }

        if (ObjectPool.instance == null)
        {
            Debug.LogError("ObjectPool NULL");
            gameObject.SetActive(false);
            return;
        }

        ObjectPool.instance.ReturnObject(gameObject, prefab);
    }
    void HandleMovement()
    {
        if (state != EnemyState.Moving)
            return;

        if (stats.IsStunned())
            return;

        if (EnemyPath.instance == null)
            return;

        if (waypointIndex >= EnemyPath.instance.WaypointCount())
        {
            ReachEnd();
            return;
        }

        Transform target = EnemyPath.instance.GetWaypoint(waypointIndex);

        Vector3 dir = target.position - transform.position;
        float moveSpeed = stats.moveSpeed * stats.GetSlowMultiplier();

        transform.position += dir.normalized * moveSpeed * Time.deltaTime;

        if (dir.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                10f * Time.deltaTime
            );
        }

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
            waypointIndex++;
    }

    void UpdateAnimation()
    {
        if (anim == null) return;

        bool moving =
            state == EnemyState.Moving &&
            !stats.IsStunned();

        anim.SetMoving(moving);
        anim.SetSpeed(velocity.magnitude);
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }
    public void SetCombat(bool value, Transform target = null)
    {
        state = value ? EnemyState.Combat : EnemyState.Moving;
        combatTarget = target;

        if (!value)
            combatTarget = null;
    }
    public void ResetEnemy()
    {
        state = EnemyState.Moving;
        waypointIndex = 0;
        combatTarget = null;
    }
}