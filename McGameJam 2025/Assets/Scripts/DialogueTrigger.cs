using UnityEngine;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue _dialogue;
    public float interactionRange = 5f; // Range within which the player can interact with NPCs
    public Transform playerCamera; // Reference to the player's camera
    public TMP_Text interactionPrompt; // Reference to the UI Text element for the interaction prompt

    private DialogueManager dialogueManager;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    void Update()
    {
        if (dialogueManager.isInConversation)
        {
            interactionPrompt.gameObject.SetActive(false);
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, interactionRange))
        {
            if (hit.transform.CompareTag("NPC"))
            {
                interactionPrompt.text = "Press E to start dialogue";
                interactionPrompt.gameObject.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E)) // Change E to whichever key you want to press to interact
                {
                    TriggerDialogue();
                }
            }
            else
            {
                interactionPrompt.gameObject.SetActive(false);
            }
        }
        else
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }

    public void TriggerDialogue()
    {
        dialogueManager.StartDialogue(_dialogue);
    }
}