using UnityEngine;

public class DecisionManager : MonoBehaviour
{
    public static DecisionManager instance { get; private set; }

    [Header("Data")]
    public Decision activeDecision;
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
    public void AssignDecision(Decision decision)
    {
        UIManager.instance.BeginDecisionDialogue(decision);
        activeDecision = decision;
    }
    public void OnYesButtonPressed()
    {
        Debug.Log("meow");
        // Addition
        ResourceManager.instance.AddFunds(activeDecision.fundsToAddA);
        ResourceManager.instance.AddPurity(activeDecision.purityToAddA);
        ResourceManager.instance.AddBiodiversity(activeDecision.biodiversityToAddA);

        //Subtraction
        ResourceManager.instance.SubtractFunds(activeDecision.fundsToSubtractA);
        ResourceManager.instance.SubtractPurity(activeDecision.purityToSubtractA);
        ResourceManager.instance.SubtractBiodiversity(activeDecision.biodiversityToSubtractA);

        //TO DO: Handle everything else: UI closing, SFX etc.
        UIManager.instance.EndDecisionDialogue();
    }
    public void OnNoButtonPressed()
    {
        // Addition
        ResourceManager.instance.AddFunds(activeDecision.fundsToAddN);
        ResourceManager.instance.AddPurity(activeDecision.purityToAddN);
        ResourceManager.instance.AddBiodiversity(activeDecision.biodiversityToAddN);

        //Subtraction
        ResourceManager.instance.SubtractFunds(activeDecision.fundsToSubtractN);
        ResourceManager.instance.SubtractPurity(activeDecision.purityToSubtractN);
        ResourceManager.instance.SubtractBiodiversity(activeDecision.biodiversityToSubtractN);

        //TO DO: Handle everything else: UI closing, SFX etc.
        UIManager.instance.EndDecisionDialogue();
    }
    public void OnMaybeButtonPressed()
    {
        UIManager.instance.EndDecisionDialogue();
    }
}
