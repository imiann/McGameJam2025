using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text Name;
    public TMP_Text DialogueText;
    public Animator animator;
    public CinemachineCamera cinemachineCamera; // Reference to the Cinemachine virtual camera

    private Queue<string> _sentences;
    public bool isInConversation { get; private set; } = false; // Make isInConversation public with a private setter

    void Start()
    {
        _sentences = new Queue<string>();
    }

    void Update()
    {
        if (isInConversation && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)))
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("isOpen", true);
        Name.text = dialogue.name;

        // Cursor unlock
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            _sentences.Enqueue(sentence);
        }

        isInConversation = true;
        cinemachineCamera.enabled = false; // Disable the Cinemachine camera
        StartCoroutine(WaitForAnimation());
    }

    private IEnumerator WaitForAnimation()
    {
        // Wait until the animator's "isOpen" parameter is true
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("DialogueBoxOpen"));

        // Wait until the animation is no longer playing
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = _sentences.Dequeue();

        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        DialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            DialogueText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
    }

    void EndDialogue()
    {
        Debug.Log("End of conversation");
        animator.SetBool("isOpen", false);

        StartCoroutine(WaitForAnimation());

        // Set cursor to invisible
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        isInConversation = false;
        cinemachineCamera.enabled = true; // Re-enable the Cinemachine camera

        // Set text to have nothing 
        DialogueText.text = "";
    }
}