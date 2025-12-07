using UnityEngine;

public class DecisionUI : MonoBehaviour
{
    // At the beginning of each day, these decisions *should* get assigned for each UI element. This assignment includes changing the decision variable
    // as well as the decision title in the dropdown list using decision.decisionTitle.
    // Each day has a pool of decisions, out of which a a few are picked at random for the player to interact with.

    [Header("Data")]
    public Decision decision;
    public void OnDecisionClicked()
    {
        if (decision != null)
        {
            Debug.Log("meow");
            DecisionManager.instance.AssignDecision(decision);
        }
        else
        {
            Debug.Log("No decision assigned!");
        }
    }
}
