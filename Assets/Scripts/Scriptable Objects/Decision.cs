using UnityEngine;

[CreateAssetMenu(menuName = "Decisions/New Decision")]
public class Decision : ScriptableObject
{
    [Header("Data")]
    public ReefType reefType;
    public string decisionTitle;
    [TextArea(2, 7)]
    public string decisionText;

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

    //add flags for events that subtract/add resources over multiple days
}
