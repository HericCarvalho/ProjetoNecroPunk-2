using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour
{
    public Tower towerXP;

    public Slider xpBar;

    public TextMeshProUGUI levelText;

    void Update()
    {
        if (towerXP == null)
            return;

        UpdateUI();
    }

    void UpdateUI()
    {
        float progress = (float)towerXP.currentXP / towerXP.xpToNextLevel;

        xpBar.value = progress;

        levelText.text = "Lv " + towerXP.level;
    }
}