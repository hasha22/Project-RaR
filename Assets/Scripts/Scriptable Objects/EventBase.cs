using UnityEngine;
public abstract class EventBase : ScriptableObject
{
    [Header("Data")]
    [TextArea(2, 7)]
    public string eventText;
    public string eventTitle;
    public ReefType reefType;
    public EventState state = EventState.Pending;
    [HideInInspector] public int daysSinceTrigger = 0;

    [Header("Trigger Conditions")]
    public int timeToTrigger;
    public int fundsToTrigger;
    public int purityToTrigger;
    public int biodiversityToTrigger;

    [Header("Later Event")]
    public EventBase laterEvent;

    public abstract void Execute();
    public virtual void ExecuteChoice(bool affirmative)
    {
        Execute();
    }
    public abstract void ShowUI();
    public abstract bool AreConditionsMet(int funds, int purity, int biodiversity);
    public abstract void ChangeButtonText();
}
