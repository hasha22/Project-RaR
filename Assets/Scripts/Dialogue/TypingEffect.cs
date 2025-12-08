using UnityEngine;
using System.Collections;
using TMPro;

// Assigned to each Text UI

public class TypingEffect : MonoBehaviour
{
    public float typeSpeed = 0.03f; 
    public bool isTyping { get; private set; }

    private TextMeshProUGUI textUI;
    private Coroutine typingCoroutine;
    private string fullText = "";

    private void Awake()
    {
        textUI = GetComponent<TextMeshProUGUI>();
    }

    public void StartTyping(string text)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

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
    }
}