using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    public GameObject player;
    public Transform playerCamera; // Reference to the player's camera
    public Transform holdPos;
    public float throwForce = 500f; // Force at which the object is thrown
    public float pickUpRange = 5f; // How far the player can pick up the object from
    public TMP_Text interactionPrompt; // Reference to the UI Text element for the interaction prompt

    private float rotationSensitivity = 1f; // How fast/slow the object is rotated in relation to mouse movement
    private GameObject heldObj; // Object which we pick up
    private Rigidbody heldObjRb; // Rigidbody of object we pick up
    private bool canDrop = true; // This is needed so we don't throw/drop object when rotating the object
    private int LayerNumber; // Layer index
    private GameObject lastHitObject; // To store the last hit object

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("HoldLayer"); // If your holdLayer is named differently make sure to change this ""
    }

    void Update()
    {
        if (heldObj == null) // If currently not holding anything
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, pickUpRange))
            {
                if (hit.transform.CompareTag("CanPickUp"))
                {
                    if (lastHitObject != hit.transform.gameObject)
                    {
                        interactionPrompt.text = "Press E to pick up";
                        interactionPrompt.gameObject.SetActive(true);
                        lastHitObject = hit.transform.gameObject;
                    }
                }
                else
                {
                    if (lastHitObject != null)
                    {
                        interactionPrompt.gameObject.SetActive(false);
                        lastHitObject = null;
                    }
                }
            }
            else
            {
                if (lastHitObject != null)
                {
                    interactionPrompt.gameObject.SetActive(false);
                    lastHitObject = null;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E)) // Change E to whichever key you want to press to pick up
        {
            if (heldObj == null) // If currently not holding anything
            {
                // Perform raycast to check if player is looking at object within pickUpRange
                RaycastHit hit;
                if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, pickUpRange))
                {
                    // Make sure pickup tag is attached
                    if (hit.transform.CompareTag("CanPickUp"))
                    {
                        // Pass in object hit into the PickUpObject function
                        PickUpObject(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                if (canDrop == true)
                {
                    StopClipping(); // Prevents object from clipping through walls
                    DropObject();
                }
            }
        }

        if (heldObj != null) // If player is holding object
        {
            MoveObject(); // Keep object position at holdPos
            RotateObject();

            if (Input.GetKeyDown(KeyCode.Mouse0) && canDrop) // Mouse0 (left click) is used to throw, change this if you want another button to be used
            {
                StopClipping();
                ThrowObject();
            }
        }
    }

    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>()) // Make sure the object has a Rigidbody
        {
            heldObj = pickUpObj; // Assign heldObj to the object that was hit by the raycast (no longer == null)
            heldObjRb = pickUpObj.GetComponent<Rigidbody>(); // Assign Rigidbody
            heldObjRb.isKinematic = true;
            heldObjRb.transform.position = holdPos.position; // Position the object at the hold position
            heldObjRb.transform.parent = holdPos; // Parent object to hold position
            heldObj.layer = LayerNumber; // Change the object layer to the holdLayer
            // Make sure object doesn't collide with player, it can cause weird bugs
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }

    void DropObject()
    {
        // Re-enable collision with player
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0; // Object assigned back to default layer
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null; // Unparent object
        heldObj = null; // Undefine game object
    }

    void MoveObject()
    {
        // Keep object position the same as the holdPosition position
        heldObj.transform.position = holdPos.position;
    }

    void RotateObject()
    {
        if (Input.GetKey(KeyCode.R)) // Hold R key to rotate, change this to whatever key you want
        {
            canDrop = false; // Make sure throwing can't occur during rotating

            float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
            float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;
            // Rotate the object depending on mouse X-Y Axis
            heldObj.transform.Rotate(Vector3.down, XaxisRotation);
            heldObj.transform.Rotate(Vector3.right, YaxisRotation);
        }
        else
        {
            canDrop = true;
        }
    }

    void ThrowObject()
    {
        // Same as drop function, but add force to object before undefining it
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(playerCamera.forward * throwForce);
        heldObj = null;
    }

    void StopClipping() // Function only called when dropping/throwing
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, playerCamera.position); // Distance from holdPos to the camera
        // Have to use RaycastAll as object blocks raycast in center screen
        // RaycastAll returns array of all colliders hit within the clip range
        RaycastHit[] hits;
        hits = Physics.RaycastAll(playerCamera.position, playerCamera.forward, clipRange);
        // If the array length is greater than 1, meaning it has hit more than just the object we are carrying
        if (hits.Length > 1)
        {
            // Change object position to camera position 
            heldObj.transform.position = playerCamera.position + new Vector3(0f, -0.5f, 0f); // Offset slightly downward to stop object dropping above player 
            // If your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
        }
    }
}