using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LocalEventHandler : Photon.PunBehaviour
{

    protected virtual void OnEnable()
    {
        ASLLocalEventManager.LocalEventTriggered += OnLocalEvent;
    }

    protected virtual void OnDestroy()
    {
        ASLLocalEventManager.LocalEventTriggered += OnLocalEvent;
    }

    public override void OnPhotonInstantiate(PhotonMessageInfo info) { }
    protected abstract void OnLocalEvent(object sender, ASLLocalEventManager.LocalEventArgs args);

}
