using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    [Header("Resources")]
    public GameObject fundsFill;
    public GameObject purityFill;
    public GameObject biodiversityFill;

    [Header("Monitor")]
    public GameObject monitorUI;
    public GameObject openMonitorButton;
    private bool isMonitorOpened = false;

    [Header("Dialogue")]
    public GameObject dialogueBox;
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
    private void Update()
    {
        if (InputManager.instance.hasPressedSpace)
        {
            InputManager.instance.hasPressedSpace = false;
            if (isMonitorOpened) monitorUI.SetActive(false);
            else monitorUI.SetActive(true);

            isMonitorOpened = !isMonitorOpened;
        }
    }
    public void EnableMonitorUI()
    {
        if (!isMonitorOpened)
        {
            isMonitorOpened = true;
            monitorUI.SetActive(true);
        }
        else
        {
            isMonitorOpened = false;
            monitorUI.SetActive(false);
        }
    }
}
