using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

// Manager that handles fade-in/out
public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;
    public float fadeDuration = 2f;

    private CanvasGroup canvasGroup;
    [SerializeField] private GameObject fadePanelObject;
    [SerializeField] private bool isFading;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else Destroy(gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += EndLoadScene;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= EndLoadScene;
    }

    public void LoadScene(string sceneName)
    {
        if (isFading) return;
        StartCoroutine(DoFadeIn(() => SceneManager.LoadScene(sceneName)));
    }

    private void EndLoadScene(Scene scene, LoadSceneMode mode)
    {
        FindFadePanel();
        if (fadePanelObject != null)
        {
            fadePanelObject.SetActive(true);
            // canvasGroup.alpha = 1f;
            StartCoroutine(DoFadeOut(null));
        }

        else isFading = false;
    }

    private void FindFadePanel()
    {
        GameObject canvasObject = GameObject.Find("Canvas");

        if (canvasObject != null)
        {
            Transform fadePanelTransform = canvasObject.transform.Find("FadePanel");
            if (fadePanelTransform != null)
            {
                fadePanelObject = fadePanelTransform.gameObject;
                canvasGroup = fadePanelTransform.GetComponent<CanvasGroup>();
            }
        }
    }

    public void FadeIn(Action onComplete)
    {
        if (isFading) return;
        StartCoroutine(DoFadeIn(onComplete));
    }

    public void FadeOut(Action onComplete)
    {
        // if (isFading) return;
        StartCoroutine(DoFadeOut(onComplete));
    }

    private IEnumerator DoFadeIn(Action onComplete)
    {
        isFading = true;
        FindFadePanel();

        if (fadePanelObject != null)
        {
            canvasGroup.alpha = 0f;
            fadePanelObject.SetActive(true);
            yield return StartCoroutine(Fade(1, fadeDuration));
        }

        onComplete?.Invoke();
    }

    private IEnumerator DoFadeOut(Action onComplete)
    {
        if (fadePanelObject != null)
        {
            yield return StartCoroutine(Fade(0, fadeDuration));
        }

        onComplete?.Invoke();
        isFading = false;
    }

    public void ChangeDay(Action onMidFade)
    {
        if (isFading) return;
        StartCoroutine(ChangeDayFade(onMidFade));
    }

    private IEnumerator ChangeDayFade(Action onMidFade)
    {
        isFading = true;
        FindFadePanel();

        if (fadePanelObject != null)
        {
            fadePanelObject.SetActive(true);

            // 1. fade in
            yield return StartCoroutine(Fade(1, fadeDuration));

            // 2. start next dat
            onMidFade?.Invoke();

            // 3. fade out
            yield return StartCoroutine(Fade(0, fadeDuration));
        }
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        if (canvasGroup == null) yield break;

        float startAlpha = canvasGroup.alpha;
        float timer = 0f;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (targetAlpha == 0)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            if (fadePanelObject != null) fadePanelObject.SetActive(false);
        }

        // isFading = false;
    }
}