using System.Collections;
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
    [Space]
    [SerializeField] private GameObject decisionDialogueBox;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private TextMeshProUGUI decisionContext;
    [Space]
    [SerializeField] private GameObject decisionYesButton;
    [SerializeField] private GameObject decisionNoButton;
    [SerializeField] private GameObject decisionMaybeButton;
    [SerializeField] private GameObject eventButton;
    [SerializeField] private GameObject decisionEventYesButton;
    [SerializeField] private GameObject decisionEventNoButton;

    private bool isDecisionListOpened = false;
    private TypingEffect decisionTypingEffect;

    [Header("Events")]
    [SerializeField] private GameObject eventPrefab;
    [SerializeField] private GameObject regularEventBox;
    [SerializeField] private GameObject decisionEventBox;
    [SerializeField] private TextMeshProUGUI regularEventSpeaker;
    [SerializeField] private TextMeshProUGUI decisionEventSpeaker;
    [SerializeField] private TextMeshProUGUI regularEventContext;
    [SerializeField] private TextMeshProUGUI decisionEventContext;
    public TextMeshProUGUI regularEventButtonText;
    public TextMeshProUGUI decisionEventYesText;
    public TextMeshProUGUI decisionEventNoText;
    public bool activeEventsExist = false;

    private TypingEffect regularEventTypingEffect;
    private TypingEffect decisionEventTypingEffect;

    [Header("Decision Text Flicker")]
    [SerializeField] private TextMeshProUGUI decisionListText;
    [SerializeField] private float flickerInterval = 1f;

    private Color regularColor = Color.white;
    private Color warningColor = new Color(1f, 0.3f, 0.3f);

    [Header("Button Animation")]
    [SerializeField] private float slideDistance = 355f;
    [SerializeField] private float slideDuration = 0.25f;
    [SerializeField] private float slideDelay = 0.3f;

    private Dictionary<RectTransform, Vector2> initialPos = new();

    [Header("Warning UI")]
    [SerializeField] private GameObject[] warningBadges; // 0:Funds / 1:Purity / 2:Bio
    [SerializeField] private GameObject warningPopup;
    [SerializeField] private TextMeshProUGUI warningText;

    private Coroutine flickerRoutine;
    private Coroutine buttonRoutine;

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

        decisionTypingEffect = decisionContext.GetComponent<TypingEffect>();
        regularEventTypingEffect = regularEventContext.GetComponent<TypingEffect>();
        decisionEventTypingEffect = decisionEventContext.GetComponent<TypingEffect>();
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
    public void RefreshDecisionAndEventUI()
    {
        foreach (Transform child in decisionsContainer)
            Destroy(child.gameObject);

        decisionList.Clear();

        var dailyEvents = EventManager.instance.GetReadyEvents();

        Debug.Log($"Found {dailyEvents.Count} ready events.");

        if (dailyEvents.Count > 0)
        {
            activeEventsExist = true;
            InstantiateEvents(dailyEvents);
            StartDecisionFlicker();
        }
        else
        {
            activeEventsExist = false;
            StopDecisionFlicker();
        }

        var decisions = DecisionManager.instance.GetDailyDecisions(
            ReefManager.Instance.activeReefData,
            DayManager.Instance.currentDay
        );

        InstantiateDecisions(decisions);
    }
    public void InstantiateEvents(List<EventBase> events)
    {
        foreach (var e in events)
        {
            GameObject obj = Instantiate(eventPrefab, decisionsContainer);
            decisionList.Add(obj);

            EventUI ui = obj.GetComponent<EventUI>();
            ui.Bind(e);
        }
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
    private void StoreInitialPositions(List<GameObject> buttons)
    {
        foreach (var button in buttons)
        {
            RectTransform rect = button.GetComponent<RectTransform>();
            if (!initialPos.ContainsKey(rect))
                initialPos.Add(rect, rect.anchoredPosition);
        }
    }
    private void ResetButtonsOffscreen(List<GameObject> buttons)
    {
        foreach (var button in buttons)
        {
            RectTransform rect = button.GetComponent<RectTransform>();
            rect.anchoredPosition = initialPos[rect] + Vector2.right * slideDistance;
        }
    }
    public void BeginEventDialogue(EventBase newEvent)
    {
        if (DialogueManager.Instance.isDialogueBoxOpened) return;
        if (decisionEventBox.activeSelf || regularEventBox.activeSelf) return;

        List<GameObject> newButtons = new List<GameObject>();

        switch (newEvent.reefType)
        {
            case ReefType.Reef1:
                regularEventSpeaker.text = "Angela";
                reefSecretary1.SetActive(true);
                break;
            case ReefType.Reef2:
                regularEventSpeaker.text = "Dutch";
                reefSecretary2.SetActive(true);
                break;
        }

        if (newEvent is RegularEvent)
        {
            newButtons.Add(eventButton);

            foreach (var button in newButtons)
            {
                button.SetActive(false);
            }

            regularEventContext.text = newEvent.eventText;
            regularEventBox.SetActive(true);
            regularEventTypingEffect.StartTyping(newEvent.eventText, newButtons);
        }
        else
        {
            newButtons.Add(decisionEventYesButton);
            newButtons.Add(decisionEventNoButton);

            foreach (var button in newButtons)
            {
                button.SetActive(false);
            }

            decisionEventContext.text = newEvent.eventText;
            decisionEventBox.SetActive(true);
            decisionEventTypingEffect.StartTyping(newEvent.eventText, newButtons);
        }
    }
    public void BeginDecisionDialogue(Decision decision)
    {
        if (DialogueManager.Instance.isDialogueBoxOpened) return;
        if (decisionEventBox.activeSelf || regularEventBox.activeSelf) return;

        decisionContext.text = decision.decisionText;

        List<GameObject> newButtons = new List<GameObject>();

        switch (decision.reefType)
        {
            case ReefType.Reef1:
                speakerName.text = "Angela";
                reefSecretary1.SetActive(true);
                break;
            case ReefType.Reef2:
                speakerName.text = "Dutch";
                reefSecretary2.SetActive(true);
                break;
        }

        newButtons.Add(decisionYesButton);
        newButtons.Add(decisionNoButton);
        newButtons.Add(decisionMaybeButton);

        foreach (var button in newButtons)
        {
            button.SetActive(false);
        }

        decisionDialogueBox.SetActive(true);
        decisionTypingEffect.StartTyping(decision.decisionText, newButtons);
    }
    public void StartButtonAnimation(List<GameObject> buttons)
    {
        StoreInitialPositions(buttons);

        foreach (var button in buttons)
        {
            button.SetActive(true);
        }

        if (buttonRoutine != null)
        {
            StopCoroutine(buttonRoutine);
        }

        ResetButtonsOffscreen(buttons);

        buttonRoutine = StartCoroutine(AnimateDecisionButtons(buttons));
    }

    private IEnumerator AnimateDecisionButtons(List<GameObject> buttons)
    {
        List<RectTransform> rectTransforms = new();

        foreach (var b in buttons)
            rectTransforms.Add(b.GetComponent<RectTransform>());

        for (int i = 0; i < rectTransforms.Count; i++)
        {
            RectTransform rect = rectTransforms[i];

            Vector2 start = rect.anchoredPosition;
            Vector2 target = initialPos[rect];

            float elapsed = 0f;
            while (elapsed < slideDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / slideDuration);
                rect.anchoredPosition = Vector2.Lerp(start, target, t);
                yield return null;
            }

            rect.anchoredPosition = target;
            yield return new WaitForSeconds(slideDelay);
        }

        buttonRoutine = null;
    }
    public void OnClickNext()
    {
        if (ResourceManager.instance.isGameOver) return;

        if (decisionTypingEffect.isTyping)
        {
            decisionTypingEffect.SkipTyping();
            return;
        }
        if (decisionEventTypingEffect.isTyping)
        {
            decisionEventTypingEffect.SkipTyping();
            return;
        }
        if (regularEventTypingEffect.isTyping)
        {
            regularEventTypingEffect.SkipTyping();
            return;
        }
    }
    public void EndRegularEventDialogue()
    {
        reefSecretary1.SetActive(false);
        reefSecretary2.SetActive(false);

        regularEventBox.SetActive(false);
    }
    public void EndDecisionEventDialogue()
    {
        reefSecretary1.SetActive(false);
        reefSecretary2.SetActive(false);

        decisionEventBox.SetActive(false);
    }
    public void EndDecisionDialogue()
    {
        reefSecretary1.SetActive(false);
        reefSecretary2.SetActive(false);

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
    public void RemoveEvent(EventBase e)
    {
        for (int i = 0; i < decisionList.Count; i++)
        {
            EventUI script = decisionList[i].GetComponent<EventUI>();
            if (script != null)
            {
                if (script.GetActiveEvent().eventTitle == e.eventTitle)
                {
                    GameObject obj = decisionList[i];

                    decisionList.RemoveAt(i);
                    Destroy(obj);
                    break;
                }
            }
        }
    }
    public void RemoveDecision(Decision decision)
    {
        for (int i = 0; i < decisionList.Count; i++)
        {
            DecisionUI script = decisionList[i].GetComponent<DecisionUI>();
            if (script != null)
            {
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
    private IEnumerator DecisionFlicker()
    {
        bool isRed = false;

        while (EventManager.instance.activeEvents.Count != 0)
        {
            isRed = !isRed;
            decisionListText.color = isRed ? warningColor : regularColor;

            yield return new WaitForSeconds(flickerInterval);
        }

        decisionListText.color = regularColor;
        flickerRoutine = null;
    }
    private void StartDecisionFlicker()
    {
        if (flickerRoutine != null)
            StopCoroutine(flickerRoutine);

        flickerRoutine = StartCoroutine(DecisionFlicker());
    }
    private void StopDecisionFlicker()
    {
        if (flickerRoutine != null)
        {
            StopCoroutine(flickerRoutine);
            flickerRoutine = null;
        }

        decisionListText.color = regularColor;
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
