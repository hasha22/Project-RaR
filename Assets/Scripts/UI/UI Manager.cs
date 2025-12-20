using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    [Header("Resources")]
    [SerializeField] private TextMeshProUGUI funds;
    [SerializeField] private TextMeshProUGUI purity;
    [SerializeField] private TextMeshProUGUI biodiversity;

    [Header("Reef Visuals")]
    [SerializeField] private Image reefImage;
    public GameObject reefSecretary1;
    public GameObject reefSecretary2;

    [Header("Monitor")]
    [SerializeField] private GameObject monitorUI;
    private bool isMonitorOpened = false;

    [Header("Decisions")]
    [SerializeField] private GameObject expandedDecisionList;
    [SerializeField] private GameObject dropdownButton;
    [SerializeField] private GameObject decisionPrefab;
    public Transform decisionsContainer;
    [SerializeField] private Sprite dropdownClosed;
    [SerializeField] private Sprite dropdownOpened;
    [SerializeField] private TextMeshProUGUI currentDecisionsTaken;
    [SerializeField] private TextMeshProUGUI maxDecisions;
    public List<GameObject> decisionList = new List<GameObject>();
    private bool isDecisionListOpened = false;
    [Space]
    [SerializeField] private GameObject decisionDialogueBox;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private TextMeshProUGUI decisionContext;
    private TypingEffect typingEffect;

    [Header("Warning UI")]
    [SerializeField] private GameObject[] warningBadges; // 0:Funds / 1:Purity / 2:Bio
    [SerializeField] private GameObject warningPopup;
    [SerializeField] private TextMeshProUGUI warningText;

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
        typingEffect = decisionContext.GetComponent<TypingEffect>();
    }
    private void Start()
    {
        Init();

        maxDecisions.text = DecisionManager.instance.decisionHardCap.ToString();
    }

    private void Init()
    {
        if (ReefManager.Instance != null)
        {
            ReefManager.Instance.OnReefSwitched += UpdateReefUI;
        }

        if (ResourceManager.instance != null)
        {
            ResourceManager.instance.OnFundsChanged += UpdateFundsUI;
            ResourceManager.instance.OnPurityChanged += UpdatePurityUI;
            ResourceManager.instance.OnBiodiversityChanged += UpdateBiodiversityUI;
        }
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
            Image img = dropdownButton.GetComponent<Image>();

            if (isDecisionListOpened)
            {
                img.sprite = dropdownClosed;
                expandedDecisionList.SetActive(false);
            }
            else
            {
                img.sprite = dropdownOpened;
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
                speakerName.text = "Angela";
                break;
            case ReefType.Reef2:
                speakerName.text = "Dutch";
                break;
                /*
                case ReefType.Reef3:
                    speakerName.text = "Micah";
                    break;
                case ReefType.Reef4:
                    speakerName.text = "Your mom";
                    break;
                */
        }

        reefSecretary1.SetActive(true);
        decisionDialogueBox.SetActive(true);
        typingEffect.StartTyping(decision.decisionText);

    }
    public void OnClickNext()
    {
        if (ResourceManager.instance.isGameOver) return;

        if (typingEffect.isTyping)
        {
            typingEffect.SkipTyping();
            return;
        }
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

    public void ToggleWarningBadge(int index, bool isActive)
    {
        if (warningBadges[index] != null) warningBadges[index].SetActive(isActive);
    }

    public void OnClickWarningBadge(int index)
    {
        string resourceName = index == 0 ? "Funds" : (index == 1 ? "Purity" : "Biodiversity");
        warningText.text = $"Warning: {resourceName} is below 0!";
        warningPopup.SetActive(true);
    }

    public void CloseWarningPopup()
    {
        warningPopup.SetActive(false);
    }
}
