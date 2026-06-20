using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    Projectile,
    Laser,
    Earthquake
}

public class Tower : MonoBehaviour
{
    public TowerData data;
    public EarthquakeAttack earthquakeAttack;

    public Transform head;
    public float rotationSpeed = 10f;
    public float rotationOffset = 90f;
    public Transform firePoint;
    public Transform target;

    public GameObject bulletPrefab;
    public GameObject rangeIndicator;

    public AttackType attackType;
    public TargetType towerType;

    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel;
    public int upgradePoints = 0;

    [Header("Upgrade Costs")]

    public int damageBaseCost = 5;
    public int rangeBaseCost = 5;
    public int fireRateBaseCost = 5;

    public float costMultiplier = 1.35f;

    float fireCountdown = 0f;

    float bonusDamage;
    float bonusRange;
    float bonusFireRate;

    int damageUpgradeLevel;
    int rangeUpgradeLevel;
    int fireRateUpgradeLevel;

    [Header("Animation")]

    [SerializeField] Animator animator;

    [SerializeField] bool useAttackAnimation = true;

    [SerializeField] string attackTrigger = "Attack";
    [SerializeField] string rotatingBool = "IsRotating";

    bool waitingAnimationShot = false;
    private bool hasTargetState = false;

    private float baseSize;
    private GameObject rangeIndicatorInstance;
    public bool isPreview = false;

    void Start()
    {
        if (isPreview)
            return;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        xpToNextLevel = data.baseXPToLevel;


        if (rangeIndicator != null)
        {
            rangeIndicatorInstance = Instantiate(rangeIndicator);

            Renderer r = rangeIndicatorInstance.GetComponent<Renderer>();
            if (r != null)
                baseSize = r.bounds.size.x;

            rangeIndicatorInstance.SetActive(false);
        }
    }

    void Update()
    {
        RotateTowardsTarget();

        if (isPreview)
            return;

        if (target == null)
            FindTarget();

        if (target != null)
        {
            float distance = Vector3.Distance(
                transform.position,
                target.position
            );

            if (distance > GetRange())
            {
                target = null;
            }
        }

        UpdateAnimationStates();


        if (attackType == AttackType.Laser)
        {
            if (target != null)
                LaserAttack();
            return;
        }

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / GetFireRate();
        }

        fireCountdown -= Time.deltaTime;
    }
    void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Shoot()
    {
        if (target == null)
            return;

        if (animator != null && useAttackAnimation)
        {
            waitingAnimationShot = true;
            animator.SetTrigger(attackTrigger);
        }
        else
        {
            ExecuteAttack();
        }
    }
    void ExecuteAttack()
    {
        switch (attackType)
        {
            case AttackType.Projectile:
                ProjectileAttack();
                break;

            case AttackType.Earthquake:
                EarthquakeAttack();
                break;

            case AttackType.Laser:
                LaserAttack();
                break;
        }
    }

    void ProjectileAttack()
    {
        GameObject bulletGO = ObjectPool.instance.GetObject(bulletPrefab);

        bulletGO.transform.position = firePoint.position;
        bulletGO.transform.rotation = firePoint.rotation;

        Bullet bullet = bulletGO.GetComponent<Bullet>();

        Vector3 predictedPos = GetPredictedPosition(target);

        bullet.SeekPosition(predictedPos, target, gameObject, bulletPrefab);

        bullet.damage = GetFinalDamage();
    }

    void LaserAttack()
    {
        if (target == null) return;

        EnemyHealth enemy = target.GetComponent<EnemyHealth>();
        if (enemy == null) return;

        float damage = GetBonusDamageValue() * Time.deltaTime;

        enemy.TakeDamage(damage, false, false);
        LevelStatsManager.instance.RegisterDamage(this, damage);
    }

    void EarthquakeAttack()
    {
        if (earthquakeAttack != null)
        {
            earthquakeAttack.Execute(
            transform.position,
            GetFinalDamage(),
            GetRange(),
            this
            );
        }
    }

    void FindTarget()
    {
        Transform bestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Transform enemy in EnemyManager.instance.enemies)
        {
            if (!enemy.gameObject.activeInHierarchy)
                continue;

            float distance = Vector3.Distance(transform.position, enemy.position);

            if (distance < shortestDistance && distance <= GetRange())
            {
                shortestDistance = distance;
                bestTarget = enemy;
            }
        }

        target = bestTarget;
    }

    public void GainXP(int amount)
    {
        currentXP += amount;

        while (currentXP >= xpToNextLevel)
            LevelUp();
    }

    public void LevelUp()
    {
        currentXP -= xpToNextLevel;
        level++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);
        upgradePoints++;
    }

    public float GetRange()
    {
        float baseValue = data.baseRange + (data.rangePerLevel * (level - 1)) + bonusRange;

        return SkillManager.instance.GetStat(
            StatType.Range,
            towerType,
            baseValue
        );
    }

    public float GetFireRate()
    {
        float baseValue = data.baseFireRate + (data.fireRatePerLevel * (level - 1)) + bonusFireRate;

        return SkillManager.instance.GetStat(
            StatType.FireRate,
            towerType,
            baseValue
        );
    }

    public float GetFinalDamage()
    {
        float baseDamage = GetBaseDamage();

        float damageWithSkills = SkillManager.instance.GetStat(
            StatType.Damage,
            towerType,
            baseDamage
        );

        return damageWithSkills + bonusDamage;
    }
    float GetBaseDamage()
    {
        switch (attackType)
        {
            case AttackType.Projectile:

                if (bulletPrefab != null)
                {
                    Bullet bullet = bulletPrefab.GetComponent<Bullet>();

                    if (bullet != null)
                        return bullet.damage;
                }

                break;

            case AttackType.Earthquake:

                if (earthquakeAttack != null)
                    return earthquakeAttack.damage;

                break;

            case AttackType.Laser:

                if (bulletPrefab != null)
                {
                    Bullet bullet = bulletPrefab.GetComponent<Bullet>();

                    if (bullet != null)
                        return bullet.damage;
                }

                break;
        }

        return 0f;
    }
    public float GetBonusDamageValue()
    {
        return bonusDamage;
    }
    public int GetDamageUpgradeCost()
    {
        return Mathf.RoundToInt(
            damageBaseCost *
            Mathf.Pow(costMultiplier, damageUpgradeLevel)
        );
    }
    public void UpgradeDamage(float amount)
    {
        if (upgradePoints <= 0)
            return;

        int cost = GetDamageUpgradeCost();

        if (!PlayerResources.instance.CanAfford(0, cost))
            return;

        PlayerResources.instance.Spend(0, cost);

        bonusDamage += amount;

        upgradePoints--;
        damageUpgradeLevel++;
    }
    public int GetRangeUpgradeCost()
    {
        return Mathf.RoundToInt(
            5f * Mathf.Pow(1.5f, rangeUpgradeLevel)
        );
    }

    public void UpgradeRange(float amount)
    {
        if (upgradePoints <= 0)
            return;

        int cost = GetRangeUpgradeCost();

        if (!PlayerResources.instance.CanAfford(0, cost))
            return;

        PlayerResources.instance.Spend(0, cost);

        bonusRange += amount;

        upgradePoints--;
        rangeUpgradeLevel++;
    }
    public int GetFireRateUpgradeCost()
    {
        return Mathf.RoundToInt(
            5f * Mathf.Pow(1.5f, fireRateUpgradeLevel)
        );
    }

    public void UpgradeFireRate(float amount)
    {
        if (upgradePoints <= 0)
            return;

        int cost = GetFireRateUpgradeCost();

        if (!PlayerResources.instance.CanAfford(0, cost))
            return;

        PlayerResources.instance.Spend(0, cost);

        bonusFireRate += amount;

        upgradePoints--;
        fireRateUpgradeLevel++;
    }
    public Tower TryEvolve()
    {
        if (level < 2)
            return null;

        if (data.nextUpgrade == null)
            return null;

        int cost = 20;

        if (!PlayerResources.instance.CanAfford(0, cost))
            return null;

        PlayerResources.instance.Spend(0, cost);

        BuildNode node = GetComponentInParent<BuildNode>();

        GameObject newTowerGO = Instantiate(
            data.nextUpgrade.prefab,
            transform.position,
            transform.rotation
        );

        newTowerGO.transform.SetParent(node.transform);

        Tower newTower = newTowerGO.GetComponent<Tower>();

        node.currentTower = newTower;

        Destroy(gameObject);

        return newTower;
    }

    public void Transmute(TowerData option)
    {
        if (level < 10) return;
        if (option == null) return;

        BuildNode node = GetComponentInParent<BuildNode>();

        GameObject newTowerGO = Instantiate(
            option.prefab,
            transform.position,
            transform.rotation
        );

        newTowerGO.transform.SetParent(node.transform);

        node.currentTower = newTowerGO.GetComponent<Tower>();

        Destroy(gameObject);
    }

    public void OnSelected()
    {
        TowerUIManager.instance.SelectTower(this);
    }

    void RotateTowardsTarget()
    {
        if (head == null)
            return;

        if (target == null)
            return;

        Vector3 direction =
            target.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
    Quaternion.LookRotation(direction) *
    Quaternion.Euler(0, rotationOffset, 0);

        head.rotation = Quaternion.RotateTowards(
            head.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void ShowRange()
    {
        if (rangeIndicatorInstance == null) return;

        rangeIndicatorInstance.SetActive(true);

        Vector3 pos = transform.position;
        pos.y = 0.05f;

        rangeIndicatorInstance.transform.position = pos;

        float diameter = GetRange() * 2f;
        float scaleFactor = diameter / baseSize;

        rangeIndicatorInstance.transform.localScale = new Vector3(
            scaleFactor,
            1,
            scaleFactor
        );
    }

    public void HideRange()
    {
        if (rangeIndicatorInstance == null) return;

        rangeIndicatorInstance.SetActive(false);
    }

    Vector3 GetPredictedPosition(Transform target)
    {
        EnemyMovement em = target.GetComponent<EnemyMovement>();

        if (em == null)
            return target.position;

        Vector3 velocity = em.GetVelocity();

        float distance = Vector3.Distance(firePoint.position, target.position);
        float timeToHit = distance / GetProjectileSpeed();

        timeToHit = Mathf.Clamp(timeToHit, 0f, 4f);

        return target.position + velocity * timeToHit;
    }

    float GetProjectileSpeed()
    {
        Bullet b = bulletPrefab.GetComponent<Bullet>();
        return b != null ? b.speed : 20f;
    }

    public void AnimationShoot()
    {
        if (!waitingAnimationShot)
            return;

        waitingAnimationShot = false;

        ExecuteAttack();
    }
    public void AnimationChargeFinished()
    {
        if (!waitingAnimationShot)
            return;

        if (animator != null && useAttackAnimation)
        {
            animator.SetTrigger(attackTrigger);
        }
        else
        {
            AnimationShoot();
        }
    }
    void UpdateAnimationStates()
    {
        if (animator == null)
            return;

        bool hasTarget = target != null;

        if (hasTarget == hasTargetState)
            return;

        hasTargetState = hasTarget;

        animator.SetBool(rotatingBool, hasTarget);
    }
}