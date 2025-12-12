using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.AI;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance { get; private set; }

    [Header("Player Resources")]
    //these need to be assigned at the beginning of a day (most likely), not through inspector
    public int funds;
    // public int purity;
    // public int biodiversity;
    private Dictionary<ReefType, int> purityByReef = new Dictionary<ReefType, int>();
    private Dictionary<ReefType, int> biodiversityByReef = new Dictionary<ReefType, int>();
    public event Action<int> OnFundsChanged;
    public event Action<int> OnPurityChanged;
    public event Action<int> OnBiodiversityChanged;
    private ReefType activeReef = ReefType.None;

    private int maxFunds = 10000;
    private int maxPurity = 100;
    private int maxBiodiversity = 100;

    private int fundsAtStart;
    private Dictionary<ReefType, int> purityAtStart = new Dictionary<ReefType, int>();
    private Dictionary<ReefType, int> biodiversityAtStart = new Dictionary<ReefType, int>();

    public bool isGameOver { get; private set; }

    public int deltaFunds { get; private set; }
    public Dictionary<ReefType, int> deltaPurity = new Dictionary<ReefType, int>();
    public Dictionary<ReefType, int> deltaBiodiversity = new Dictionary<ReefType, int>();
    /////////////////////////////////////////////////////////////////////////////

    [Header("Resource UI Scripts")]
    public ResourceBarUI fundsUI;
    public ResourceBarUI purityUI;
    public ResourceBarUI biodiversityUI;

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
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnDayStart += AssignResources;
            DayManager.Instance.OnDayEnd += GetDailyChange;
            Debug.Log($"{name}: Subscribing to DayManager events");
        }

        if (ReefManager.Instance != null)
        {
            ReefManager.Instance.OnReefSwitched += OnReefSwitched;
            Debug.Log($"{name}: Subscribing to ReefManager events");
        }
    }

    private void OnReefSwitched(ReefType targetReef)
    {
        activeReef = targetReef;
        
        if (activeReef != ReefType.None && purityByReef.ContainsKey(activeReef))
        {
            // Reef가 전환될 경우 해당 Reef의 Purity/Biodiversity 값을 즉시 UI에 알림
            OnPurityChanged?.Invoke(purityByReef[activeReef]);
            OnBiodiversityChanged?.Invoke(biodiversityByReef[activeReef]);
        }
    }

    private void AssignResources()
    {
        fundsUI.SetMaxValue(maxFunds);
        purityUI.SetMaxValue(maxPurity);
        biodiversityUI.SetMaxValue(maxBiodiversity);
        
        if (DayManager.Instance.currentDay == 1)
        {
            funds = 2000;
        }

        foreach (ReefData data in ReefManager.Instance.allReefData) 
        {
            // day 1에만 초기값 설정: 이후 날짜에는 전날의 최종값을 씀
            if (DayManager.Instance.currentDay == 1)
            {
                purityByReef[data.reefType] = data.initialPurity;
                biodiversityByReef[data.reefType] = data.initialBiodiversity;
            }

            // 증감 계산을 위해 시작 값 기록: 매일 아침 갱신
            purityAtStart[data.reefType] = purityByReef[data.reefType];
            biodiversityAtStart[data.reefType] = biodiversityByReef[data.reefType];
        }

        fundsAtStart = funds;

        // UI는 OnFundsChanged / OnPurityChanged / OnBiodiversityChanged 이벤트 구독자에게 맡김
        // Funds는 글로벌 값이므로 이곳에서 한번 발송
        OnFundsChanged?.Invoke(funds);
        fundsUI.SetValue(funds);
        
        if (activeReef != ReefType.None && purityByReef.ContainsKey(activeReef))
        {
            // Purity와 Biodiversity는 현재 Active Reef의 값만 발송
            if (activeReef != ReefType.None && purityByReef.ContainsKey(activeReef))
            {
                OnPurityChanged?.Invoke(purityByReef[activeReef]);
                OnBiodiversityChanged?.Invoke(biodiversityByReef[activeReef]);
            }

            purityUI.SetValue(purityByReef[activeReef]);
            biodiversityUI.SetValue(biodiversityByReef[activeReef]);
        }
        
        /*
        UIManager.instance.UpdateFundsUI(funds);
        UIManager.instance.UpdatePurityUI(purityByReef[activeReef]);
        UIManager.instance.UpdateBiodiversityUI(biodiversityByReef[activeReef]);

        // 증감 계산을 위해 시작 값 기록
        fundsAtStart = funds;
        purityAtStart = purity;
        biodiversityAtStart = biodiversity;
        */
    }

    public void GetDailyChange()
    {
        deltaFunds = funds - fundsAtStart;
        foreach (ReefData data in ReefManager.Instance.allReefData) 
        {
            deltaPurity[data.reefType] = purityByReef[data.reefType] - purityAtStart[data.reefType];
            deltaBiodiversity[data.reefType] = biodiversityByReef[data.reefType] - biodiversityAtStart[data.reefType];
        }

        // Debug.Log($"delta funds: {deltaFunds}\ndelta purity: {deltaPurity}\ndelta biodiversity: {deltaBiodiversity}");
    }

    private void CheckGameOver()
    {
        if (funds <= 0) 
        {
            Debug.Log("GAME OVER by funds");

            isGameOver = true;
            DayManager.Instance.AdvanceDay();
        }

        foreach (ReefData data in ReefManager.Instance.allReefData)
        {
            if (purityByReef[data.reefType] <= 0 || biodiversityByReef[data.reefType] <= 0)
            {
                Debug.Log($"GAME OVER by purity of biodiversity");

                isGameOver = true;
                DayManager.Instance.AdvanceDay();
            }
        }
    }
    /////////////////////////////////////////////////////////////////////////////

    private void Start()
    {
        Init();

        /*
        fundsUI.SetMaxValue(maxFunds);
        purityUI.SetMaxValue(maxPurity);
        biodiversityUI.SetMaxValue(maxBiodiversity);

        fundsUI.SetValue(funds);
        purityUI.SetValue(purity);
        biodiversityUI.SetValue(biodiversity);

        UIManager.instance.UpdateFundsUI(funds);
        UIManager.instance.UpdatePurityUI(purity);
        UIManager.instance.UpdateBiodiversityUI(biodiversity);
        */
    }

    public void OnDayPassed()
    {
        //requires data about the day, how many resources to award the player
    }
    public void AddFunds(int newFunds)
    {
        funds += newFunds;
        OnFundsChanged?.Invoke(funds);
        fundsUI.SetValue(funds);
        UIManager.instance.UpdateFundsUI(funds);
    }
    public void AddPurity(ReefType targetReef, int newPurity)
    {
        if (targetReef == ReefType.None || !purityByReef.ContainsKey(targetReef)) return;
        
        purityByReef[targetReef] += newPurity;
        
        // 현재 활성 Reef인 경우에만 UI에 변경을 알립니다.
        if (targetReef == activeReef)
        {
            OnPurityChanged?.Invoke(purityByReef[targetReef]);
        }

        purityUI.SetValue(purityByReef[targetReef]);
        UIManager.instance.UpdatePurityUI(purityByReef[targetReef]);
    }
    public void AddBiodiversity(ReefType targetReef, int newBiodiversity)
    {
        if (targetReef == ReefType.None || !biodiversityByReef.ContainsKey(targetReef)) return;
        
        biodiversityByReef[targetReef] += newBiodiversity;
        
        // 현재 활성 Reef인 경우에만 UI에 변경을 알립니다.
        if (targetReef == activeReef)
        {
            OnBiodiversityChanged?.Invoke(biodiversityByReef[targetReef]);
        }

        biodiversityUI.SetValue(biodiversityByReef[targetReef]);
        UIManager.instance.UpdateBiodiversityUI(biodiversityByReef[targetReef]);
    }
    public void SubtractFunds(int newFunds)
    {
        funds -= newFunds;
        OnFundsChanged?.Invoke(funds);
        fundsUI.SetValue(funds);
        UIManager.instance.UpdateFundsUI(funds);

        CheckGameOver();
    }
    public void SubtractPurity(ReefType targetReef, int newPurity)
    {
        if (targetReef == ReefType.None || !purityByReef.ContainsKey(targetReef)) return;
        
        purityByReef[targetReef] -= newPurity;
        
        // 현재 활성 Reef인 경우에만 UI에 변경을 알립니다.
        if (targetReef == activeReef)
        {
            OnPurityChanged?.Invoke(purityByReef[targetReef]);
        }

        purityUI.SetValue(purityByReef[targetReef]);
        UIManager.instance.UpdatePurityUI(purityByReef[targetReef]);

        CheckGameOver();
    }
    public void SubtractBiodiversity(ReefType targetReef, int newBiodiversity)
    {
        if (targetReef == ReefType.None || !biodiversityByReef.ContainsKey(targetReef)) return;
        
        biodiversityByReef[targetReef] -= newBiodiversity;
        
        // 현재 활성 Reef인 경우에만 UI에 변경을 알립니다.
        if (targetReef == activeReef)
        {
            OnBiodiversityChanged?.Invoke(biodiversityByReef[targetReef]);
        }

        biodiversityUI.SetValue(biodiversityByReef[targetReef]);
        UIManager.instance.UpdateBiodiversityUI(biodiversityByReef[targetReef]);

        CheckGameOver();
    }
}