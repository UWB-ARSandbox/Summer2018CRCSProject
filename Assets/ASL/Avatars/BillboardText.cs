using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.LocalEventSystem;
using System;

namespace ASL
{
    namespace PlayerSystem
    {
        /// <summary>
        /// Script used to have a world space text identifying an avatar, facing the local avatar.
        /// </summary>
        /// <description>Future Development: Use local event system, fix the reference setting for different player types.</description>
        public class BillboardText : LocalEventHandler
        {

            private Camera primaryPlayerCamera;

            /// <summary>
            /// Initilize the reference the networked components.
            /// </summary>
            /// <param name="info"></param>
            public override void OnPhotonInstantiate(PhotonMessageInfo info)
            {
                FindPrimaryLocalCamera();
                if (transform.GetComponent<PhotonView>().isMine)
                {
                    this.enabled = false;
                    // May required disabling the canvas object to disable the owner from seeing their own billboard text.
                }
            }

            /// <summary>
            /// Event handler for local events relating to the synchronization of the world space
            /// canvas text.
            /// </summary>
            /// <param name="sender">Object that triggered the event</param>
            /// <param name="args">The event being triggered</param>
            /// <event>PrimaryCameraSet</event>
            /// <description>This event is handled by attempting finding the Local Primary Camera.</description>
            protected override void OnLocalEvent(object sender, ASLLocalEventManager.LocalEventArgs args)
            {
                switch (args.MyEvent)
                {
                    case ASLLocalEventManager.LocalEvents.PrimaryCameraSet:
                        {
                            FindPrimaryLocalCamera();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            /// <summary>
            /// Upon joining a room find the primary camera.
            /// </summary>
            public override void OnJoinedRoom()
            {
                FindPrimaryLocalCamera();
            }

            private void FindPrimaryLocalCamera()
            {
                primaryPlayerCamera = GameObject.FindGameObjectWithTag("Local Primary Camera").GetComponent<Camera>();
            }

            /// <summary>
            /// Makes this instance of the billboard text face toward the local player.
            /// </summary>
            /// <description>
            /// Issue: Not currently tested, may be issues between different player types/camera rigs.</description>
            void Update()
            {
                if (primaryPlayerCamera != null)
                {
                    transform.LookAt(transform.position + primaryPlayerCamera.transform.rotation * Vector3.forward, primaryPlayerCamera.transform.rotation * Vector3.up);
                }
            }
        }
    }
}