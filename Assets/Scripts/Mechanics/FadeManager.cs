using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// Manager that handles fade-in/out during scene transitions
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
        StartCoroutine(StartLoadScene(sceneName));
    }

    private IEnumerator StartLoadScene(string sceneName)
    {
        isFading = true;
        FindFadePanel();
        if (fadePanelObject != null)
        {
            fadePanelObject.SetActive(true);
            canvasGroup.alpha = 0f;
            yield return StartCoroutine(Fade(1, fadeDuration));
        }

        SceneManager.LoadScene(sceneName);
    }

    private void EndLoadScene(Scene scene, LoadSceneMode mode)
    {
        FindFadePanel();
        if (fadePanelObject != null)
        {
            fadePanelObject.SetActive(true);
            canvasGroup.alpha = 1f;
            StartCoroutine(Fade(0, fadeDuration));
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

        isFading = false;
    }
}