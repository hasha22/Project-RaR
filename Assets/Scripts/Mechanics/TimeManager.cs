using UnityEngine;
using System;

// Use subscription like this: 
// private void OnEnable() { TimeManager.instance.OnDayStart += FUNCTION; }
// private void OnDisable() { TimeManager.instance.OnDayStart -= FUNCTION; }
// private void FUNCTION() { }

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    public int currentDay = 1; // 현재 게임 일수
    public bool isDayActive = false;

    // 이벤트: 하루 시작 / 끝
    public event Action OnDayStart;
    public event Action OnDayEnd;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // 하루 시작
    public void StartDay()
    {
        Debug.Log($"Day {currentDay} started");

        isDayActive = true;
        currentDay += 1;
        OnDayStart?.Invoke();
    }

    // 하루 끝: 자동으로 다음 날 시작
    public void EndDay()
    {
        Debug.Log($"Day {currentDay} ended");

        isDayActive = false;
        OnDayEnd?.Invoke();
        StartDay();
    }

    // 하루 진행: 유저의 특정 행동 후 호출
    public void AdvanceDay()
    {
        EndDay();
    }

    // 현재 일수 반환
    public int GetCurrentDay() => currentDay;

    // 게임 시간 초기화
    public void ResetTime(int startDay = 1)
    {
        currentDay = startDay;
        isDayActive = false;
    }
}