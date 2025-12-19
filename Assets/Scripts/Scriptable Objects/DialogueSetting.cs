using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Dialogues/New Dialogue Setting")]
public class DialogueSetting : ScriptableObject
{
    [System.Serializable]
    public class Dialogue
    {
        public int day; // 해당 날짜
        public DialogueNode dialogueNode; // 출력할 대화 데이터
    }

    public List<Dialogue> dialogues;

    // 날짜에 해당되는 대화 찾는 함수
    public DialogueNode GetDialogueForDay(int day)
    {
        Dialogue entry = dialogues.Find(d => d.day == day);
        return entry?.dialogueNode;
    }
}