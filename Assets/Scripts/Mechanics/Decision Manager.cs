using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecisionManager : MonoBehaviour
{
    public static DecisionManager instance { get; private set; }

    [Header("Data")]
    //Decision pools for the 4 reefs. will be added manually
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
    public int firstReefHardCap;
    private int secondReefHardCap;
    private int thirdReefHardCap;
    private int fourthReefHardCap;
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
    private void Start()
    {
        // temporary on start. needs to be implemented to shuffle at the beginning of each day
        firstReefDecisions = GetRandomDecisions();
        UIManager.instance.InstantiateDecisions(firstReefDecisions);
    }
    public List<Decision> GetRandomDecisions()
    {
        List<Decision> temp = new List<Decision>(firstReefDecisionPool);

        for (int i = 0; i < temp.Count; i++)
        {
            int randIndex = Random.Range(i, temp.Count);
            (temp[i], temp[randIndex]) = (temp[randIndex], temp[i]);
        }

        // 5 is temporary, might have to change to account for game balance
        return temp.Take(5).ToList();
    }
    public void AssignDecision(Decision decision)
    {
        activeDecision = decision;
        // also temp, needs to be reef specific
        if (decisionsTakenFirstReef < firstReefHardCap) { UIManager.instance.BeginDecisionDialogue(decision); }
    }
    public void OnYesButtonPressed()
    {
        // Addition
        ResourceManager.instance.AddFunds(activeDecision.fundsToAddA);
        ResourceManager.instance.AddPurity(activeDecision.purityToAddA);
        ResourceManager.instance.AddBiodiversity(activeDecision.biodiversityToAddA);

        //Subtraction
        ResourceManager.instance.SubtractFunds(activeDecision.fundsToSubtractA);
        ResourceManager.instance.SubtractPurity(activeDecision.purityToSubtractA);
        ResourceManager.instance.SubtractBiodiversity(activeDecision.biodiversityToSubtractA);

        //TO DO: Handle everything else: UI closing, SFX, removing decision from daily list
        IncreaseDecisionsTaken();

        UIManager.instance.EndDecisionDialogue();
        UIManager.instance.RemoveDecision(activeDecision);
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

        //TO DO: Handle everything else: UI closing, SFX, removing decision from daily list
        IncreaseDecisionsTaken();

        UIManager.instance.EndDecisionDialogue();
        UIManager.instance.RemoveDecision(activeDecision);
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
