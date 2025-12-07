using UnityEngine;

public class Decision : ScriptableObject
{
    [Header("Data")]
    [TextArea(2, 7)]
    public string decisionText;

    [Header("Addition")]
    public int fundsToAdd;
    public int purityToAdd;
    public int biodiversityToAdd;

    [Header("Subtraction")]
    public int fundsToSubtract;
    public int purityToSubtract;
    public int biodiversityToSubtract;

    //add flags for events that subtract/add resources over multiple days
}
