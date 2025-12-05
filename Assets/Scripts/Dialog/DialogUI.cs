using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Dialog UI 관리 스크립트
public class DialogUI : MonoBehaviour
{
    public static DialogUI Instance { get; private set; }

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contentText;
    public Button nextButton;
    public Transform selectPanel; // 활성화 비활성화로 토글
    public GameObject selectButtonPrefab;
    private string lastCharacterName = "";
    private Action onNextRequested;

    void Awake()
    {
        Instance = this;
        Hide();
    }

    public void ShowText(string name, string content, Action onNext = null)
    {
        gameObject.SetActive(true);
        // Name
        if (!string.IsNullOrEmpty(name)) lastCharacterName = name;
        nameText.text = string.IsNullOrEmpty(name) ? lastCharacterName : name;
        // Content
        contentText.text = content;
        // Select Panel
        selectPanel.gameObject.SetActive(false);
        // Next Button
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() => onNext?.Invoke());
        onNextRequested = onNext;
    }

    public void ShowSelectOptions(List<DialogSelectOption> options, Action<DialogSelectOption> onSelected)
    {
        selectPanel.gameObject.SetActive(true);

        // cleanup
        foreach (Transform t in selectPanel) Destroy(t.gameObject);

        foreach (var opt in options)
        {
            var go = Instantiate(selectButtonPrefab, selectPanel);
            var txt = go.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = opt.Text;
            var btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => { onSelected?.Invoke(opt); });
        }

        // Grid Layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(selectPanel.GetComponent<RectTransform>());
    }

    public void HideSelectPanel()
    {
        selectPanel.gameObject.SetActive(false);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}