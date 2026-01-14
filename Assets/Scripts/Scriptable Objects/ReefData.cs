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
    public int seaChangeThreshold = 50; // 바다 배경 바뀌는 Purity 임계값

    [Header("Visuals: Pollution Settings")]
    public List<int> trashThresholds; // 쓰레기가 생성되는 Purity 임계값들
    public List<Sprite> trashSprites;

    [Header("Decision Pool Reference")]
    public List<Decision> decisionPool;

}
