using UnityEngine;

// 테스트 스크립트
public class TestManager : MonoBehaviour
{
    [SerializeField] private DialogueNode testDialogueNode;
    [SerializeField] private DialogueManager dialogueManager;

    private void Start()
    { 
        DayManager.Instance.StartDay();
        dialogueManager.StartDialogue(testDialogueNode); 
    }
}