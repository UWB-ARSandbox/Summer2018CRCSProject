using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Manipulation.Objects;
using System;
using ASL.PortalSystem;

public class VRInitilizationScript : LocalEventHandler
{
    // Enum of supported events.
    public enum SupportedDevices
    {
        Simulator,
        SteamVR
    }

    public SupportedDevices DeviceToUse = SupportedDevices.Simulator;
    public Vector3 origin;
    public GameObject simulator;
    public GameObject steamVR;

    private GameObject simulatorCameraRig;

    protected override void OnLocalEvent(object sender, ASLLocalEventManager.LocalEventArgs args)
    {
        switch (args.MyEvent)
        {
            case ASLLocalEventManager.LocalEvents.SimCameraRigCreationSucceeded:
                {
                    SimulatorCameraRigEventHandler();
                    break;
                }
            case ASLLocalEventManager.LocalEvents.SimCameraRigCreationFailed:
                {
                    InstantiateSimulatorCameraRig();
                    break;
                }
            case ASLLocalEventManager.LocalEvents.SimulatorActivated:
                {
                    SimulatorActivatedEventHandler();
                    break;
                }
            case ASLLocalEventManager.LocalEvents.SimCameraRigActivated:
                {
                    InputSimulatorHandler();
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    private void InputSimulatorHandler()
    {
        if (photonView.isMine)
        {
            simulatorCameraRig.GetComponent<VRTK.SDK_InputSimulator>().enabled = true;
        }
        else
        {
            simulatorCameraRig.GetComponent<VRTK.SDK_InputSimulator>().enabled = false;
        }
    }

    private void SimulatorActivatedEventHandler()
    {
        if (DeviceToUse == SupportedDevices.Simulator)
        {
            GameObject myCamera = transform.GetComponentInChildren<Camera>(true).gameObject;
            myCamera.tag = "Local Primary Camera";
            GameObject.Find("PortalManager").GetComponent<PortalManager>().SetPlayer(myCamera);

            simulatorCameraRig.GetComponent<VRTK.SDK_InputSimulator>().enabled = true;

            Debug.Log("IS THE INPUT SIMULATOR ACTIVE?" + simulatorCameraRig.GetComponent<VRTK.SDK_InputSimulator>().enabled);

        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("VR Connected to the room, creating the Simulator camera rig");
        if (DeviceToUse == SupportedDevices.Simulator)
        {
            InstantiateSimulatorCameraRig();
        }
    }

    private void InstantiateSimulatorCameraRig()
    {
        ObjectInteractionManager objectManager = GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>();
        simulatorCameraRig = objectManager.InstantiateOwnedObject("[VRSimulator_ASLAvatar]");
        if (simulatorCameraRig == null)
        {
            ASLLocalEventManager.Instance.Trigger(gameObject, ASLLocalEventManager.LocalEvents.SimCameraRigCreationFailed);
        }
        else
        {
            ASLLocalEventManager.Instance.Trigger(gameObject, ASLLocalEventManager.LocalEvents.SimCameraRigCreationSucceeded);
        }

    }

    private void SimulatorCameraRigEventHandler()
    {

        Debug.Log("VR initilization script is trying to handle the simulate camera rig event");
        if (simulatorCameraRig.GetComponent<PhotonView>().isMine)
        {

            if (DeviceToUse == SupportedDevices.Simulator)
            {
                transform.GetComponent<VRTK.VRTK_SDKManager>().enabled = true;
                simulator.GetComponent<VRTK.VRTK_SDKSetup>().enabled = true;
            }

            Debug.Log("Trying to enable input simulator - current state: " + simulatorCameraRig.GetComponent<VRTK.SDK_InputSimulator>().enabled);
            if (!simulatorCameraRig.GetComponent<VRTK.SDK_InputSimulator>().enabled)
            {
                simulatorCameraRig.GetComponent<VRTK.SDK_InputSimulator>().enabled = true;
                Debug.Log("After trying - current state: " + simulatorCameraRig.GetComponent<VRTK.SDK_InputSimulator>().enabled);
            }
            simulatorCameraRig.transform.Find("Canvas").gameObject.SetActive(true);

            simulatorCameraRig.transform.parent = simulator.transform;
        }
    }
}


