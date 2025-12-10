using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public static TitleScreenManager instance { get; private set; }

    [Header("Scene Index")]
    [SerializeField] private int introSceneIndex = 1;
    [SerializeField] private int gameSceneIndex = 2;

    [Header("Confirmation Message")]
    [SerializeField] private GameObject confirmationBox;

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
        confirmationBox.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void EnableConfirmation()
    {
        confirmationBox.SetActive(true);
    }
    public void StartNewGame()
    {
        StartCoroutine(LoadScene(gameSceneIndex));
    }
    public void StartGameIntro()
    {
        StartCoroutine(LoadScene(introSceneIndex));
    }
    public IEnumerator LoadScene(int index)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(index);
        yield return null;
    }
}
