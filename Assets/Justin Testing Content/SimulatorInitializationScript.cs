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
        GameObject.Find("VRTK_SDKManager").GetComponent<VRTK.VRTK_SDKManager>().enabled = true;
    }
}
