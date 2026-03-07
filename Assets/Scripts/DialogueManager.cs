using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField]
    private string targetTag = "Player";

    [SerializeField]
    private float typingDelay = 0.03f;

    [SerializeField]
    private TMP_Text dialogueSpeaker;

    [SerializeField]
    private TMP_Text dialogueText;

    [SerializeField]
    private TMP_Text dialogueHint;

    [SerializeField]
    private string defaultHint;

    [SerializeField]
    private float defaultAdvanceDelay;

    private CharacterMovement characterMovement;
    private DialogueLine[] currentLines;
    private int currentLineIndex;
    private bool isTyping;
    private bool isDialogueActive;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        GameObject player = GameObject.FindWithTag(targetTag);
        characterMovement = player.GetComponent<CharacterMovement>();

        if (characterMovement == null)
        {
            Debug.LogError(
                "Could not attach to the character movement component. No dialogues will appear."
            );
            Destroy(gameObject);
            return;
        }

        gameObject.SetActive(false);
    }

    public void BeginDialogue(DialogueLine[] lines)
    {
        currentLines = lines;
        currentLineIndex = 0;
        isDialogueActive = true;

        gameObject.SetActive(true);
        ShowCurrentLine();
    }

    private void Update()
    {
        if (!isDialogueActive)
            return;

        bool modifierPressed =
            Keyboard.current != null && Keyboard.current.shiftKey.isPressed;
        bool advancePressed = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;

        if (advancePressed)
        {
            // If SHIFT + E are pressed at the same time, skip the entire dialogue
            if (modifierPressed)
            {
                EndDialogue();

                DialogueLine line = currentLines[currentLines.Length - 1];
                characterMovement.moveAllowed = line.moveAllowed;
                characterMovement.jumpAllowed = line.jumpAllowed;
                characterMovement.wallJumpAllowed = line.wallJumpAllowed;
                characterMovement.dashAllowed = line.dashAllowed;

                return;
            }

            if (isTyping)
                FinishTyping();
            else
                AdvanceLine();
        }
    }

    private void ShowCurrentLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(currentLines[currentLineIndex]));
    }

    public IEnumerator TypeLine(DialogueLine line)
    {
        isTyping = true;

        dialogueSpeaker.text = line.speaker;
        dialogueText.text = "";
        dialogueHint.text = "";
        dialogueText.color = line.dialogueColor;

        characterMovement.moveAllowed = line.moveAllowed;
        characterMovement.jumpAllowed = line.jumpAllowed;
        characterMovement.wallJumpAllowed = line.wallJumpAllowed;
        characterMovement.dashAllowed = line.dashAllowed;

        foreach (char c in line.dialogue)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingDelay);
        }

        isTyping = false;

        dialogueHint.text = string.IsNullOrEmpty(line.hint) ? defaultHint : line.hint;

        if (!line.advanceManually)
        {
            yield return new WaitForSeconds(
                line.advanceDelay <= 0f ? defaultAdvanceDelay : line.advanceDelay
            );

            if (currentLineIndex >= currentLines.Length - 1)
                EndDialogue();
            else
                AdvanceLine();
        }
    }

    public void FinishTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        isTyping = false;

        DialogueLine line = currentLines[currentLineIndex];
        dialogueText.text = line.dialogue;
        dialogueHint.text = string.IsNullOrEmpty(line.hint) ? defaultHint : line.hint;

        if (!line.advanceManually)
            typingCoroutine = StartCoroutine(AutoAdvanceAfterDelay(line));
    }

    private IEnumerator AutoAdvanceAfterDelay(DialogueLine line)
    {
        yield return new WaitForSeconds(
            line.advanceDelay <= 0f ? defaultAdvanceDelay : line.advanceDelay
        );
        if (currentLineIndex >= currentLines.Length - 1)
            EndDialogue();
        else
            AdvanceLine();
    }

    private void AdvanceLine()
    {
        currentLineIndex++;

        if (currentLineIndex < currentLines.Length)
            ShowCurrentLine();
        else
            EndDialogue();
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        gameObject.SetActive(false);
    }

    public bool IsDialogueActive() => isDialogueActive;
}
