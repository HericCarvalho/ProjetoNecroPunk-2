using AudioSystem;
using TMPro;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 20f;
    public float damage = 50f;

    [Header("Damage Type")]
    public bool isMagicDamage;
    public bool isTrueDamage;

    [Header("Effects Chance")]
    public float burnChance;
    public float slowChance;
    public float stunChance;

    [Header("Effects Power")]
    public float burnDuration;
    public float burnDPS;
    public float slowDuration;
    public float slowMultiplier;
    public float stunDuration;

    private float baseDamage;

    [SerializeField] SoundData soundData;

    private Transform target;
    private EnemyHealth cachedEnemy;
    private GameObject prefabReference;
    private Tower ownerTower;

    Vector3 targetPosition;
    bool usePredicted = false;

    public void Seek(Transform _target, GameObject tower, GameObject prefab)
    {
        target = _target;
        prefabReference = prefab;
        usePredicted = false;

        if (target == null) return;

        EnemyMovement em = target.GetComponent<EnemyMovement>();
        Vector3 predictedPos = target.position;

        // Cálculo opcional de previsão de movimento para o disparo inicial
        if (em != null)
        {
            Vector3 enemyVelocity = em.GetVelocity();
            float distance = Vector3.Distance(transform.position, target.position);
            float timeToHit = distance / speed;
            predictedPos = target.position + enemyVelocity * timeToHit;
        }

        targetPosition = predictedPos;

        // Rotação inicial pura em direção ao ponto previsto
        Vector3 direction = (predictedPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        cachedEnemy = target.GetComponent<EnemyHealth>();

        if (tower != null)
            ownerTower = tower.GetComponent<Tower>();
    }

    public void SeekPosition(Vector3 pos, Transform _target, GameObject tower, GameObject prefab)
    {
        targetPosition = pos;
        target = _target;
        prefabReference = prefab;
        usePredicted = true;

        cachedEnemy = _target != null ? _target.GetComponent<EnemyHealth>() : null;
        ownerTower = tower != null ? tower.GetComponent<Tower>() : null;

        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void Awake()
    {
        baseDamage = damage;
    }

    void OnEnable()
    {
        damage = baseDamage;
        cachedEnemy = null;
        ownerTower = null;

        if (soundData != null)
        {
            SoundManager.Instance.CreateSoundBuilder()
                .WithGameObjectAsParent(this.transform)
                .WithRandomPitch()
                .Play(soundData);
        }
    }

    void Update()
    {
        // Se o alvo morrer antes do impacto, o míssil vai até à última posição conhecida
        if (target == null && !usePredicted)
        {
            // Opcional: Se preferir que o míssil suma imediatamente quando o alvo morre, descomente a linha abaixo:
            // ReturnToPool(); return;
        }
        else if (target != null && !usePredicted)
        {
            // Atualiza a posição do alvo em tempo real (Teleguiado puro)
            targetPosition = target.position;
        }

        // Calcula a direção DIRETA até ao inimigo (Sem offsets estranhos)
        Vector3 dir = targetPosition - transform.position;
        Vector3 move = dir.normalized * speed * Time.deltaTime;

        // Faz o projétil olhar diretamente para onde está a andar
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }

        // Move o projétil
        transform.position += move;

        // Verifica o impacto com base na distância que se moveu neste frame
        if (move.magnitude >= dir.magnitude)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        if (cachedEnemy != null)
        {
            cachedEnemy.TakeDamage(damage, isMagicDamage, isTrueDamage);
            LevelStatsManager.instance.RegisterDamage(ownerTower, damage);

            if (Random.value <= burnChance)
                cachedEnemy.ApplyBurn(burnDuration, burnDPS);

            if (Random.value <= slowChance)
                cachedEnemy.ApplySlow(slowDuration, slowMultiplier);

            if (Random.value <= stunChance)
                cachedEnemy.ApplyStun(stunDuration);

            if (ownerTower != null)
                ownerTower.GainXP(1);
        }

        ReturnToPool();
    }

    void ReturnToPool()
    {
        if (prefabReference == null)
        {
            Debug.LogWarning("Bullet sem prefabReference!");
            gameObject.SetActive(false);
            return;
        }

        ObjectPool.instance.ReturnObject(gameObject, prefabReference);
    }
}