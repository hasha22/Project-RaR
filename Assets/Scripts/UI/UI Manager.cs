using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    [Header("Resources")]
    /*
    public GameObject fundsFill;
    public GameObject purityFill;
    public GameObject biodiversityFill;
    */
    public TextMeshProUGUI funds;
    public TextMeshProUGUI purity;
    public TextMeshProUGUI biodiversity;

    [Header("Reef Secretary")]
    public GameObject reefSecretary1;

    [Header("Monitor")]
    public GameObject monitorUI;
    public GameObject openMonitorButton;
    private bool isMonitorOpened = false;

    [Header("Decisions")]
    public GameObject expandedDecisionList;
    private bool isDecisionListOpened = false;
    [Space]
    public GameObject dropdownButton;
    public Sprite dropdownClosed;
    public Sprite dropdownOpened;
    [Space]
    public GameObject decisionDialogueBox;
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI decisionContext;
    //public float typingSpeed = 0.03f;

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
        decisionDialogueBox.SetActive(false);
        reefSecretary1.SetActive(false);
        expandedDecisionList.SetActive(false);
        monitorUI.SetActive(false);
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
        if (InputManager.instance.hasPressedF)
        {
            InputManager.instance.hasPressedF = false;

            if (isDecisionListOpened)
            {
                Sprite imageSprite = dropdownButton.GetComponent<Image>().sprite;
                imageSprite = dropdownClosed;
                expandedDecisionList.SetActive(false);
            }
            else
            {
                Sprite imageSprite = dropdownButton.GetComponent<Image>().sprite;
                imageSprite = dropdownOpened;
                expandedDecisionList.SetActive(true);
            }

            isDecisionListOpened = !isDecisionListOpened;
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
    public void EnableExpandedList()
    {
        if (!isDecisionListOpened)
        {
            Sprite imageSprite = dropdownButton.GetComponent<Image>().sprite;
            imageSprite = dropdownOpened;
            isDecisionListOpened = true;
            expandedDecisionList.SetActive(true);
        }
        else
        {
            Sprite imageSprite = dropdownButton.GetComponent<Image>().sprite;
            imageSprite = dropdownClosed;
            isDecisionListOpened = false;
            expandedDecisionList.SetActive(false);
        }
    }
    public void BeginDecisionDialogue(Decision decision)
    {
        decisionContext.text = decision.decisionText;

        switch (decision.reefType)
        {
            case ReefType.Reef1:
                speakerName.text = "Angela"; //names are placeholders
                break;
            case ReefType.Reef2:
                speakerName.text = "Dutch";
                break;
            case ReefType.Reef3:
                speakerName.text = "Micah";
                break;
            case ReefType.Reef4:
                speakerName.text = "Your mom";
                break;
        }

        reefSecretary1.SetActive(true);
        decisionDialogueBox.SetActive(true);
    }
    public void EndDecisionDialogue()
    {
        reefSecretary1.SetActive(false);
        decisionDialogueBox.SetActive(false);
    }
    public void UpdateFundsUI(int newFunds)
    {
        funds.text = newFunds.ToString();
    }
    public void UpdatePurityUI(int newPurity)
    {
        funds.text = newPurity.ToString();
    }
    public void UpdateBiodiversityUI(int newBiodiversity)
    {
        funds.text = newBiodiversity.ToString();
    }

}
