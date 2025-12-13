using System;
using UnityEngine;

// Create an Init() to subscribe to events, and call it from Start()
// Script for managing the start and end of each day

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }
    public int currentDay = 0; // 현재 게임 일수
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
        isDayActive = true;
        currentDay += 1;
        OnDayStart?.Invoke();

        //Debug.Log($"Day {currentDay} started");
    }

    // 하루 끝
    public void EndDay()
    {
        isDayActive = false;
        OnDayEnd?.Invoke();

        //Debug.Log($"Day {currentDay} ended");
    }

    // 하루 진행: 유저의 특정 행동 후 호출
    public void AdvanceDay()
    {
        EndDay();
    }

    // 현재 일수 반환
    public int GetCurrentDay() => currentDay;
}