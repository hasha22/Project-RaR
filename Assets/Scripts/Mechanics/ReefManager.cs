using System;
using System.Collections.Generic;
using UnityEngine;

// Script for managing reef state and dispatching switch events
public class ReefManager : MonoBehaviour
{
    public static ReefManager Instance { get; private set; }

    [Header("Data")]
    public List<ReefData> allReefData;

    public ReefType activeReefType { get; private set; }
    public ReefData activeReefData { get; private set; }

    // Reef가 전환될 때마다 UI와 DecisionManager에게 알리는 이벤트
    public event Action<ReefType> OnReefSwitched;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        DontDestroyOnLoad(this);

        //changed to reef1
        activeReefType = ReefType.Reef1;
    }

    public void SwitchReef(ReefType targetReef)
    {
        if (activeReefType == targetReef) return;

        ReefData data = allReefData.Find(r => r.reefType == targetReef);
        if (data == null) return;

        activeReefType = targetReef;
        activeReefData = data;

        //Debug.Log($"Switched to {targetReef}");

        OnReefSwitched?.Invoke(activeReefType);
    }
    public void SetNewReef(ReefData newReefData)
    {
        activeReefData = newReefData;
    }
    public GameObject GetActiveReefSecretary()
    {
        switch (activeReefType)
        {
            case ReefType.Reef1:
                return UIManager.instance.reefSecretary1;
            case ReefType.Reef2:
                return UIManager.instance.reefSecretary2;
        }
        return null;
    }
}