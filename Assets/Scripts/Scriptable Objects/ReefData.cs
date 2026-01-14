using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Reefs/New Reef")]
public class ReefData : ScriptableObject
{
    [Header("Reef Info")]
    public ReefType reefType;
    public string reefName;
    public int initialPurity;
    public int initialBiodiversity;

    [Header("Visuals: Backgrounds")]
    public Sprite backgroundImage; // 실내 배경: 고정
    public List<Sprite> seaBackgroundImages;

    [Header("Visuals: Pollution Settings")]
    [Range(0, 100)] public int pollutionThreshold = 40; // 쓰레기가 생성되는 Purity 지점
    public List<Sprite> trashSprites;

    [Header("Decision Pool Reference")]
    public List<Decision> decisionPool;

}
