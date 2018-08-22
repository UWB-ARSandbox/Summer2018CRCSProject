using UnityEngine;
using ASL.PortalSystem;

namespace ASL.PlayerSystem
{

    /// <summary>
    /// This script enables the user to control an avatar with a simple set of movement commands.
    /// PlayerController allows the user to toggle between interacting with gravity and encountering
    /// collisions with colliders.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        #region Public Fields
        /// <summary>
        /// Speed the player avatar moves
        /// </summary>
        public float Speed = 5f;
        /// <summary>
        /// Downward velocity applied to the player when not grounded.
        /// </summary>
        public float Gravity = -9.81f;
        /// <summary>
        /// Button to press to toggle between clipping through objects.
        /// </summary>
        public KeyCode ToggleClippingButton = KeyCode.C;
        /// <summary>
        /// Button to press to toggle gravity.
        /// </summary>
        public KeyCode ToggleGravityButton = KeyCode.G;
        #endregion Public Fields

        #region Private Fields
        private bool transEnabled;
        private bool gravityEnabled;
        private bool clippingEnabled;
        private bool previousGState;
        private Vector3 _velocity;
        private Collider _collider;
        private CharacterController _controller;
        private Rigidbody _rigid;
        private GameObject car;
        private float distToGround;
        #endregion Private Fields

        /*
         * Getting initial references required for controlling the player avatar.
         */
        void Start()
        {
            transEnabled = true;
            gravityEnabled = false;
            clippingEnabled = true;
            _velocity = new Vector3(0, Gravity, 0);
            _rigid = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _controller = GetComponent<CharacterController>();
            previousGState = gravityEnabled;
            distToGround = _collider.bounds.extents.y;
        }

        /* 
         * Checks for user input to update state of the playercontroller. Takes updated state and calls
         *  appropriate method for controlling movement of the player.
        */
        void Update()
        {
            // Clipping refers to when collisions occur and prevent movement through colliders. Toggling off
            // clipping allows for the user to move through any object and prevents the effects of gravity.
            if (Input.GetKeyDown(ToggleClippingButton))
            {
                toggleClipping();
            }

            if (Input.GetKeyDown(ToggleGravityButton))
            {
                toggleGravity();
            }
        }

        /// <summary>
        /// Update movement once every frame.
        /// </summary>
        private void LateUpdate()
        {

            // Collider must be enabled to allow for character controller movement to be used.
            updateMovment();

        }

        /* 
         * This function allows the player to travel through colliders. Uses the physics system to ignore interaction between layers.
        */
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

        /*
         * Toggles gravity & saves previous state.
         */
        private void toggleGravity()
        {
            previousGState = gravityEnabled;
            gravityEnabled = !gravityEnabled;
        }

        /*
         * This function reads from the axis input and key inputs to move the player in response to user input.
         */
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

            if (transEnabled)
            {
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
            }
            _controller.Move((forward * Time.deltaTime * Speed) + (strafe * Time.deltaTime * Speed));
        }

        /// <summary>
        /// The setTransEnabled method assigns the given value 
        /// to the transEnabled field.If the value is false then
        /// the player will not be translated with user input from
        /// keyboard keys.
        /// </summary>
        /// <param name="enabled">The new value assigned to the transEnabled field.</param>
        public void setTransEnabled(bool enabled)
        {
            transEnabled = enabled;
        }
    }
}