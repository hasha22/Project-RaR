using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance { get; private set; }

    [Header("Events")]
    public EventBase currentActiveEvent;
    public List<EventBase> activeEvents = new();
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
    public void IncreaseDaysSinceTrigger()
    {
        for (int i = 0; i < activeEvents.Count; i++)
        {
            activeEvents[i].daysSinceTrigger++;
            Debug.Log($"Event `{activeEvents[i].eventTitle}`: {activeEvents[i].daysSinceTrigger}. ");
        }
    }
    public void EvaluateEvents()
    {
        foreach (var e in activeEvents)
        {
            bool timeMet = TimeConditionMet(e);
            bool resourcesMet = false;

            if (timeMet)
            {
                resourcesMet = ResourceConditionsMet(e);
            }
            else
            {
                Debug.Log($"Time condition NOT met for event: {e.eventTitle}");
            }

            if (timeMet && resourcesMet)
            {
                Debug.Log($"Event READY: {e.eventTitle}");
                e.state = EventState.Ready;
            }
        }

        Debug.Log("Events Evaluated!");
    }
    private bool TimeConditionMet(EventBase e)
    {
        if (e.daysSinceTrigger >= e.timeToTrigger)
            Debug.Log($"Time Condition Met for event: {e.eventTitle}.");

        return e.daysSinceTrigger >= e.timeToTrigger;
    }
    private bool ResourceConditionsMet(EventBase e)
    {
        ReefType reef = DetermineReef(e);

        int funds = ResourceManager.instance.funds;
        int purity = ResourceManager.instance.purityByReef[reef];
        int biodiversity = ResourceManager.instance.biodiversityByReef[reef];

        bool met = e.AreConditionsMet(funds, purity, biodiversity);

        Debug.Log(
            $"Resource check for {e.eventTitle} " +
            $"Funds: {funds}, Purity: {purity}, Biodiversity: {biodiversity} -> {met}"
        );

        return met;
    }
    public List<EventBase> GetReadyEvents()
    {
        return activeEvents
        .Where(e => e.state == EventState.Ready)
        .ToList();
    }
    public void OnEventButtonPressed()
    {
        currentActiveEvent.Execute();
    }
    public void OnYesPressed()
    {
        currentActiveEvent.ExecuteChoice(true);
    }
    public void OnNoPressed()
    {
        currentActiveEvent.ExecuteChoice(false);
    }
    public void RemoveActiveEvent(string title)
    {
        currentActiveEvent = null;
        for (int i = 0; i < activeEvents.Count; i++)
        {
            if (activeEvents[i].eventTitle == title)
            {
                activeEvents.Remove(activeEvents[i]);
                if (activeEvents.Count == 0)
                { UIManager.instance.activeEventsExist = false; }
                break;
            }
        }
    }
    //helper method
    private ReefType DetermineReef(EventBase activeEvent)
    {
        switch (activeEvent.reefType)
        {
            case ReefType.Reef1:
                return ReefType.Reef1;
            case ReefType.Reef2:
                return ReefType.Reef2;
        }
        return ReefType.None;
    }
}
