using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dialog 재생 담당(싱글톤)
// - PlayEvent(eventId) : 즉시 재생
// - PlayEventAtTime(eventId, DateTime) : 특정 시점에 재생
// - 다른 스크립트는 이 API를 호출해서 대화를 시작
public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    private DialogEvent currentEvent;
    private int currentIndex = 0; // index in Lines list (0-based)
    private bool isPlaying = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    //////////// Public API - 호출자

    // 외부에서 호출해 해당하는 EventID 대화를 재생하도록 함
    public void PlayEvent(int eventId)
    {
        Debug.Log("DialogManager: PlayEvent() ok");
        // DialogDatabase에서 eventId에 해당하는 DialogEvent 데이터를 가져옴
        var b = DialogDatabase.Instance.GetEvent(eventId);
        if (b == null) 
        {
            Debug.Log("B is NULL");
            return;
        }

        currentEvent = b;
        currentIndex = 0;
        isPlaying = true;
        ShowCurrentLine();
    }

    /*
    public void PlayEventAtTime(int eventId, DateTime targetUtc)
    {
        // 투두: 특정 시점에 재생 ?
        var now = DateTime.UtcNow;
        var diff = (targetUtc - now).TotalSeconds;
        if (diff <= 0) PlayEvent(eventId);
    }
    */

    //////////// 재생

    private void ShowCurrentLine()
    {
        Debug.Log("DialogManager: ShowCurrentLine() ok");
        if (!isPlaying || currentEvent == null) return;

        if (currentIndex >= currentEvent.Lines.Count)
        {
            EndEvent();
            return;
        }

        var line = currentEvent.Lines[currentIndex];

        // UI에 대사 출력
        DialogUI.Instance.ShowText(line.CharacterName, line.Text, OnAdvanceRequested);

        // 현재 라인에 SelectID가 있을 경우
        if (!string.IsNullOrEmpty(line.SelectID) && currentEvent.Selects.TryGetValue(line.SelectID, out var options))
        {
            // UI에 옵션 띄우고 유저가 선택한 경우 HandleSelect(opt) 호출
            DialogUI.Instance.ShowSelectOptions(options, (opt) => { HandleSelect(opt); });
        }
    }

    // 유저가 Next를 눌렀을 때 호출
    private void OnAdvanceRequested()
    {
        // 1. 현재 라인이 선택지로 처리 중인 경우 Next 동작은 무시: 선택지 클릭 기다림
        var line = currentEvent.Lines[currentIndex];
        if (!string.IsNullOrEmpty(line.SelectID) && currentEvent.Selects.ContainsKey(line.SelectID)) return;

        // 2. SkipAndMoveTo 값이 -1인 경우 해당 이벤트 끝
        if (line.SkipAndMoveTo == -1)
        {
            EndEvent();
            return;
        }

        // SkipAndMoveTo 값이 정상적으로 있는 경우 해당라인까지 출력 후 스킵라인으로 이동
        if (line.SkipAndMoveTo.HasValue && line.SkipAndMoveTo.Value > 0)
        {
            int target = line.SkipAndMoveTo.Value;
            int idx = currentEvent.Lines.FindIndex(l => l.DialogID == target);

            if (idx != -1) currentIndex = idx;
            else currentIndex += 1; // fallback

            ShowCurrentLine();
            return;
        }

        // 기본: 다음 라인으로 진행
        currentIndex += 1;
        ShowCurrentLine();
    }

    private void HandleSelect(DialogSelectOption opt)
    {
        // 선택지에 연결된 DialogID로 이동
        int targetDialogID = opt.DialogID;

        // 그 DialogID를 가진 라인의 인덱스 검색
        int idx = currentEvent.Lines.FindIndex(l => l.DialogID == targetDialogID);
        if (idx == -1) return;

        // 선택지로 이동한 라인 자체에 SkipAndMoveTo가 있을 경우 그 값을 읽어 다시 jump 처리
        var selectedLine = currentEvent.Lines[idx];
        if (selectedLine.SkipAndMoveTo > 0)
        {
            int skipIdx = currentEvent.Lines.FindIndex(l => l.DialogID == selectedLine.SkipAndMoveTo);
            if (skipIdx != -1) currentIndex = skipIdx;
            else currentIndex = idx + 1; // fallback
        }

        else
        {
            // 기본: 다음 라인부터 재생
            currentIndex = idx;
        }

        // 선택지를 닫고 해당 라인부터 재생
        DialogUI.Instance.HideSelectPanel();
        ShowCurrentLine();
    }

    private void EndEvent()
    {
        isPlaying = false;
        currentEvent = null;
        currentIndex = 0;
        DialogUI.Instance.Hide();
    }
}