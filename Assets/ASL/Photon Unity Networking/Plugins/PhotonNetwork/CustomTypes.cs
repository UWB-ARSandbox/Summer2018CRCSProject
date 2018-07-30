// ----------------------------------------------------------------------------
// <copyright file="CustomTypes.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

#pragma warning disable 1587
/// \file
/// <summary>Sets up support for Unity-specific types. Can be a blueprint how to register your own Custom Types for sending.</summary>
#pragma warning restore 1587


using ExitGames.Client.Photon;
using UnityEngine;

//Data struct used for properties across the network
[System.Serializable]
public struct AvatarInfo
{
    [SerializeField] public int playerID;
    [SerializeField] public int viewID;
    [SerializeField] public Vector3 spawnPosition;
    [SerializeField] public Color color;

    public AvatarInfo(int playerID, int viewID, Vector3 spawnPosition, Color color)
    {
        this.playerID = playerID;
        this.viewID = viewID;
        this.spawnPosition = spawnPosition;
        this.color = color;
    }
}

/// <summary>
/// Internally used class, containing de/serialization methods for various Unity-specific classes.
/// Adding those to the Photon serialization protocol allows you to send them in events, etc.
/// </summary>
internal static class CustomTypes
{
    /// <summary>Register</summary>
    internal static void Register()
    {
        PhotonPeer.RegisterType(typeof(Vector2), (byte)'W', SerializeVector2, DeserializeVector2);
        PhotonPeer.RegisterType(typeof(Vector3), (byte)'V', SerializeVector3, DeserializeVector3);
        PhotonPeer.RegisterType(typeof(Quaternion), (byte)'Q', SerializeQuaternion, DeserializeQuaternion);
        PhotonPeer.RegisterType(typeof(PhotonPlayer), (byte)'P', SerializePhotonPlayer, DeserializePhotonPlayer);
        PhotonPeer.RegisterType(typeof(Color), (byte)'C', SerializeColor, DeserializeColor);
        PhotonPeer.RegisterType(typeof(AvatarInfo), (byte)'A', SerializeAvatarInfo, DeserializeAvatarInfo);
    }


    #region Custom De/Serializer Methods


    public static readonly byte[] memVector3 = new byte[3 * 4];
    private static short SerializeVector3(StreamBuffer outStream, object customobject)
    {
        Vector3 vo = (Vector3)customobject;

        int index = 0;
        lock (memVector3)
        {
            byte[] bytes = memVector3;
            Protocol.Serialize(vo.x, bytes, ref index);
            Protocol.Serialize(vo.y, bytes, ref index);
            Protocol.Serialize(vo.z, bytes, ref index);
            outStream.Write(bytes, 0, 3 * 4);
        }

        return 3 * 4;
    }

    private static object DeserializeVector3(StreamBuffer inStream, short length)
    {
        Vector3 vo = new Vector3();
        lock (memVector3)
        {
            inStream.Read(memVector3, 0, 3 * 4);
            int index = 0;
            Protocol.Deserialize(out vo.x, memVector3, ref index);
            Protocol.Deserialize(out vo.y, memVector3, ref index);
            Protocol.Deserialize(out vo.z, memVector3, ref index);
        }

        return vo;
    }


    public static readonly byte[] memVector2 = new byte[2 * 4];
    private static short SerializeVector2(StreamBuffer outStream, object customobject)
    {
        Vector2 vo = (Vector2)customobject;
        lock (memVector2)
        {
            byte[] bytes = memVector2;
            int index = 0;
            Protocol.Serialize(vo.x, bytes, ref index);
            Protocol.Serialize(vo.y, bytes, ref index);
            outStream.Write(bytes, 0, 2 * 4);
        }

        return 2 * 4;
    }

    private static object DeserializeVector2(StreamBuffer inStream, short length)
    {
        Vector2 vo = new Vector2();
        lock (memVector2)
        {
            inStream.Read(memVector2, 0, 2 * 4);
            int index = 0;
            Protocol.Deserialize(out vo.x, memVector2, ref index);
            Protocol.Deserialize(out vo.y, memVector2, ref index);
        }

        return vo;
    }


    public static readonly byte[] memQuarternion = new byte[4 * 4];
    private static short SerializeQuaternion(StreamBuffer outStream, object customobject)
    {
        Quaternion o = (Quaternion)customobject;

        lock (memQuarternion)
        {
            byte[] bytes = memQuarternion;
            int index = 0;
            Protocol.Serialize(o.w, bytes, ref index);
            Protocol.Serialize(o.x, bytes, ref index);
            Protocol.Serialize(o.y, bytes, ref index);
            Protocol.Serialize(o.z, bytes, ref index);
            outStream.Write(bytes, 0, 4 * 4);
        }

        return 4 * 4;
    }

    private static object DeserializeQuaternion(StreamBuffer inStream, short length)
    {
        Quaternion o = new Quaternion();

        lock (memQuarternion)
        {
            inStream.Read(memQuarternion, 0, 4 * 4);
            int index = 0;
            Protocol.Deserialize(out o.w, memQuarternion, ref index);
            Protocol.Deserialize(out o.x, memQuarternion, ref index);
            Protocol.Deserialize(out o.y, memQuarternion, ref index);
            Protocol.Deserialize(out o.z, memQuarternion, ref index);
        }

        return o;
    }

    public static readonly byte[] memPlayer = new byte[4];
    private static short SerializePhotonPlayer(StreamBuffer outStream, object customobject)
    {
        int ID = ((PhotonPlayer)customobject).ID;

        lock (memPlayer)
        {
            byte[] bytes = memPlayer;
            int off = 0;
            Protocol.Serialize(ID, bytes, ref off);
            outStream.Write(bytes, 0, 4);
            return 4;
        }
    }

    private static object DeserializePhotonPlayer(StreamBuffer inStream, short length)
    {
        int ID;
        lock (memPlayer)
        {
            inStream.Read(memPlayer, 0, length);
            int off = 0;
            Protocol.Deserialize(out ID, memPlayer, ref off);
        }

        if (PhotonNetwork.networkingPeer.mActors.ContainsKey(ID))
        {
            return PhotonNetwork.networkingPeer.mActors[ID];
        }
        else
        {
            return null;
        }
    }

    public static readonly byte[] memColor = new byte[4 * sizeof(float)]; //4 floats (r, g, b, a)

    private static short SerializeColor(StreamBuffer outStream, object customObject)
    {
        Color color = (Color)customObject;
       
        lock(memColor)
        {
            byte[] bytes = memColor;
            int index = 0;
            Protocol.Serialize(color.r, bytes, ref index);
            Protocol.Serialize(color.g, bytes, ref index);
            Protocol.Serialize(color.b, bytes, ref index);
            Protocol.Serialize(color.a, bytes, ref index);
            outStream.Write(bytes, 0, 4 * 4);
        }

        return 4 * 4;
    }

    private static object DeserializeColor(StreamBuffer inStream, short length)
    {
        Color color = new Color();
        lock(memColor)
        {
            inStream.Read(memColor, 0, 4 * 4);
            int index = 0;
            Protocol.Deserialize(out color.r, memColor, ref index);
            Protocol.Deserialize(out color.g, memColor, ref index);
            Protocol.Deserialize(out color.b, memColor, ref index);
            Protocol.Deserialize(out color.a, memColor, ref index);
        }
        return color;
    }

    public static readonly byte[] memAvatarInfo = new byte[4 +          //int playerID
                                                           4 +          //int viewID
                                                           4 * 4 +      //Color color
                                                           3 * 4];      //Vector3 spawnPosition
    private static short SerializeAvatarInfo(StreamBuffer outStream, object customObject)
    {
        AvatarInfo avatarInfo = (AvatarInfo)customObject;

        int index = 0;
        lock(memAvatarInfo)
        {
            byte[] bytes = memAvatarInfo;

            Protocol.Serialize(avatarInfo.playerID, bytes, ref index);
            Protocol.Serialize(avatarInfo.viewID, bytes, ref index);
            outStream.Write(bytes, 0, 8);

            index += SerializeVector3(outStream, avatarInfo.spawnPosition);
            index += SerializeColor(outStream, avatarInfo.color);
        }
        return 36;
    }

    private static object DeserializeAvatarInfo(StreamBuffer inStream, short length)
    {
        AvatarInfo avatarInfo = new AvatarInfo();
        int index = 0;
        lock(memAvatarInfo)
        {
            inStream.Read(memAvatarInfo, 0, 8);
            Protocol.Deserialize(out avatarInfo.playerID, memAvatarInfo, ref index);
            Protocol.Deserialize(out avatarInfo.viewID, memAvatarInfo, ref index);

            avatarInfo.spawnPosition = (Vector3)DeserializeVector3(inStream, 12);
            index += 12;

            avatarInfo.color = (Color)DeserializeColor(inStream, 16);
            index += 16;
        }

        return avatarInfo;
    }
    #endregion
}
