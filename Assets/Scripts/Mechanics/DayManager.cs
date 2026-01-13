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

    [Header("Dialogue Setting")]
    [SerializeField] private DialogueSetting dialogueSetting;

    [HideInInspector] public int ending;

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

        if (currentDay == 2)
        {
            isDayActive = false;
            ending = EndGame();
            TitleScreenManager.instance.EndGame();
            return;
        }

        //events go first
        EventManager.instance.EvaluateEvents();

        UIManager.instance.RefreshDecisionAndEventUI();

        if (dialogueSetting != null)
        {
            DialogueNode todayDialogue = dialogueSetting.GetDialogueForDay(currentDay);
            if (todayDialogue != null) DialogueManager.Instance.StartDialogue(todayDialogue);
        }

        OnDayStart?.Invoke();

        AudioManager.instance.PlayBGM(AudioManager.instance.defaultBGM);
    }

    public int EndGame()
    {
        int purityReef1 = 0;
        int purityReef2 = 0;
        int biodiversityReef1 = 0;
        int biodiversityReef2 = 0;

        foreach (ReefData data in ReefManager.Instance.allReefData)
        {
            if (data.reefType == ReefType.Reef1)
            {
                purityReef1 = ResourceManager.instance.purityByReef[data.reefType];
                biodiversityReef1 = ResourceManager.instance.biodiversityByReef[data.reefType];
            }
            if (data.reefType == ReefType.Reef2)
            {
                purityReef2 = ResourceManager.instance.purityByReef[data.reefType];
                biodiversityReef2 = ResourceManager.instance.biodiversityByReef[data.reefType];
            }
        }

        float averagePurity = (purityReef1 + purityReef2) / 2;
        float averageBiodiversity = (biodiversityReef1 + biodiversityReef2) / 2;

        float overallAverage = (averagePurity + averageBiodiversity) / 2;

        /* Good Ending
         * Average purity OR Average Biodiversity (or both) above 70, and overall average above 60. Reef is saved
         * 
         * Neutral Ending
         * Average purity OR Average Biodiversity (or both) below 70 and above 50, and overall average below 60 and above 40. Neutral ending. Reef was preserved, but only momentarily. It is still endangered.
         * 
         * Bad Ending
         * Average purity OR Average Biodiversity (or both) below 50, and overall average below 40. Reef is facing extinction and environmental collapse. You have failed utterly at your task.
         * 
        */

        if ((averagePurity >= 70 || averageBiodiversity >= 70) && overallAverage >= 60)
        {
            Debug.Log("Triggered good ending. Yippee");
            return 1;
        }
        if ((averagePurity >= 50 && averagePurity < 70) || (averageBiodiversity >= 50 && averageBiodiversity < 70) && (overallAverage >= 40 && overallAverage < 60))
        {
            Debug.Log("Triggered neutral ending. Reef was saved, for now.");
            return 2;
        }
        if ((averagePurity < 50 || averageBiodiversity < 50) && overallAverage < 40)
        {
            Debug.Log("Bad ending. Reef was destroyed.");
            return 3;
        }
        return 0;
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