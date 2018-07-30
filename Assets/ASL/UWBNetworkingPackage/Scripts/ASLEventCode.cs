using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// Encapsulates all event codes for customization of PUN events that 
    /// allows ASL-personalized cross-network communication and data 
    /// transfer.
    /// </summary>
    public static class ASLEventCode
    {
        #region Fields
        /// <summary>
        /// Event code used for network instantiation of a given object / prefab.
        /// </summary>
        public const byte EV_INSTANTIATE = 99;

        /// <summary>
        /// Event code used for network destruction of a given object / prefab.
        /// </summary>
        public const byte EV_DESTROYOBJECT = 98;

        /// <summary>
        /// Event code used for synchronization of a client's scene with
        /// other clients' scenes.
        /// </summary>
        public const byte EV_SYNCSCENE = 97;

        /// <summary>
        /// Event code used for proper joining of a receiving client to an
        /// existing ASL room.
        /// </summary>
        public const byte EV_JOIN = 55;

        /// <summary>
        /// Event code used for managing object ownership.
        /// </summary>
        public const byte EV_SYNC_OBJECT_OWNERSHIP_RESTRICTION = 96;

        #region WorldManager Events
        /// <summary>
        /// Event code used for adding a world to the World Manager for the
        /// Portal System.
        /// </summary>
        public const byte EV_WORLD_ADD = 30;    //add a world to the worldManager

        /// <summary>
        /// Event code used for adding an entity to one of the worlds managed
        /// by the World Manager.
        /// </summary>
        public const byte EV_WORLD_ADD_TO = 31;    //add an entity to one of the worlds
        #endregion

        #region PortalManager Events
        /// <summary>
        /// Event code used for synchronizing portals across all clients.
        /// </summary>
        public const byte EV_PORTAL_SYNC = 39;

        /// <summary>
        /// Event code used for registering a portal in the World Manager system.
        /// </summary>
        public const byte EV_PORTAL_REG = 40;

        /// <summary>
        /// Event code used for unregistering a portal in the World Manager 
        /// system.
        /// </summary>
        public const byte EV_PORTAL_UNREG = 41;

        /// <summary>
        /// Event code for linking two portals in the World Manager together.
        /// </summary>
        public const byte EV_PORTAL_LINK = 42;

        /// <summary>
        /// Event code for unlinking two portals in the World Manager.
        /// </summary>
        public const byte EV_PORTAL_UNLINK = 43;
        #endregion

        #region PortalManager Test World Events
        /// <summary>
        /// Event code for knowing when the master just created a world that
        /// must be loaded.
        /// </summary>
        public const byte EV_MASTER_LOAD = 44;  //master just created worlds

        /// <summary>
        /// Event code for making an avatar.
        /// </summary>
        public const byte EV_AVATAR_MAKE = 45;  //avatar created by a user
        #endregion
        #endregion
    }
}