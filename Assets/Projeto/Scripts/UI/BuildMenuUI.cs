using UnityEngine;

public class BuildMenuUI : MonoBehaviour
{
    public static BuildMenuUI instance;

    public GameObject menuPanel;

    BuildNode currentNode;

    public bool IsMenuOpen => menuPanel.activeSelf;

    void Awake()
    {
        instance = this;
        menuPanel.SetActive(false);
    }

    public void OpenMenu(BuildNode node)
    {
        currentNode = node;
        menuPanel.SetActive(true);
    }

    public void CloseMenu()
    {
        menuPanel.SetActive(false);
        currentNode = null;
    }

    public BuildNode GetNode()
    {
        return currentNode;
    }
}