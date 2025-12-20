using UnityEngine;
public class EventBase : ScriptableObject
{
    [Header("Data")]
    [TextArea(2, 7)]
    public string eventText;
    public string eventTitle;
    public ReefType reefType;

    [Header("Trigger Conditions")]
    public int timeToTrigger;
    public int fundsToTrigger;
    public int purityToTrigger;
    public int biodiversityToTrigger;

    [Header("Later Event")]
    public EventBase laterEvent;

    public int daysSinceTrigger;
}
