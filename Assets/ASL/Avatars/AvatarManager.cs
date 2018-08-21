using UnityEngine;

using Photon;
public class AvatarManager : PunBehaviour
{

    public void OnApplicationQuit()
    {
        string objName = "Player Avatar";
        int viewID = 2001;
        GameObject.FindObjectOfType<UWBNetworkingPackage.ObjectManager>().DestroyHandler(objName, viewID);

    }

    override public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        base.OnPhotonPlayerDisconnected(player);

        int id = player.ID;
        DeleteVestigialAvatar(id);
    }

    public void DeleteVestigialAvatar(int id)
    {
        var list = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject go in list)
        {
            if (go.name.Equals("Player Avatar"))
            {
                if (go.GetComponent<PhotonView>() != null)
                {
                    var pv = go.GetComponent<PhotonView>();
                    int creatorID = pv.viewID / 1000; // Get thecreator's ID,not the current owner ID because that can change over time
                    if (creatorID == id)
                    {
                        GameObject.Destroy(go);
                    }
                }
            }
        }
    }
}