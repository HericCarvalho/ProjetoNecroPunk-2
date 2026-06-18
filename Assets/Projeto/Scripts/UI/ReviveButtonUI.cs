using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReviveButtonUI : MonoBehaviour
{
    [Header("Unit Info")]
    public string enemyID;
    public GameObject prefab;

    [Header("Cost")]
    public float manaCost = 20f;
    public int fragmentsRequired = 5;

    [Header("UI")]
    public Button button;
    public TextMeshProUGUI fragmentText;
    public TextMeshProUGUI manaText;

    [Header("Visual Feedback")]
    public Image buttonImage;
    public Color availableColor = Color.white;
    public Color unavailableColor = Color.gray;

    private bool wasAvailableLastFrame = false;

    void Update()
    {
        if (PlayerMana.instance == null || FragmentManager.instance == null)
            return;

        int currentFragments = FragmentManager.instance.GetFragments(enemyID);
        float currentMana = PlayerMana.instance.currentMana;

        bool canRevive = currentFragments >= fragmentsRequired;
        bool hasMana = currentMana >= manaCost;

        bool isAvailable = canRevive && hasMana;

        if (fragmentText != null)
            fragmentText.text = currentFragments + " / " + fragmentsRequired;

        if (manaText != null)
            manaText.text = (int)manaCost + " Mana";

        if (button != null)
            button.interactable = isAvailable;

        if (buttonImage != null)
            buttonImage.color = isAvailable ? availableColor : unavailableColor;

        if (isAvailable && !wasAvailableLastFrame)
        {
            AnimateButton();
        }

        wasAvailableLastFrame = isAvailable;
    }

    public void OnClick()
    {
        Debug.Log("Botão clicado");

        if (button != null && !button.interactable)
            return;

        if (!FragmentManager.instance.CanRevive(enemyID))
        {
            Debug.Log("Fragmentos insuficientes");
            return;
        }

        if (!PlayerMana.instance.HasMana(manaCost))
        {
            Debug.Log("Mana insuficiente");
            return;
        }

        Debug.Log("Entrando em modo de posicionamento");

        PlayerMana.instance.SpendMana(manaCost);
        FragmentManager.instance.ConsumeFragments(enemyID,fragmentsRequired
);

        PlacementManager.instance.StartPlacing(prefab);
    }

    void AnimateButton()
    {
        StopAllCoroutines();
        StartCoroutine(Pulse());
    }

    System.Collections.IEnumerator Pulse()
    {
        float time = 0f;
        Vector3 originalScale = transform.localScale;

        while (time < 0.3f)
        {
            time += Time.deltaTime;
            float scale = 1f + Mathf.Sin(time * 20f) * 0.1f;
            transform.localScale = originalScale * scale;
            yield return null;
        }

        transform.localScale = originalScale;
    }
}