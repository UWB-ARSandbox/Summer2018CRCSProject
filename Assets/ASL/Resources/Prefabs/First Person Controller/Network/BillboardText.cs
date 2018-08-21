using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL
{
    /// <summary>
    /// Script used to have a world space text identifying an avatar, facing the local avatar.
    /// </summary>
    /// Future Development: Use local event system, fix the reference setting for different player types.
    public class BillboardText : Photon.PunBehaviour
    {

        public Camera myCamera;
        public bool photonViewNotNull = false;

        private PhotonView myPhotonView;

        void Awake()
        {
            myPhotonView = transform.GetComponent<PhotonView>();
            Debug.Log("Creating billboard text");
            if (myCamera == null)
            {
                myCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            }


            if (PhotonNetwork.inRoom)
            {
                Debug.Log("Created Billboard text");
            }
        }


        public override void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            Debug.Log("NAMETAG INSTANTIATED");
        }

        public void setCamera(Camera c)
        {
            myCamera = c;

        }

        void Update()
        {
            if (myPhotonView == null)
            {
                myPhotonView = transform.GetComponent<PhotonView>();
                photonViewNotNull = true;
                Debug.Log("Photon view is avaliable & it belongs to me: " + myPhotonView.isMine);
            }
            if (!photonViewNotNull)
            {
                Debug.Log("Photon View has not been attached yet");
            }
            if (myCamera != null)
            {
                transform.LookAt(transform.position + myCamera.transform.rotation * Vector3.forward, myCamera.transform.rotation * Vector3.up);
            }
            else
            {
                Debug.Log("Looking for Camera");
                myCamera = GameObject.FindGameObjectWithTag("Local Primary Camera").GetComponent<Camera>();
            }
        }
    }
}