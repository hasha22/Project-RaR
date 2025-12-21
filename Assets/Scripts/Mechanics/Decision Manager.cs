using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DayManager;

// Logic:
// 1) Decision Pools are filled with SOs in the Unity Editor
// 2) At the beginning of each day, a number of decisions gets randomly chosen for each reef
// 3) These are displayed in the decision list, whenever one decisions gets chosen it cannot be chosen again
// 4) Player has a hardcap for how many decisions can be taken each day for a section of the reef
public class DecisionManager : MonoBehaviour
{
    public static DecisionManager instance { get; private set; }

    [Header("Data")]
    public Decision activeDecision;
    private HashSet<string> takenDecisionTitles = new();
    [Space]
    public int dailyDecisionsTaken;
    public int decisionHardCap;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        if (ReefManager.Instance != null)
        {
            ReefManager.Instance.OnReefSwitched += (reefType) =>
            {
                UIManager.instance.RefreshDecisionAndEventUI();
            };
        }
    }
    /*
    public void UpdateDecisionPool(ReefType newReef)
    {
        if (ReefManager.Instance.activeReefData == null) return;

        ReefData data = ReefManager.Instance.activeReefData;

        // 기존 Decision 리스트 UI 초기화 및 새 Decision 생성

        ResetDecisionList(data);
    }

    public void ResetDecisionList(ReefData data)
    {
        // 기존 UI 버튼들 제거
        foreach (Transform child in UIManager.instance.decisionsContainer) Destroy(child.gameObject);

        UIManager.instance.decisionList.Clear();

        // 새로운 무작위 결정 리스트 확보 및 인스턴스화
        List<Decision> newDecisions = GetDailyDecisions(data, DayManager.Instance.currentDay);
        UIManager.instance.InstantiateDecisions(newDecisions);
    }
    */
    private void CheckProgress()
    {
        if (dailyDecisionsTaken >= decisionHardCap)
        {
            DayManager.Instance.AdvanceDay();
            dailyDecisionsTaken = 0;
            UIManager.instance.UpdateDecisionsTaken(dailyDecisionsTaken);
        }
    }

    public List<Decision> GetDailyDecisions(ReefData data, int currentDay)
    {
        if (data == null)
        {
            Debug.Log("GetDailyDecisions called with null ReefData.");
            return new List<Decision>();
        }

        if (DayManager.Instance.dailyDecisionCache.TryGetValue(data, out var cache))
        {
            if (cache.day == currentDay)
                return cache.decisions;
        }

        //filters out already taken decisions
        List<Decision> temp = data.decisionPool
        .Where(d => !DecisionManager.instance.IsDecisionTaken(d))
        .ToList();

        if (temp.Count == 0)
            return new List<Decision>();

        for (int i = 0; i < temp.Count; i++)
        {
            int randIndex = Random.Range(i, temp.Count);
            (temp[i], temp[randIndex]) = (temp[randIndex], temp[i]);
        }

        var newCache = new DailyDecisionCache
        {
            day = currentDay,
            decisions = temp.Take(3).ToList()
        };

        DayManager.Instance.dailyDecisionCache[data] = newCache;
        return newCache.decisions;
    }
    public void AssignDecision(Decision decision)
    {
        activeDecision = decision;
        if (dailyDecisionsTaken < decisionHardCap) { UIManager.instance.BeginDecisionDialogue(decision); }
    }
    public void OnYesButtonPressed()
    {
        MarkDecisionTaken(activeDecision);

        ReefType currentReef = activeDecision.reefType;

        // Addition
        ResourceManager.instance.AddFunds(activeDecision.fundsToAddA);
        ResourceManager.instance.AddPurity(currentReef, activeDecision.purityToAddA);
        ResourceManager.instance.AddBiodiversity(currentReef, activeDecision.biodiversityToAddA);

        //Subtraction
        ResourceManager.instance.SubtractFunds(activeDecision.fundsToSubtractA);
        ResourceManager.instance.SubtractPurity(currentReef, activeDecision.purityToSubtractA);
        ResourceManager.instance.SubtractBiodiversity(currentReef, activeDecision.biodiversityToSubtractA);

        dailyDecisionsTaken++;
        UIManager.instance.UpdateDecisionsTaken(dailyDecisionsTaken);

        if (activeDecision.eventToTriggerA != null)
        {
            activeDecision.eventToTriggerA.daysSinceTrigger = 0;
            EventManager.instance.activeEvents.Add(activeDecision.eventToTriggerA);

            if (activeDecision.eventToTriggerA.laterEvent != null)
            {
                activeDecision.eventToTriggerA.laterEvent.daysSinceTrigger = 0;
                EventManager.instance.activeEvents.Add(activeDecision.eventToTriggerA.laterEvent);
            }

            if (activeDecision.eventToTriggerA.timeToTrigger == 0)
            {
                //Trigger Event Immediately and activate event box immediately
            }
        }

        //removes from daily decision cache
        GetDailyDecisions(ReefManager.Instance.activeReefData, DayManager.Instance.currentDay).Remove(activeDecision);

        UIManager.instance.EndDecisionDialogue();
        UIManager.instance.RemoveDecision(activeDecision);

        CheckProgress();
    }
    public void OnNoButtonPressed()
    {
        MarkDecisionTaken(activeDecision);

        ReefType currentReef = activeDecision.reefType;

        // Addition
        ResourceManager.instance.AddFunds(activeDecision.fundsToAddN);
        ResourceManager.instance.AddPurity(currentReef, activeDecision.purityToAddN);
        ResourceManager.instance.AddBiodiversity(currentReef, activeDecision.biodiversityToAddN);

        //Subtraction
        ResourceManager.instance.SubtractFunds(activeDecision.fundsToSubtractN);
        ResourceManager.instance.SubtractPurity(currentReef, activeDecision.purityToSubtractN);
        ResourceManager.instance.SubtractBiodiversity(currentReef, activeDecision.biodiversityToSubtractN);

        dailyDecisionsTaken++;
        UIManager.instance.UpdateDecisionsTaken(dailyDecisionsTaken);

        if (activeDecision.eventToTriggerN != null)
        {
            activeDecision.eventToTriggerN.daysSinceTrigger = 0;
            EventManager.instance.activeEvents.Add(activeDecision.eventToTriggerN);

            if (activeDecision.eventToTriggerN.laterEvent != null)
            {
                activeDecision.eventToTriggerN.laterEvent.daysSinceTrigger = 0;
                EventManager.instance.activeEvents.Add(activeDecision.eventToTriggerN.laterEvent);
            }

            if (activeDecision.eventToTriggerN.timeToTrigger == 0)
            {
                //Trigger Event Immediately and activate event box immediately
            }
        }

        //removes from daily decision cache
        GetDailyDecisions(ReefManager.Instance.activeReefData, DayManager.Instance.currentDay).Remove(activeDecision);

        UIManager.instance.EndDecisionDialogue();
        UIManager.instance.RemoveDecision(activeDecision);

        CheckProgress();
    }
    public void OnMaybeButtonPressed()
    {
        UIManager.instance.EndDecisionDialogue();
    }
    private bool IsDecisionTaken(Decision decision)
    {
        return takenDecisionTitles.Contains(decision.decisionTitle);
    }
    private void MarkDecisionTaken(Decision decision)
    {
        takenDecisionTitles.Add(decision.decisionTitle);
    }
}
