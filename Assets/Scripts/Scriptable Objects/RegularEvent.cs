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
}
