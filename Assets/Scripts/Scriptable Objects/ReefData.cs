using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu( menuName = "Reefs/New Reef")]
public class ReefData : ScriptableObject
{
    [Header("Reef Info")]
    public ReefType reefType;
    public string reefName;
    public int initialPurity;
    public int initialBiodiversity;

    [Header("Visuals")]
    public Sprite backgroundImage; 
    
    [Header("Decision Pool Reference")]
    public List<Decision> decisionPool;
}