using UnityEngine;

[CreateAssetMenu(menuName = "Decisions/New Decision Event")]
public class DecisionEvent : EventBase
{
    [Header("Text")]
    public string yesButtonText;
    public string noButtonText;

    [Header("Affirmative")]
    public int fundsToAddA;
    public int purityToAddA;
    public int biodiversityToAddA;
    public int fundsToSubtractA;
    public int purityToSubtractA;
    public int biodiversityToSubtractA;

    [Header("Negative")]
    public int fundsToAddN;
    public int purityToAddN;
    public int biodiversityToAddN;
    public int fundsToSubtractN;
    public int purityToSubtractN;
    public int biodiversityToSubtractN;

    public override void Execute()
    {

    }
    public override void ExecuteChoice(bool affirmative)
    {
        ReefType reef = ReefManager.Instance.activeReefType;

        if (affirmative)
        {
            ApplyOutcome(
                fundsToAddA,
                purityToAddA,
                biodiversityToAddA,
                fundsToSubtractA,
                purityToSubtractA,
                biodiversityToSubtractA,
                reef
            );
        }
        else
        {
            ApplyOutcome(
                fundsToAddN,
                purityToAddN,
                biodiversityToAddN,
                fundsToSubtractN,
                purityToSubtractN,
                biodiversityToSubtractN,
                reef
            );
        }
        if (laterEventA != null && affirmative)
        {
            laterEventA.daysSinceTrigger = 0;
            EventManager.instance.activeEvents.Add(laterEventA);
        }
        else if (laterEventN != null && !affirmative)
        {
            laterEventN.daysSinceTrigger = 0;
            EventManager.instance.activeEvents.Add(laterEventN);
        }

        UIManager.instance.EndDecisionEventDialogue();
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
        if (fundsToTrigger == 0 && purityToTrigger == 0 && biodiversityToTrigger == 0)
            return true;
        else
            Debug.Log("idk");

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
    private void ApplyOutcome(int addFunds, int addPurity, int addBiodiversity, int subFunds, int subPurity, int subBiodiversity, ReefType reef)
    {
        ResourceManager.instance.AddFunds(addFunds);
        ResourceManager.instance.SubtractFunds(subFunds);

        ResourceManager.instance.AddPurity(reef, addPurity);
        ResourceManager.instance.SubtractPurity(reef, subPurity);

        ResourceManager.instance.AddBiodiversity(reef, addBiodiversity);
        ResourceManager.instance.SubtractBiodiversity(reef, subBiodiversity);
    }
    public override void ChangeButtonText()
    {
        UIManager.instance.decisionEventYesText.text = yesButtonText;
        UIManager.instance.decisionEventNoText.text = noButtonText;
    }
}
