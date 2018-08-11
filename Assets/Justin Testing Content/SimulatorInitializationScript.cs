using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatorInitializationScript : LocalEventHandler {

    protected override void OnLocalEvent(object sender, ASLLocalEventManager.LocalEventArgs args)
    {
        switch (args.MyEvent)
        {
            case ASLLocalEventManager.LocalEvents.PlayerInitialized:
                {
                    break;
                }
            case ASLLocalEventManager.LocalEvents.SimulatorCameraRigInstantiated:
                {
                    OnSimulatorInstantiated();
                    break;
                }
            default:
                {
                    break;
                }
        }
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

        foreach (VRTK.SDK_InputSimulator input in inputSimulators)
        {
            if (input.transform.name.Equals("[VRSimulator_ASLAvatar]"))
            {
                input.enabled = true;
                break;
            }
        }
        gameObject.GetComponent<VRTK.VRTK_SDKManager>().enabled = true;
    }
}
