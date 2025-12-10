using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour
{
    [Header("Game Scene Index")]
    [SerializeField] private int gameSceneIndex = 2;

    public void StartNewGame()
    {
        StartCoroutine(LoadScene(gameSceneIndex));
    }
    public IEnumerator LoadScene(int index)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(index);
        yield return null;
    }
}
