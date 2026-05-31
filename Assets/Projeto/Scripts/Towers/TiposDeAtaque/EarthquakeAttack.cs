using AudioSystem;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class EarthquakeAttack : MonoBehaviour
{
    [Header("Stats")]
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

    [Header("Visual")]
    public GameObject effectPrefab;

    [SerializeField] SoundData soundData;

    public void Execute(Vector3 position,float finalDamage,float range,Tower owner)
    {
        if (soundData != null)
        {
            SoundManager.Instance.CreateSoundBuilder()
                .WithPosition(position) 
                .WithRandomPitch()      
                .Play(soundData);
        }

        if (EnemyManager.instance == null) return;

        Transform[] snapshot = EnemyManager.instance.enemies.ToArray();

        foreach (Transform enemyTransform in snapshot)
        {
            if (enemyTransform == null || !enemyTransform.gameObject.activeInHierarchy) continue;

            float distance = Vector3.Distance(position, enemyTransform.position);
            if (distance > range) continue;

            EnemyHealth enemy = enemyTransform.GetComponent<EnemyHealth>();
            if (enemy == null) continue;

            float damageToDeal = finalDamage;

            enemy.TakeDamage(damageToDeal,isMagicDamage,isTrueDamage);
            LevelStatsManager.instance.RegisterDamage(owner,damageToDeal);

            if (owner != null)
                owner.GainXP(1);

            if (Random.value <= burnChance) enemy.ApplyBurn(burnDuration, burnDPS);
            if (Random.value <= slowChance) enemy.ApplySlow(slowDuration, slowMultiplier);
            if (Random.value <= stunChance) enemy.ApplyStun(stunDuration);
        }
    }
}
