using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerUIManager : MonoBehaviour
{
    public static TowerUIManager instance;

    public GameObject panel;
    
    [Header("Icon")]
    public Image towerIcon;

    [Header("Texts")]
    public TextMeshProUGUI towerNameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI pointsText;

    public TextMeshProUGUI damageText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI fireRateText;

    [Header("Buttons")]
    public Button damageButton;
    public Button rangeButton;
    public Button fireRateButton;
    public Button evolveButton;

    [Header("Transmute")]
    public GameObject transmutePanel;
    public Button transmuteA;
    public Button transmuteB;

    [Header("Upgrade Values")]
    public float damageUpgradeAmount = 5f;
    public float rangeUpgradeAmount = 1f;
    public float fireRateUpgradeAmount = 0.2f;

    private Tower currentTower;


    void Awake()
    {
        Debug.Log("TowerUIManager Awake");

        instance = this;

        panel.SetActive(false);
        transmutePanel.SetActive(false);
    }

    public void SelectTower(Tower tower)
    {
        currentTower = tower;

        panel.SetActive(true);
    }

    void Update()
    {
        if (currentTower == null) return;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (currentTower == null) return;

        levelText.text = "Lv " + currentTower.level;
        pointsText.text = $"Pontos: {currentTower.upgradePoints}";

        bool canUpgrade = currentTower.upgradePoints > 0;

        damageButton.interactable = canUpgrade;
        rangeButton.interactable = canUpgrade;
        fireRateButton.interactable = canUpgrade;

        evolveButton.interactable = currentTower.level >= 2;

        if (currentTower.level >= 10)
            transmutePanel.SetActive(true);

        float currentDamage = currentTower.GetFinalDamage();
        float bonusDamage = damageUpgradeAmount;

        damageText.text = $"{currentDamage:0} <color=green>(+{bonusDamage:0})</color>";

        float currentRange = currentTower.GetRange();
        float bonusRange = rangeUpgradeAmount;

        rangeText.text = $"{currentRange:0.0} <color=green>(+{bonusRange:0.0})</color>";

        float currentFR = currentTower.GetFireRate();
        float bonusFR = fireRateUpgradeAmount;

        fireRateText.text = $"{currentFR:0.00} <color=green>(+{bonusFR:0.00})</color>";

        if (currentTower.data != null)
        {
            towerNameText.text = currentTower.data.towerName;

            if (currentTower.data.icon != null)
                towerIcon.sprite = currentTower.data.icon;
        }
    }

    public void UpgradeDamage()
    {
        currentTower.UpgradeDamage(damageUpgradeAmount);
    }

    public void UpgradeRange()
    {
        currentTower.UpgradeRange(rangeUpgradeAmount);
    }

    public void UpgradeFireRate()
    {
        currentTower.UpgradeFireRate(fireRateUpgradeAmount);
    }

    public void Evolve()
    {
        currentTower.TryEvolve();
        Close();
    }

    public void OpenTransmute()
    {
        TransmuteUI.instance.Open(currentTower);
    }
    public void SellTower()
    {
        if (currentTower == null)
            return;

        BuildNode node =
            currentTower.GetComponentInParent<BuildNode>();

        if (currentTower.data != null)
        {
            PlayerResources.instance.AddMoney(
                currentTower.data.sellValue
            );
        }

        if (node != null)
        {
            node.currentTower = null;
            node.isOccupied = false;
        }

        Destroy(currentTower.gameObject);

        Close();
    }

    public void Close()
    {
        if (currentTower != null)
            currentTower.HideRange();

        panel.SetActive(false);
        currentTower = null;
    }
}