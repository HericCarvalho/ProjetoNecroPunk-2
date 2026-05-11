using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialPagesUI : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup panelGroup;
    public Image displayImage;

    public Button nextButton;
    public Button backButton;
    public Button exitButton;

    [Header("Conteúdo")]
    public Sprite[] pages;

    int currentPage = 0;
    bool isTransitioning = false;

    void Start()
    {
        nextButton.onClick.AddListener(Next);
        backButton.onClick.AddListener(Back);
        //exitButton.onClick.AddListener(Close);

        UpdateUI();
    }

    public void Open()
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeCanvas(0, 1, 0.3f));

        panelGroup.interactable = true;
        panelGroup.blocksRaycasts = true;
    }

    //public void Close()
    //{
    //    StartCoroutine(CloseRoutine());
    //}

    IEnumerator CloseRoutine()
    {
        yield return FadeCanvas(1, 0, 0.3f);

        panelGroup.interactable = false;
        panelGroup.blocksRaycasts = false;

        gameObject.SetActive(false);
    }

    public void Next()
    {
        if (isTransitioning) return;
        if (currentPage >= pages.Length - 1) return;

        currentPage++;
        StartCoroutine(SwitchPage());
    }

    public void Back()
    {
        if (isTransitioning) return;
        if (currentPage <= 0) return;

        currentPage--;
        StartCoroutine(SwitchPage());
    }

    IEnumerator SwitchPage()
    {
        isTransitioning = true;

        RectTransform rt = displayImage.rectTransform;

        Vector3 startPos = rt.localPosition;
        Vector3 left = startPos + Vector3.left * 200f;

        float t = 0;

        while (t < 0.2f)
        {
            t += Time.unscaledDeltaTime;
            rt.localPosition = Vector3.Lerp(startPos, left, t / 0.2f);
            yield return null;
        }

        rt.localPosition = startPos + Vector3.right * 200f;
        displayImage.sprite = pages[currentPage];

        t = 0;

        while (t < 0.25f)
        {
            t += Time.unscaledDeltaTime;
            rt.localPosition = Vector3.Lerp(rt.localPosition, startPos, t / 0.25f);
            yield return null;
        }

        rt.localPosition = startPos;

        UpdateUI();

        isTransitioning = false;
    }

    void UpdateUI()
    {
        displayImage.sprite = pages[currentPage];

        backButton.gameObject.SetActive(currentPage > 0);
        nextButton.gameObject.SetActive(currentPage < pages.Length - 1);
    }

    IEnumerator FadeImage(float from, float to, float duration)
    {
        float t = 0;
        Color c = displayImage.color;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            c.a = a;
            displayImage.color = c;
            yield return null;
        }

        c.a = to;
        displayImage.color = c;
    }

    IEnumerator FadeCanvas(float from, float to, float duration)
    {
        float t = 0;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            panelGroup.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        panelGroup.alpha = to;
    }
}