using System;
using System.Collections.Generic;
using UnityEngine;

// Create an Init() to subscribe to events, and call it from Start()
// Script for managing the start and end of each day

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }
    public int currentDay = 0; // 현재 게임 일수
    public bool isDayActive = false;

    public int daysWithDeficit = 0; // 적자로 끝난 날수
    public int maxAllowedDeficitDays = 3; // 적자 허락되는 최대 일수

    public Dictionary<ReefData, DailyDecisionCache> dailyDecisionCache = new Dictionary<ReefData, DailyDecisionCache>();

    // 이벤트: 하루 시작 / 끝
    public event Action OnDayStart;
    public event Action OnDayEnd;

    [Header("Dialog Setting")]
    [SerializeField] private DialogueSetting dialogueSetting;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
        ReefManager.Instance.SetNewReef(ReefManager.Instance.allReefData[0]);
        StartDay();
    }
    // 하루 시작
    public void StartDay()
    {
        isDayActive = true;
        currentDay += 1;

        //events go first
        EventManager.instance.EvaluateEvents();

        UIManager.instance.RefreshDecisionAndEventUI();

        if (dialogueSetting != null)
        {
            DialogueNode todayDialogue = dialogueSetting.GetDialogueForDay(currentDay);
            if (todayDialogue != null) DialogueManager.Instance.StartDialogue(todayDialogue);
        }

        OnDayStart?.Invoke();
    }

    // 하루 끝
    public void EndDay()
    {
        isDayActive = false;
        ResourceManager.instance.CheckGameOver();
        OnDayEnd?.Invoke();

    }

    // 하루 진행: 유저의 특정 행동 후 호출
    public void AdvanceDay()
    {
        EventManager.instance.IncreaseDaysSinceTrigger();
        EndDay();
    }

    // 현재 일수 반환
    public int GetCurrentDay() => currentDay;

    public class DailyDecisionCache
    {
        public int day;
        public List<Decision> decisions;
    }
}