using UnityEngine;
using System.Collections.Generic;

public class HighlightManager : MonoBehaviour
{
    public static HighlightManager Instance;

    [SerializeField] private GameObject highlightOverlay; // 어두운 배경 panel
    [SerializeField] private RectTransform maskTransform; // 구멍 뚫릴 마스크

    // 인스펙터에서 키값과 실제 UI RectTransform 할당
    [System.Serializable]
    public struct UIEntry { public string key; public RectTransform targetUI; }
    public List<UIEntry> uiMappings;

    private RectTransform overlayRect;

    private void Awake()
    {
        Instance = this;
        
        overlayRect = highlightOverlay.GetComponent<RectTransform>();
    }

    public void SetHighlight(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            highlightOverlay.SetActive(false);
            return;
        }

        UIEntry entry = uiMappings.Find(x => x.key == key);
        if (entry.targetUI != null)
        {
            highlightOverlay.SetActive(true);
            // 1. 부모(마스크)를 타겟 UI 위치로 이동
            maskTransform.position = entry.targetUI.position;
            maskTransform.sizeDelta = entry.targetUI.sizeDelta;

            // 2. 자식(배경)의 월드 포지션을 화면 중앙으로 강제 고정
            overlayRect.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        }
    }
}