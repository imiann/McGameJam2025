using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue _dialogue;

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(_dialogue);
    }
}
