using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance { get; private set; }

    [Header("Events")]
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
        }
    }
    public void CheckAllEvents()
    {
        for (int i = 0; i < activeEvents.Count; i++)
        {
            // used to determine which reef specific resources to affect
            ReefType reef = DetermineReef(activeEvents[i]);
            int reefPurity = ResourceManager.instance.purityByReef[reef];
            int reefBiodiversity = ResourceManager.instance.biodiversityByReef[reef];

            //events only fire after a set amount of days have passed
            if (activeEvents[i].timeToTrigger >= activeEvents[i].daysSinceTrigger)
            {
                if (activeEvents[i].fundsToTrigger == 0 && activeEvents[i].purityToTrigger == 0 && activeEvents[i].biodiversityToTrigger == 0)
                {
                    //Trigger day based event
                }
                TriggerResourceBasedEvent(activeEvents[i], ResourceManager.instance.funds, reefPurity, reefBiodiversity, reef);
            }
        }
    }
    public List<EventBase> GetReadyEvents()
    {
        List<EventBase> readyEvents = new();
        foreach (var e in activeEvents)
        {
            if (!e.hasBeenSeen && e.daysSinceTrigger >= e.timeToTrigger)
            {
                readyEvents.Add(e);
            }
        }

        return readyEvents;
    }
    public void RemoveActiveEvent(string title)
    {
        for (int i = 0; i < activeEvents.Count; i++)
        {
            if (activeEvents[i].eventTitle == title)
            {
                activeEvents.Remove(activeEvents[i]);
                break;
            }
        }
    }
    private void TriggerResourceBasedEvent(EventBase activeEvent, int funds, int purity, int biodiversity, ReefType reef)
    {
        if (activeEvent.fundsToTrigger != 0)
        {
            //Trigger Event
            if (activeEvent.fundsToTrigger >= 5000 && funds >= activeEvent.fundsToTrigger)
            {

            }
            else if (activeEvent.fundsToTrigger < 5000 && funds <= activeEvent.fundsToTrigger)
            {

            }
        }
        if (activeEvent.purityToTrigger != 0)
        {
            //Trigger Event
            if (activeEvent.purityToTrigger >= 50 && purity >= activeEvent.purityToTrigger)
            {

            }
            else if (activeEvent.purityToTrigger < 50 && purity <= activeEvent.purityToTrigger)
            {

            }
        }
        if (activeEvent.biodiversityToTrigger != 0)
        {
            //Trigger Event
            if (activeEvent.biodiversityToTrigger >= 50 && biodiversity >= activeEvent.biodiversityToTrigger)
            {

            }
            else if (activeEvent.biodiversityToTrigger < 50 && biodiversity <= activeEvent.biodiversityToTrigger)
            {

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
