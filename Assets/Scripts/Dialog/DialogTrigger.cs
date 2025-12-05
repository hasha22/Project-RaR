using System;
using UnityEngine;

// DialogManager.PlayEvent를 호출
// 다른 스크립트에서 이 메서드를 호출한 경우 대화 시작
// 예를 들어 
// DialogManager.Instance.PlayEvent(5);

// 특정 플레이어 행동(메서드 호출)으로 이벤트 호출
public class ActionTrigger : MonoBehaviour
{
    public int eventId = 1;

    public void InvokeDialog()
    {
        DialogManager.Instance.PlayEvent(eventId);
    }
}

// 게임 시각에 따른 호출
/*
public class ClockTrigger : MonoBehaviour
{
    public int eventId = 1;
    public DateTime triggerUtc;

    void Start()
    {
        // 특정 시각에 재생
        DialogManager.Instance.PlayEventAtTime(eventId, triggerUtc);
    }
}
*/