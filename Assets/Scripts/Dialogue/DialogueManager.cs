using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialogueBox; // 최고부모 
    [SerializeField] private GameObject talkPanel; // 대화 뭉텅이 1개
    [SerializeField] private TextMeshProUGUI talkerNameText;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private GameObject choicePanel; // 분기점 

    [Header("Choice Button")]
    [SerializeField] private ChoiceController choiceButtonPrefab;
    [SerializeField] private Transform choiceButtonContainer;
    [SerializeField] private float choiceButtonSpacing = 10f;

    private DialogueNode currentDialogueNode;
    private int activeTalkIndex = 0;
    private List<ChoiceController> choiceButtons = new List<ChoiceController>();

    private void Awake() { dialogueBox.SetActive(false); }

    // 외부에서 대화를 시작할 때 호출
    public void StartDialogue(DialogueNode startNode)
    {
        if (startNode == null) return;

        currentDialogueNode = startNode;
        activeTalkIndex = 0;
        dialogueBox.SetActive(true);

        ProgressDialogue();
    }

    // 유저의 클릭으로 다음 대화나 선택지로 진행
    public void OnClickNext()
    {
        // 선택지가 활성화된 상태에서는 클릭 무시
        if (choicePanel.activeSelf) return;

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
            choicePanel.SetActive(false);
            DestroyChoiceButtons();

            DialogueNode.Talk currentTalk = currentDialogueNode.sequentialTalks[activeTalkIndex];
            talkerNameText.text = currentTalk.talkerName;
            contentText.text = currentTalk.content;

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
            // 1) 선택지 분기점 처리
            if (currentDialogueNode.choices != null && currentDialogueNode.choices.Length > 0) ShowChoices();

            // 2) 대화 끝
            else EndDialogue();
        }
    }

    // 선택지 버튼
    private void ShowChoices()
    {
        talkPanel.SetActive(false);
        choicePanel.SetActive(true);

        DestroyChoiceButtons();

        for (int i = 0; i < currentDialogueNode.choices.Length; i++)
        {
            DialogueNode.Choice choice = currentDialogueNode.choices[i];

            // 버튼 인스턴스화 및 설정
            ChoiceController button = Instantiate(choiceButtonPrefab, choiceButtonContainer);

            // 위치 조정
            // TODO: grid layout group 으로 설정하기
            button.transform.localPosition = Vector3.down * choiceButtonSpacing * i;

            // 데이터 설정 및 리스너 등록
            button.SetChoice(this, choice, i);

            choiceButtons.Add(button);
        }
    }

    // ChoiceButtonController에서 호출되어 선택지 처리 및 다음 노드 이동
    public void HandleChoiceSelected(DialogueNode.Choice selectedChoice)
    {
        //Decision currentDecision = selectedChoice.decision;

        // 2. 대화 끝
        EndDialogue();

        // 3. 다음 노드로 이동 (재귀 호출)
        if (selectedChoice.nextNode != null) StartDialogue(selectedChoice.nextNode);
    }

    // 생성된 선택지 버튼들 제거
    private void DestroyChoiceButtons()
    {
        foreach (var button in choiceButtons) Destroy(button.gameObject);
        choiceButtons.Clear();
    }

    // 대화 종료
    private void EndDialogue()
    {
        Debug.Log("Dialogue End");

        dialogueBox.SetActive(false);
        currentDialogueNode = null;
        activeTalkIndex = 0;
        DestroyChoiceButtons();
    }
}