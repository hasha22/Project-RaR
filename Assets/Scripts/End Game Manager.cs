using TMPro;
using UnityEngine;

public class EndGameManager : MonoBehaviour
{
    public static EndGameManager instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject talkPanel;
    [SerializeField] private TextMeshProUGUI talkerNameText;
    [SerializeField] private TextMeshProUGUI contentText;
    [HideInInspector] public bool isDialogueBoxOpened = false;
    [SerializeField] private GameObject nextText;
    [SerializeField] private GameObject nextButton;
    [Space]
    [SerializeField] private GameObject coralSecretary;
    [SerializeField] private GameObject kelpSecretary;
    [SerializeField] private GameObject goodCoralBG;
    [SerializeField] private GameObject evilCoralBG;
    [SerializeField] private GameObject goodKelpBG;
    [SerializeField] private GameObject evilKelpBG;
    [SerializeField] private GameObject gameBG1;
    [SerializeField] private GameObject gameBG2;
    [SerializeField] private GameObject pollution;

    [Header("Dialogue")]
    [SerializeField] private DialogueNode badEndingNode;
    [SerializeField] private DialogueNode neutralEndingNode;
    [SerializeField] private DialogueNode goodEndingNode;

    private DialogueNode currentDialogueNode;
    private int activeTalkIndex = 0;

    private bool isEvil = false;

    private TypingEffect typingEffect;
    private void Awake()
    {
        if (instance == null) instance = this;

        typingEffect = contentText.GetComponent<TypingEffect>();

        typingEffect.OnTypingComplete = () => { if (nextText != null) nextText.SetActive(true); };

        dialogueBox.SetActive(false);

        AudioManager.instance.PlayBGM(AudioManager.instance.endGameBGM);

        DecideEnding();
    }
    public void DecideEnding()
    {
        int ending = DayManager.Instance.ending;
        if (ending == 0)
        {
            Debug.LogError("Error occurred with calculation");
            return;
        }

        DialogueNode endGameNode = null;
        switch (ending)
        {
            case 1:
                endGameNode = goodEndingNode;
                break;
            case 2:
                endGameNode = neutralEndingNode;
                break;
            case 3:
                endGameNode = badEndingNode;
                isEvil = true;
                break;
        }

        StartEndGameDialogue(endGameNode);
    }

    public void StartEndGameDialogue(DialogueNode startNode)
    {
        if (startNode == null) return;

        currentDialogueNode = startNode;
        activeTalkIndex = 0;
        dialogueBox.SetActive(true);

        isDialogueBoxOpened = true;

        ProgressDialogue();
    }
    public void OnClickNext()
    {
        if (ResourceManager.instance.isGameOver) return;

        if (typingEffect.isTyping)
        {
            typingEffect.SkipTyping();
            return;
        }
        ProgressDialogue();
    }
    private void ProgressDialogue()
    {
        if (activeTalkIndex < currentDialogueNode.sequentialTalks.Length)
        {
            talkPanel.SetActive(true);
            nextButton.SetActive(true);
            nextText.SetActive(false);

            DialogueNode.Talk currentTalk = currentDialogueNode.sequentialTalks[activeTalkIndex];
            talkerNameText.text = currentTalk.talkerName;

            if (talkerNameText.text == "Carol")
            {
                coralSecretary.SetActive(true);
                if (isEvil)
                {
                    pollution.gameObject.SetActive(true);
                    evilCoralBG.SetActive(true);
                }
                else goodCoralBG.SetActive(true);
                gameBG1.SetActive(true);

                kelpSecretary.SetActive(false);
                goodKelpBG.SetActive(false);
                evilKelpBG.SetActive(false);
                gameBG2.SetActive(false);
            }
            else
            {
                coralSecretary.SetActive(false);
                goodCoralBG.SetActive(false);
                evilCoralBG.SetActive(false);
                gameBG1.SetActive(false);

                kelpSecretary.SetActive(true);
                if (isEvil)
                {
                    pollution.SetActive(true);
                    evilKelpBG.SetActive(true);
                }
                else goodKelpBG.SetActive(true);
                gameBG2.SetActive(true);
            }

            typingEffect.StartTyping(currentTalk.content);

            activeTalkIndex += 1;
        }
        else
        {
            nextText.SetActive(false);
            EndDialogue();
        }
    }
    private void EndDialogue()
    {
        dialogueBox.SetActive(false);

        coralSecretary.SetActive(false);
        kelpSecretary.SetActive(false);

        isDialogueBoxOpened = false;
        currentDialogueNode = null;
        activeTalkIndex = 0;
    }
}

