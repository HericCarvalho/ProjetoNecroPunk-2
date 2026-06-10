using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image fillImage;

    private Transform cam;
    private EnemyHealth health;

    public Vector3 offset = new Vector3(0, 2f, 0);

    void Start()
    {
        cam = Camera.main.transform;
        health = GetComponentInParent<EnemyHealth>();
    }

    void LateUpdate()
    {
        if (health == null) return;

        transform.localPosition = offset;

        transform.LookAt(cam);
        transform.Rotate(0, 0, 0);

        float value = health.CurrentHealth / health.maxHealth;
        fillImage.fillAmount = value;

        fillImage.enabled = value < 0.99f;
    }
}