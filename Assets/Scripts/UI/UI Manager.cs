using System.Collections.Generic;
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
    [SerializeField] private TextMeshProUGUI funds;
    [SerializeField] private TextMeshProUGUI purity;
    [SerializeField] private TextMeshProUGUI biodiversity;

    [Header("Reef Visuals")]
    [SerializeField] private Image reefImage; 
    [SerializeField] private GameObject reefSecretary1;

    [Header("Monitor")]
    [SerializeField] private GameObject monitorUI;
    [SerializeField] private GameObject openMonitorButton;
    private bool isMonitorOpened = false;

    [Header("Decisions")]
    private bool isDecisionListOpened = false;
    //list variables
    [SerializeField] private GameObject expandedDecisionList;
    [SerializeField] private GameObject dropdownButton;
    [SerializeField] private GameObject decisionPrefab;
    // [SerializeField] private Transform decisionsContainer;
    public Transform decisionsContainer;
    [SerializeField] private Sprite dropdownClosed;
    [SerializeField] private Sprite dropdownOpened;
    [SerializeField] private TextMeshProUGUI currentDecisionsTaken;
    [SerializeField] private TextMeshProUGUI maxDecisions;
    // private List<GameObject> decisionList = new List<GameObject>();
    public List<GameObject> decisionList = new List<GameObject>();
    [Space]
    //dialogue box variables
    [SerializeField] private GameObject decisionDialogueBox;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private TextMeshProUGUI decisionContext;
    private TypingEffect typingEffect;
    //public float typingSpeed = 0.03f;

    [Header("Dialogue")]
    [SerializeField] private GameObject dialogueBox;
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

        //temporary. the hard cap for each reef should be set at the beginning of the day

        decisionDialogueBox.SetActive(false);
        reefSecretary1.SetActive(false);
        expandedDecisionList.SetActive(false);
        monitorUI.SetActive(false);
        typingEffect = decisionContext.GetComponent<TypingEffect>();
    }

    private void Init() 
    { 
        if (ReefManager.Instance != null)
        {
            ReefManager.Instance.OnReefSwitched += UpdateReefUI;
            Debug.Log($"{name}: Subscribing to ReefManager events");
        }
    }

    private void Start()
    {
        Init();

        //moved to start due to loading order errors, will be changed later in project settings
        maxDecisions.text = DecisionManager.instance.firstReefHardCap.ToString();
    }

    private void UpdateReefUI(ReefType newReef)
    {
        ReefData data = ReefManager.Instance.activeReefData;
        
        // 인게임 배경 이미지 갱신
        if (reefImage != null && data.backgroundImage != null)
        {
            reefImage.sprite = data.backgroundImage;
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
        Image img = dropdownButton.GetComponent<Image>();

        if (!isDecisionListOpened)
        {
            img.sprite = dropdownOpened;
            isDecisionListOpened = true;
            expandedDecisionList.SetActive(true);
        }
        else
        {
            img.sprite = dropdownClosed;
            isDecisionListOpened = false;
            expandedDecisionList.SetActive(false);
        }
    }
    public void BeginDecisionDialogue(Decision decision)
    {
        if (DialogueManager.Instance.isDialogueBoxOpened) return;

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
        typingEffect.StartTyping(decision.decisionText);

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
        purity.text = newPurity.ToString();
    }
    public void UpdateBiodiversityUI(int newBiodiversity)
    {
        biodiversity.text = newBiodiversity.ToString();
    }
    public void UpdateDecisionsTaken(int newDecisions)
    {
        currentDecisionsTaken.text = newDecisions.ToString();
    }
    public void InstantiateDecisions(List<Decision> decisions)
    {
        for (int i = 0; i < decisions.Count; i++)
        {
            GameObject decision = Instantiate(decisionPrefab, decisionsContainer);
            decisionList.Add(decision);

            TextMeshProUGUI title = decision.GetComponentInChildren<TextMeshProUGUI>();
            title.text = decisions[i].decisionTitle;

            //this logic should be separate, works for now
            DecisionUI decisionScript = decision.GetComponent<DecisionUI>();
            decisionScript.decision = decisions[i];
        }
    }
    public void RemoveDecision(Decision decision)
    {
        for (int i = 0; i < decisionList.Count; i++)
        {
            DecisionUI script = decisionList[i].GetComponent<DecisionUI>();
            if (script.decision.decisionTitle == decision.decisionTitle)
            {
                GameObject obj = decisionList[i];

                decisionList.RemoveAt(i);
                Destroy(obj);
                break;
            }
        }
    }

}
