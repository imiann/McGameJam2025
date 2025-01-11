using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text Name;
    public TMP_Text DialogueText;

    public Animator animator;

    private Queue<string> _sentences;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        //Debug.Log("Starting conversation with " + dialogue.name);


        animator.SetBool("isOpen", true);
        Name.text = dialogue.name;

        _sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            _sentences.Enqueue(sentence);
        }

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
            yield return new WaitForSeconds(0.05f);
        }
    }

    void EndDialogue()
    {
        Debug.Log("End of conversation");
        animator.SetBool("isOpen", false);
    }


}
