using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 5f;
    private int waypointIndex = 0;

    bool hasReachedEnd = false;
    private bool isInCombat = false;

    Vector3 lastPosition;
    Vector3 velocity;

    private EnemyHealth stats;

    [Header("Rotation")]
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] Vector3 rotationOffset;

    void Awake()
    {
        stats = GetComponent<EnemyHealth>();
    }

    void OnEnable()
    {
        waypointIndex = 0;

        hasReachedEnd = false;

        isInCombat = false;

        velocity = Vector3.zero;

        lastPosition = transform.position;
    }

    void Update()
    {
        Move();
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }

    void Move()
    {

        if (hasReachedEnd || isInCombat)
            return;

        if (stats.IsStunned())
            return;

        if (waypointIndex >= EnemyPath.instance.WaypointCount())
        {
            ReachEnd();
            return;
        }

        Transform target = EnemyPath.instance.GetWaypoint(waypointIndex);

        Vector3 dir = target.position - transform.position;

        float speed = stats.moveSpeed * stats.GetSlowMultiplier();

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(dir.normalized)
                * Quaternion.Euler(rotationOffset);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            waypointIndex++;
        }
    }

    void ReachEnd()
    {
        WaveManager.instance.RegisterEnemyDeath();

        hasReachedEnd = true;

        if (BaseHealth.instance != null)
            BaseHealth.instance.TakeDamage(1);

        EnemyHealth eh = GetComponent<EnemyHealth>();

        if (eh != null)
        {
            GameObject prefab = eh.GetPrefab();

            if (prefab != null && ObjectPool.instance != null)
            {
                ObjectPool.instance.ReturnObject(gameObject, prefab);
            }
        }
    }

    public float GetProgress()
    {
        return waypointIndex;
    }
    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public void SetCombat(bool value)
    {
        isInCombat = value;
    }
}