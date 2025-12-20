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
}
