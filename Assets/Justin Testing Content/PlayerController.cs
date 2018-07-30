﻿using UnityEngine;
using ASL.PortalSystem;

// This script enables the user to control an avatar with a simple set of movement commands.
// PlayerController allows the user to toggle between interacting with gravity and encountering
// collisions with colliders.
public class PlayerController : MonoBehaviour
{
    #region Public Fields
    public float Speed = 5f;

    public float Gravity = -9.81f;
    public KeyCode ToggleClippingButton = KeyCode.C;
    public KeyCode toggleGravityButton = KeyCode.G;
    #endregion Public Fields

    #region Private Fields
    private bool gravityEnabled = false;
    private bool clippingEnabled = true;
    private bool previousGState;
    private Vector3 _velocity;
    private Collider _collider;
    private CharacterController _controller;
    private Rigidbody _rigid;
    private float distToGround;
    #endregion Private Fields

    // Start
    // Most important parts of initilization are getting references to the CharacterController and
    // Collider. These enable movement and toggling of clipping.
    void Start()
    {
        _velocity = new Vector3(0, Gravity, 0);
        _rigid = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _controller = GetComponent<CharacterController>();
        previousGState = gravityEnabled;
        distToGround = _collider.bounds.extents.y;

        GameObject.Find("PortalManager").GetComponent<PortalManager>().player = this.gameObject;
    }

    // Update
    // Checks for user input to update state of the playercontroller. Takes updated state and calls
    // appropriate method for controlling movement of the player.
    void Update()
    {   
        // Clipping refers to when collisions occur and prevent movement through colliders. Toggling off
        // clipping allows for the user to move through any object and prevents the effects of gravity.
        if (Input.GetKeyDown(ToggleClippingButton))
        {
            toggleClipping();
        }

        if (Input.GetKeyDown(toggleGravityButton))
        {
            toggleGravity();
        }       
    }

    private void LateUpdate()
    {

        // Collider must be enabled to allow for character controller movement to be used.
        updateMovment();

    }

    private void toggleClipping()
    {
        clippingEnabled = !clippingEnabled;

        if (clippingEnabled)
        {
            gameObject.layer = LayerMask.NameToLayer("Clipping Enabled");

        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Clipping Disabled");

        }
    }


    private void toggleGravity()
    {
        previousGState = gravityEnabled;
        gravityEnabled = !gravityEnabled;
    }

    // RigidBodyMovement
    // This movement update method takes user input and applies forces through the CharacterController component to
    // move the player.
    private void updateMovment()
    {
        Vector3 forward = transform.forward;
        Vector3 strafe = transform.right;

        forward = Vector3.Normalize(forward);
        strafe = Vector3.Normalize(strafe);
        
        if (gravityEnabled)
        {
            if (!_controller.isGrounded)
            {
                _controller.Move(_velocity * Time.deltaTime);
            }
        }

        if (Input.GetAxis("Vertical") < 0)
        {
            forward = -1 * forward;
        }
        if (Input.GetAxis("Vertical") == 0)
        {
            forward *= 0;
        }

        if (Input.GetAxis("Horizontal") < 0)
        {
            strafe *= -1;
        }
        if (Input.GetAxis("Horizontal") == 0)
        {
            strafe *= 0;
        }

        _controller.Move((forward * Time.deltaTime * Speed) + (strafe * Time.deltaTime * Speed));

    }
}