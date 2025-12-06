using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue System/Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    // 1. 순차적 대화
    [System.Serializable]
    public class Talk
    {
        public string talkerName;
        [TextArea(2, 7)]
        public string content;
    }

    public Talk[] sequentialTalks; // 순차적 대화 목록

    // 2. 분기점
    [System.Serializable]
    public class Choice
    {
        public string text;

        // 이동할 다음 Dialogue 노드 (분기 설정)
        // 값 없는 경우 넘어감
        public DialogueNode nextNode;

        // TODO: stat / flag 데이터 관리

        public Decision decision;
        /*
        public string flagToSet;
        public bool flagValue;
        */
    }

    public Choice[] choices; // 선택지 목록
}