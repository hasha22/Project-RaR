using System;
using System.Collections.Generic;
using UnityEngine;

// Dialog 관련 데이터 타입들을 모아둔 파일

[Serializable]
public class DialogLine
{
    public int EventID; // 대화 뭉텅이 ID
    public int DialogID; // 이벤트 내 순서 ID
    public string CharacterName;
    public string Text;
    public string SelectID;  // 빈 경우 선택지 없음
    public int? SkipAndMoveTo; // 빈 경우 쓰지 않음
}

[Serializable]
public class DialogSelectOption
{
    public string SelectID; 
    public int DialogID; // 선택지 선택 시 연결되는 DialogID (반응 라인)
    public string Text; 
}

[Serializable]
public class DialogEvent
{
    public int EventID;
    public List<DialogLine> Lines = new List<DialogLine>();
    // SelectID >> 옵션 리스트
    public Dictionary<string, List<DialogSelectOption>> Selects = new Dictionary<string, List<DialogSelectOption>>();
}