using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    [Header("Resources")]
    public GameObject fundsOutline;
    public GameObject purityOutline;
    public GameObject biodiversityOutline;
    [Space]
    public GameObject fundsFill;
    public GameObject purityFill;
    public GameObject biodiversityFill;
    [Space]
    public GameObject fundsIcon;
    public GameObject purityIcon;
    public GameObject biodiversityIcon;

    [Header("Monitor")]
    public GameObject openMonitorButton;
    public GameObject closeMonitorButton;

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
}
