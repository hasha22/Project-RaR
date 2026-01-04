using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Assigned to each Choice Button
public class ChoiceController : MonoBehaviour
{
    private DialogueManager dialogueManager;
    private DialogueNode.Choice choiceData;

    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI contentText;

    // DialogueManager에서 호출되어 버튼을 초기화
    public void SetChoice(DialogueManager manager, DialogueNode.Choice choice, int index)
    {
        Debug.Log($"SetChoice called on {name} with text: {choice.text}");
        dialogueManager = manager;
        choiceData = choice;

        // 버튼 텍스트 설정
        contentText.text = choice.text;

        button = GetComponent<Button>();

        // 버튼 클릭 이벤트 리스너 등록
        button.onClick.RemoveAllListeners(); // 이전에 등록된 리스너 제거
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // DialogueManager에 선택된 Choice 데이터 전달
        Debug.Log($"Clicked button {name} with text: {contentText.text}");
        dialogueManager.HandleChoiceSelected(choiceData);
    }
}