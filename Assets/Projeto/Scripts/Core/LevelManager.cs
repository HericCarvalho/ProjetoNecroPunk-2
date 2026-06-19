using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public WinUI winUI;
    public GameObject winPanel;
    public GameObject gameOverPanel;
    public GameObject reviveHUD;

    bool isReviveOpen = false;

    public CanvasGroup winGroup;
    public CanvasGroup gameOverGroup;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        LevelStatsManager.instance.StartLevel();
    }
    public void GoToMenu()
    {
        Time.timeScale = 1f;

        CanvasGroup hud = null;

        if (winPanel.activeSelf)
            hud = winGroup;
        else if (gameOverPanel.activeSelf)
            hud = gameOverGroup;

        SceneLoader.instance.LoadScene("LevelSelection", hud);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        CanvasGroup hud = null;

        if (winPanel.activeSelf)
            hud = winGroup;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneLoader.instance.LoadSceneByIndex(nextIndex, hud);
        }
        else
        {
            SceneLoader.instance.LoadScene("Menu", hud);
        }
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void WinGame()
    {
        if (LevelStatsManager.instance != null)
        {
            LevelStatsManager.instance.EndLevel();
        }

        int stars = LevelStatsManager.instance.GetStars();

        GameManager.instance.UnlockNextLevel();

        winUI.ShowStats(stars);

        Time.timeScale = 0f;
        winPanel.SetActive(true);
    }
    public void GoToMenuPrincipal()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
    public void GameOver()
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }

    public void ToggleReviveHUD()
    {
        isReviveOpen = !isReviveOpen;
        reviveHUD.SetActive(isReviveOpen);
    }

    public void OpenReviveHUD()
    {
        isReviveOpen = true;
        reviveHUD.SetActive(true);
    }

    public void CloseReviveHUD()
    {
        isReviveOpen = false;
        reviveHUD.SetActive(false);
    }

    public bool IsReviveOpen()
    {
        return isReviveOpen;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}