using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public static TitleScreenManager instance { get; private set; }

    [Header("Scene Index")]
    [SerializeField] private int gameSceneIndex = 1;
    [SerializeField] private int endGameSceneIndex = 2;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        AudioManager.instance.PlayBGM(AudioManager.instance.menuBGM);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void StartNewGame()
    {
        StartCoroutine(LoadScene(gameSceneIndex));
    }
    public void EndGame()
    {
        StartCoroutine(LoadScene(endGameSceneIndex));
    }
    public IEnumerator LoadScene(int index)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(index);
        yield return null;
    }
}
