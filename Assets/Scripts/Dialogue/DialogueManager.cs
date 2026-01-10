using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Overall flow:
// 1) 'StartDialogue(node)' is called externally
// 2) 'DialogueManager' displays the 'sequential talks' in order
// 3) If the last Talk contains 'choices', the choice UI is shown
// 4) When the user selects an option, 'HandleChoiceSelected()' moves to the next node.
// 5) If there are no choices, 'EndDialogue()' is called

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject dialogueBox; // 최고부모 
    [SerializeField] private GameObject talkPanel; // 대화 뭉텅이 1개
    [SerializeField] private TextMeshProUGUI talkerNameText;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private GameObject choicePanel; // 분기점
    [HideInInspector] public bool isDialogueBoxOpened = false;
    [SerializeField] private GameObject nextText;
    [SerializeField] private GameObject nextButton;

    [Header("Choice Button")]
    [SerializeField] private ChoiceController choiceButtonPrefab;
    [SerializeField] private Transform choiceButtonContainer;

    private DialogueNode currentDialogueNode;
    private int activeTalkIndex = 0;
    private List<ChoiceController> choiceButtons = new List<ChoiceController>();

    //added colors for dialogue clarity
    private Color speakerColor = Color.white;
    private Color backgroundColor = new Color(166f / 255f, 166f / 255f, 166f / 255f);


    private TypingEffect typingEffect;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        dialogueBox.SetActive(false);
        if (contentText != null) typingEffect = contentText.GetComponent<TypingEffect>();
        typingEffect.OnTypingComplete = () => { if (nextText != null) nextText.SetActive(true); };
    }

    // 외부에서 대화를 시작할 때 호출
    public void StartDialogue(DialogueNode startNode)
    {
        if (startNode == null) return;

        currentDialogueNode = startNode;
        activeTalkIndex = 0;
        dialogueBox.SetActive(true);

        GameObject reefSecretary = ReefManager.Instance.GetActiveReefSecretary();
        if (reefSecretary != null)
        {
            reefSecretary.SetActive(true);
        }

        isDialogueBoxOpened = true;

        ProgressDialogue();
    }

    // 유저의 클릭으로 다음 대화나 선택지로 진행
    public void OnClickNext()
    {
        // 게임오버인 경우 클릭 무시
        if (ResourceManager.instance.isGameOver) return;

        // 선택지가 활성화된 경우 클릭 무시
        if (choicePanel.activeSelf) return;

        if (typingEffect.isTyping)
        {
            typingEffect.SkipTyping();
            return;
        }

        ProgressDialogue();
    }

    private void ProgressDialogue()
    {
        // 1. 순차적 대화
        if (activeTalkIndex < currentDialogueNode.sequentialTalks.Length)
        {
            // talk panel 활성화
            // choice panel 비활성화
            talkPanel.SetActive(true);
            nextButton.SetActive(true);
            choicePanel.SetActive(false);
            nextText.SetActive(false);
            DestroyChoiceButtons();

            DialogueNode.Talk currentTalk = currentDialogueNode.sequentialTalks[activeTalkIndex];
            talkerNameText.text = currentTalk.talkerName;

            GameObject reefSecretary = ReefManager.Instance.GetActiveReefSecretary();
            Image secretaryImage = reefSecretary.GetComponent<Image>();

            if (!string.IsNullOrEmpty(currentTalk.highlightUIKey)) HighlightManager.Instance.SetHighlight(currentTalk.highlightUIKey);
            else HighlightManager.Instance?.SetHighlight(null);

            if (talkerNameText.text == "Manager") secretaryImage.color = backgroundColor;
            else secretaryImage.color = speakerColor;

            // contentText.text = currentTalk.content;
            typingEffect.StartTyping(currentTalk.content);

            activeTalkIndex += 1;
        }

        // TODO: 분기점 아니고 순차적 대화에서 다음 대화로 넘길 때의 버튼에 텍스트 넣을 수 있게 하기
        // 예를 들어 
        // 화자: 그래서 말이지
        // 버튼: 그래서? << 이걸 클릭해서 다음 대화로 넘어가는거임
        // 화자: 무슨무슨 일이 있더라고 ***

        // 2. 노드 마지막에 도달한 경우
        else
        {
            HighlightManager.Instance?.SetHighlight(null);

            nextText.SetActive(false);

            // 1) 선택지 분기점 처리
            if (currentDialogueNode.choices != null && currentDialogueNode.choices.Length > 0) ShowChoices();

            // 2) 대화 끝
            else EndDialogue();
        }
    }

    // 선택지 버튼
    private void ShowChoices()
    {
        choicePanel.SetActive(true);
        nextButton.SetActive(false);

        DestroyChoiceButtons();

        for (int i = 0; i < currentDialogueNode.choices.Length; i++)
        {
            DialogueNode.Choice choice = currentDialogueNode.choices[i];

            // 버튼 인스턴스화 및 설정
            ChoiceController button = Instantiate(choiceButtonPrefab, choiceButtonContainer);

            // 위치 조정: grid layout group 으로 설정

            // 데이터 설정 및 리스너 등록
            button.SetChoice(this, choice, i);

            choiceButtons.Add(button);
        }
    }

    // ChoiceButtonController에서 호출되어 선택지 처리 및 다음 노드 이동
    public void HandleChoiceSelected(DialogueNode.Choice selectedChoice)
    {
        // 1) 대화 끝
        EndDialogue();

        // 2) 다음 노드로 이동 (재귀 호출)
        if (selectedChoice.nextNode != null) StartDialogue(selectedChoice.nextNode);
    }

    // 생성된 선택지 버튼들 제거
    private void DestroyChoiceButtons()
    {
        foreach (var button in choiceButtons) Destroy(button.gameObject);
        choiceButtons.Clear();
    }

    // 대화 끝
    private void EndDialogue()
    {
        Debug.Log("Dialogue End");

        HighlightManager.Instance?.SetHighlight(null);

        dialogueBox.SetActive(false);

        GameObject reefSecretary = ReefManager.Instance.GetActiveReefSecretary();
        if (reefSecretary != null)
        {
            reefSecretary.SetActive(false);
        }

        isDialogueBoxOpened = false;
        currentDialogueNode = null;
        activeTalkIndex = 0;
        DestroyChoiceButtons();
    }
    public GameObject GetDialogueBox()
    {
        return dialogueBox;
    }
}