using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASL.Manipulation.Objects;
using ASL.LocalEventSystem;

// Script used when making the simulator camera rig a networked object, have transitioned to separting the VR SDK scripts and the
// networked repressentation of a VR player.
public class SimulatorInitializationScript : LocalEventHandler {

    public Vector3 origin;
    public GameObject simulator;

    protected override void OnLocalEvent(object sender, ASLLocalEventManager.LocalEventArgs args)
    {
        switch (args.MyEvent)
        {
            default:
                {
                    break;
                }
        }
    }

    public override void OnJoinedRoom()
    {
        InstantiateSimulatorCameraRig();
    }

    private void InstantiateSimulatorCameraRig()
    {
        GameObject.Find("ObjectInteractionManager").GetComponent<ObjectInteractionManager>().InstantiateOwnedObject(simulator.name);

    }

    void OnSimulatorInstantiated()
    {
        Debug.Log("Looking for the Simulator");
        GameObject simulator = transform.Find("Simulator").gameObject;
        Debug.Log("Simulator found");
        simulator.GetComponent<VRTK.VRTK_SDKSetup>().enabled = true;
        Debug.Log("Enabled SDK Setup for the simulator");

        //GameObject cameraRig = simulator.transform.childCount

        //transform.Find("Simulator").gameObject.GetComponentInChildren<VRTK.SDK_InputSimulator>().enabled = true;

        VRTK.SDK_InputSimulator[] inputSimulators = Resources.FindObjectsOfTypeAll<VRTK.SDK_InputSimulator>();

        Debug.Log("Trying to enable the input simulator");
        foreach (VRTK.SDK_InputSimulator input in inputSimulators)
        {
            Debug.Log("Searching through found input input simulators");
            if (input.transform.name.Equals("[VRSimulator_ASLAvatar]"))
            {
                Debug.Log("Trying to enable the input simulator in " + input.gameObject.name + "which is currently active: " + input.enabled);
                input.enabled = true;
            }
        }
        gameObject.GetComponent<VRTK.VRTK_SDKManager>().enabled = true;
    }
}
