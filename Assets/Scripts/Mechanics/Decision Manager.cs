using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Logic:
// 1) Decision Pools are filled with SOs in the Unity Editor
// 2) At the beginning of each day, a number of decisions gets randomly chosen for each reef
// 3) These are displayed in the decision list, whenever one decisions gets chosen it cannot be chosen again
// 4) Player has a hardcap for how many decisions can be taken each day for a section of the reef
public class DecisionManager : MonoBehaviour
{
    public static DecisionManager instance { get; private set; }

    [Header("Data")]
    //Decision pools for the 4 reefs.
    [SerializeField] private List<Decision> firstReefDecisionPool = new List<Decision>();
    [SerializeField] private List<Decision> secondReefDecisionPool = new List<Decision>();
    [SerializeField] private List<Decision> thirdReefDecisionPool = new List<Decision>();
    [SerializeField] private List<Decision> fourthReefDecisionPool = new List<Decision>();
    [Space]
    //decision lists that get filled in at the beginning of each day
    public List<Decision> firstReefDecisions;
    public List<Decision> secondReefDecisions;
    public List<Decision> thirdReefDecisions;
    public List<Decision> fourthReefDecisions;
    public Decision activeDecision;
    [Space]
    public int decisionsTakenFirstReef;
    public int decisionsTakenSecondReef;
    public int decisionsTakenThirdReef;
    public int decisionsTakenFourthReef;
    [Space]
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
    private void Init()
    {
        if (ReefManager.Instance != null)
        {
            ReefManager.Instance.OnReefSwitched += UpdateDecisionPool;
            Debug.Log($"{name}: Subscribing to ReefManager events");
        }
    }

    private void Start()
    {
        ReefManager.Instance.SetNewReef(ReefManager.Instance.allReefData[0]);
        UpdateDecisionPool(ResourceManager.instance.activeReef);
        Init();

        // temporary on start. needs to be implemented to shuffle at the beginning of each day
        // firstReefDecisions = GetRandomDecisions();
        // UIManager.instance.InstantiateDecisions(firstReefDecisions);
    }

    private void UpdateDecisionPool(ReefType newReef)
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

        //Debug.Log($"Decisions updated for {ReefManager.Instance.activeReefType}");
    }

    private void CheckProgress()
    {
        // temporary. could have advancing to the next day be done with a UI button
        if (decisionsTakenFirstReef >= decisionHardCap) DayManager.Instance.AdvanceDay();
    }

    public List<Decision> GetDailyDecisions(ReefData data, int currentDay)
    {
        Debug.Log(data.lastGeneratedDay);
        if (data.dailyDecisions != null && data.lastGeneratedDay == currentDay)
            return data.dailyDecisions;

        List<Decision> temp = new List<Decision>(data.decisionPool);

        for (int i = 0; i < temp.Count; i++)
        {
            int randIndex = Random.Range(i, temp.Count);
            (temp[i], temp[randIndex]) = (temp[randIndex], temp[i]);
        }

        // 5 is temporary, might have to change to account for game balance
        data.dailyDecisions = temp.Take(5).ToList();
        data.lastGeneratedDay = currentDay;

        return data.dailyDecisions;
    }
    public void AssignDecision(Decision decision)
    {
        activeDecision = decision;
        // also temp, needs to be reef specific
        if (decisionsTakenFirstReef < decisionHardCap) { UIManager.instance.BeginDecisionDialogue(decision); }
    }
    public void OnYesButtonPressed()
    {
        ReefType currentReef = activeDecision.reefType;

        // Addition
        ResourceManager.instance.AddFunds(activeDecision.fundsToAddA);
        ResourceManager.instance.AddPurity(currentReef, activeDecision.purityToAddA);
        ResourceManager.instance.AddBiodiversity(currentReef, activeDecision.biodiversityToAddA);

        //Subtraction
        ResourceManager.instance.SubtractFunds(activeDecision.fundsToSubtractA);
        ResourceManager.instance.SubtractPurity(currentReef, activeDecision.purityToSubtractA);
        ResourceManager.instance.SubtractBiodiversity(currentReef, activeDecision.biodiversityToSubtractA);

        IncreaseDecisionsTaken();

        ReefManager.Instance.activeReefData.dailyDecisions.Remove(activeDecision);

        UIManager.instance.EndDecisionDialogue();
        UIManager.instance.RemoveDecision(activeDecision);

        CheckProgress();
    }
    public void OnNoButtonPressed()
    {
        ReefType currentReef = activeDecision.reefType;

        // Addition
        ResourceManager.instance.AddFunds(activeDecision.fundsToAddN);
        ResourceManager.instance.AddPurity(currentReef, activeDecision.purityToAddN);
        ResourceManager.instance.AddBiodiversity(currentReef, activeDecision.biodiversityToAddN);

        //Subtraction
        ResourceManager.instance.SubtractFunds(activeDecision.fundsToSubtractN);
        ResourceManager.instance.SubtractPurity(currentReef, activeDecision.purityToSubtractN);
        ResourceManager.instance.SubtractBiodiversity(currentReef, activeDecision.biodiversityToSubtractN);

        IncreaseDecisionsTaken();

        UIManager.instance.EndDecisionDialogue();
        UIManager.instance.RemoveDecision(activeDecision);

        CheckProgress();
    }
    public void OnMaybeButtonPressed()
    {
        UIManager.instance.EndDecisionDialogue();
    }
    private void IncreaseDecisionsTaken()
    {
        switch (activeDecision.reefType)
        {
            case ReefType.Reef1:
                decisionsTakenFirstReef++;
                UIManager.instance.UpdateDecisionsTaken(decisionsTakenFirstReef);
                break;
            case ReefType.Reef2:
                decisionsTakenSecondReef++;
                UIManager.instance.UpdateDecisionsTaken(decisionsTakenSecondReef);
                break;
            case ReefType.Reef3:
                decisionsTakenThirdReef++;
                UIManager.instance.UpdateDecisionsTaken(decisionsTakenThirdReef);
                break;
            case ReefType.Reef4:
                decisionsTakenFourthReef++;
                UIManager.instance.UpdateDecisionsTaken(decisionsTakenFourthReef);
                break;
        }
    }
}
