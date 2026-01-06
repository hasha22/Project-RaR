using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Assigned to each Text UI

public class TypingEffect : MonoBehaviour
{
    public float typeSpeed = 0.03f;
    public bool isTyping { get; private set; }

    private TextMeshProUGUI textUI;
    private Coroutine typingCoroutine;
    private string fullText = "";
    private List<GameObject> newButtons = new List<GameObject>();

    public Action OnTypingComplete;

    private void Awake()
    {
        textUI = GetComponent<TextMeshProUGUI>();
    }
    // added optional buttons list to make buttons slide-in after typing has been finished
    public void StartTyping(string text, List<GameObject> buttons = null)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        if (buttons != null) newButtons = buttons;

        fullText = text;
        textUI.text = "";
        typingCoroutine = StartCoroutine(TypingRoutine());
    }

    // If typing is in progress, skip: instantly display the full sentence
    public void SkipTyping()
    {
        if (!isTyping) return;

        StopCoroutine(typingCoroutine);
        textUI.text = fullText;
        isTyping = false;
        UIManager.instance.StartButtonAnimation(newButtons);
        OnTypingComplete?.Invoke();
    }

    private IEnumerator TypingRoutine()
    {
        isTyping = true;

        for (int i = 0; i < fullText.Length; i++)
        {
            textUI.text = fullText.Substring(0, i + 1);
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
        UIManager.instance.StartButtonAnimation(newButtons);
        OnTypingComplete?.Invoke();
    }
}