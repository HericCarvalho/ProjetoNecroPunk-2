using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    public GameObject loadingPrefab;

    Image fadeImage;
    Image progressBar;

    TextMeshProUGUI loadingText;
    TextMeshProUGUI levelNameText;
    TextMeshProUGUI descriptionText;
    TextMeshProUGUI continueText;

    CanvasGroup contentGroup;
    CanvasGroup introGroup;

    public float fadeDuration = 0.5f;
    public float titleFadeInDuration = 0.6f;
    public float titleFadeOutDuration = 0.8f;
    public float titleHoldDuration = 0.5f;

    Typewriter typewriter;

    Coroutine blinkRoutine;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName, CanvasGroup hudGroup = null)
    {
        StartCoroutine(LoadRoutine(sceneName, hudGroup));
    }

    IEnumerator LoadRoutine(string sceneName, CanvasGroup hudGroup)
    {
        GameObject loading = Instantiate(loadingPrefab);
        DontDestroyOnLoad(loading);

        Canvas canvas = loading.GetComponentInChildren<Canvas>();
        canvas.sortingOrder = 9999;

        fadeImage = canvas.transform.Find("FadeImage").GetComponent<Image>();
        progressBar = canvas.transform.Find("Content/ProgressBar/Fill").GetComponent<Image>();
        loadingText = canvas.transform.Find("Content/LoadingText").GetComponent<TextMeshProUGUI>();

        contentGroup = canvas.transform.Find("Content").GetComponent<CanvasGroup>();
        introGroup = canvas.transform.Find("Intro").GetComponent<CanvasGroup>();

        levelNameText = canvas.transform.Find("Intro/LevelNameText").GetComponent<TextMeshProUGUI>();
        descriptionText = canvas.transform.Find("Intro/DescriptionText").GetComponent<TextMeshProUGUI>();
        continueText = canvas.transform.Find("Intro/ContinueText").GetComponent<TextMeshProUGUI>();

        typewriter = loading.GetComponent<Typewriter>();

        CanvasGroup titleCG = levelNameText.GetComponent<CanvasGroup>();
        CanvasGroup descCG = descriptionText.GetComponent<CanvasGroup>();
        CanvasGroup contCG = continueText.GetComponent<CanvasGroup>();

        Color c = fadeImage.color;
        c.a = 0;
        fadeImage.color = c;

        contentGroup.alpha = 0;
        introGroup.alpha = 0;

        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.SmoothStep(0, 1, t / fadeDuration);

            if (hudGroup != null)
                hudGroup.alpha = 1 - alpha;

            c.a = alpha;
            fadeImage.color = c;

            yield return null;
        }

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        contentGroup.alpha = 1;

        while (op.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);

            progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, progress, 10f * Time.unscaledDeltaTime);
            loadingText.text = "Carregando... " + Mathf.RoundToInt(progress * 100f) + "%";

            yield return null;
        }

        progressBar.fillAmount = 1f;
        loadingText.text = "Carregando... 100%";

        yield return new WaitForSecondsRealtime(0.2f);

        op.allowSceneActivation = true;

        while (!op.isDone)
            yield return null;

        contentGroup.alpha = 0;
        introGroup.alpha = 1;

        LevelData data = null;

        if (Levelbase.instance != null)
            data = Levelbase.instance.GetLevel(sceneName);

        string displayName = data != null ? data.displayName : sceneName;
        string desc = data != null ? data.description : "";

        levelNameText.text = displayName;

        titleCG.alpha = 0;
        descCG.alpha = 0;
        contCG.alpha = 0;

        yield return StartCoroutine(FadeCanvas(titleCG, 0, 1, titleFadeInDuration));
        yield return StartCoroutine(FadeCanvas(descCG, 0, 1, 0.3f));

        if (typewriter != null)
            yield return StartCoroutine(typewriter.Write(descriptionText, desc));
        else
            descriptionText.text = desc;

        yield return StartCoroutine(FadeCanvas(contCG, 0, 1, 0.4f));

        blinkRoutine = StartCoroutine(BlinkContinue(contCG, continueText.transform));

        while (true)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame ||
                Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
                break;

            yield return null;
        }

        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        StartCoroutine(FadeCanvas(descCG, 1, 0, 0.3f));
        StartCoroutine(FadeCanvas(contCG, 1, 0, 0.3f));

        yield return StartCoroutine(FadeImage(1, 0, fadeDuration));

        yield return new WaitForSecondsRealtime(titleHoldDuration);

        yield return StartCoroutine(FadeCanvas(titleCG, 1, 0, titleFadeOutDuration));

        Destroy(loading);
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float duration)
    {
        float t = 0;
        cg.alpha = from;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float smooth = Mathf.SmoothStep(0, 1, t / duration);
            cg.alpha = Mathf.Lerp(from, to, smooth);
            yield return null;
        }

        cg.alpha = to;
    }

    IEnumerator FadeImage(float from, float to, float duration)
    {
        float t = 0;
        Color c = fadeImage.color;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;
    }

    IEnumerator BlinkContinue(CanvasGroup cg, Transform target)
    {
        float speed = 1.5f;

        while (true)
        {
            float t = 0;

            while (t < 1)
            {
                t += Time.unscaledDeltaTime * speed;

                cg.alpha = Mathf.SmoothStep(0, 1, t);
                target.localScale = Vector3.one * (1f + Mathf.Sin(Time.unscaledTime * 2f) * 0.05f);

                yield return null;
            }

            t = 0;

            while (t < 1)
            {
                t += Time.unscaledDeltaTime * speed;

                cg.alpha = Mathf.SmoothStep(1, 0, t);
                target.localScale = Vector3.one * (1f + Mathf.Sin(Time.unscaledTime * 2f) * 0.05f);

                yield return null;
            }
        }
    }
    public void LoadSceneByIndex(int index, CanvasGroup hudGroup = null)
    {
        string sceneName = SceneManager.GetSceneByBuildIndex(index).name;
        LoadScene(sceneName, hudGroup);
    }
}