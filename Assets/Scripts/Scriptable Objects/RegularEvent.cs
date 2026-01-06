using UnityEngine;

[CreateAssetMenu(menuName = "Decisions/New Regular Event")]
public class RegularEvent : EventBase
{
    [Header("Resources")]
    public int fundsToAdd;
    public int purityToAdd;
    public int biodiversityToAdd;
    public int fundsToSubtract;
    public int purityToSubtract;
    public int biodiversityToSubtract;

    [Header("Text")]
    public string buttonFlavorText;

    public override void Execute()
    {
        state = EventState.Resolved;

        ReefType currentReef = reefType;

        ResourceManager.instance.AddFunds(fundsToAdd);
        ResourceManager.instance.AddPurity(currentReef, purityToAdd);
        ResourceManager.instance.AddBiodiversity(currentReef, biodiversityToAdd);

        ResourceManager.instance.SubtractFunds(fundsToSubtract);
        ResourceManager.instance.SubtractPurity(currentReef, purityToSubtract);
        ResourceManager.instance.SubtractBiodiversity(currentReef, biodiversityToSubtract);

        if (laterEvent != null)
        {
            laterEvent.daysSinceTrigger = 0;
            EventManager.instance.activeEvents.Add(laterEvent);
        }

        UIManager.instance.EndRegularEventDialogue();
        EventManager.instance.RemoveActiveEvent(eventTitle);
        UIManager.instance.RemoveEvent(this);
        UIManager.instance.RefreshDecisionAndEventUI();
        DecisionManager.instance.CheckProgress();
    }
    public override void ShowUI()
    {
        UIManager.instance.BeginEventDialogue(this);
    }
    public override bool AreConditionsMet(int funds, int purity, int biodiversity)
    {
        Debug.Log($"Resources: {funds}, {purity}, {biodiversity}");
        if (fundsToTrigger == 0 && purityToTrigger == 0 && biodiversityToTrigger == 0)
        {
            Debug.Log("No resources required for trigger");
            return true;
        }

        if (fundsToTrigger != 0)
        {
            if (fundsToTrigger >= 5000 && funds >= fundsToTrigger)
            {
                return true;
            }
            else if (fundsToTrigger < 5000 && funds <= fundsToTrigger)
            {
                return true;
            }
        }
        if (purityToTrigger != 0)
        {
            if (purityToTrigger >= 5000 && purity >= purityToTrigger)
            {
                return true;
            }
            else if (purityToTrigger < 5000 && purity <= purityToTrigger)
            {
                return true;
            }
        }
        if (biodiversityToTrigger != 0)
        {
            if (biodiversityToTrigger >= 5000 && biodiversity >= biodiversityToTrigger)
            {
                return true;
            }
            else if (biodiversityToTrigger < 5000 && biodiversity <= biodiversityToTrigger)
            {
                return true;
            }
        }
        return false;
    }
    public override void ChangeButtonText()
    {
        UIManager.instance.regularEventButtonText.text = buttonFlavorText;
    }
}
