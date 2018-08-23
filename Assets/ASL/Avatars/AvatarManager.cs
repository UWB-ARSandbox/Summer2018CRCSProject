using UnityEngine;
using Photon;

namespace ASL.PlayerSystem
{
    /// <summary>
    /// This class manages the instances of Player Avatars and ensures that they are deleted upon
    /// the owner leaving the room.
    /// </summary>
    /// Note: Implemented by Thomas, not very robust.
    public class AvatarManager : PunBehaviour
    {
        /// <summary>
        /// Functions as networked alternative to the OnDestroy function.
        /// </summary>
        public void OnApplicationQuit()
        {
            string objName = "Player Avatar";
            int viewID = 2001;
            GameObject.FindObjectOfType<UWBNetworkingPackage.ObjectManager>().DestroyHandler(objName, viewID);

        }

        /// <summary>
        /// Handler function that will delete the avatar of another player when they disconnect.
        /// </summary>
        /// <param name="player"></param>
        override public void OnPhotonPlayerDisconnected(PhotonPlayer player)
        {
            base.OnPhotonPlayerDisconnected(player);

            int id = player.ID;
            DeleteVestigialAvatar(id);
        }

        /// <summary>
        /// Searches the scene for non active players, who have discconnected, and destory
        /// the gameobjects associated with them.
        /// </summary>
        /// <param name="id"></param>
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
}