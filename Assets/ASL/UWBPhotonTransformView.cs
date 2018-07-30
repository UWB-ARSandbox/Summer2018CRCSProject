// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformView.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize Transforms via PUN PhotonView.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using UnityEngine;

/// <summary>
/// This class helps you to synchronize position, rotation and scale
/// of a GameObject. It also gives you many different options to make
/// the synchronized values appear smooth, even when the data is only
/// send a couple of times per second.
/// Simply add the component to your GameObject and make sure that
/// the PhotonTransformView is added to the list of observed components
/// </summary>
[RequireComponent(typeof(PhotonView))]
[AddComponentMenu("Photon Networking/UWB Photon Transform View")]
public class UWBPhotonTransformView : MonoBehaviour, IPunObservable
{
    #region Fields
    /// <summary>
    /// Object display text.
    /// </summary>
    private TextMesh objText = null;

    //Since this component is very complex, we seperated it into multiple objects.
    //The PositionModel, RotationModel and ScaleMode store the data you are able to
    //configure in the inspector while the control objects below are actually moving
    //the object and calculating all the inter- and extrapolation

    /// <summary>
    /// The (serializable) local position of the object.
    /// </summary>
    [SerializeField]
    PhotonTransformViewPositionModel m_PositionModel = new PhotonTransformViewPositionModel();

    /// <summary>
    /// The (serializable) local orientation of the object.
    /// </summary>
    [SerializeField]
    PhotonTransformViewRotationModel m_RotationModel = new PhotonTransformViewRotationModel();

    /// <summary>
    /// The (serializable) local scale of the object.
    /// </summary>
    [SerializeField]
    PhotonTransformViewScaleModel m_ScaleModel = new PhotonTransformViewScaleModel();

    /// <summary>
    /// Handles synchronization of local position of attached GameObject.
    /// </summary>
    PhotonTransformViewPositionControl m_PositionControl;

    /// <summary>
    /// Handles synchronization of local orientation of attached GameObject.
    /// </summary>
    PhotonTransformViewRotationControl m_RotationControl;

    /// <summary>
    /// Handles synchronization of local scale of attached GameObject.
    /// </summary>
    PhotonTransformViewScaleControl m_ScaleControl;

    /// <summary>
    /// ?
    /// </summary>
    Color netColor;

    /// <summary>
    /// The Photon View component associated with this GameObject.
    /// </summary>
    PhotonView m_PhotonView;

    /// <summary>
    /// Determines if you're connected to the backend PUN network.
    /// </summary>
    bool m_ReceivedNetworkUpdate = false;

    /// <summary>
    /// Flag to skip initial data when Object is instantiated and rely on the first deserialized data instead.
    /// </summary>
    bool m_firstTake = false;
    #endregion

    #region Methods
    /// <summary>
    /// Triggers after all "Start" methods are successfully called or when this
    /// class is enabled. Initializes position, rotation, and scale control.
    /// Dynamically sets the Photon View reference for the GameObject associated
    /// with this script. Sets the netColor (what does this do...?)
    /// </summary>
    void Awake()
    {
        this.m_PhotonView = GetComponent<PhotonView>();

        this.m_PositionControl = new PhotonTransformViewPositionControl(this.m_PositionModel);
        this.m_RotationControl = new PhotonTransformViewRotationControl(this.m_RotationModel);
        this.m_ScaleControl = new PhotonTransformViewScaleControl(this.m_ScaleModel);

        if (this.gameObject.GetComponent<Renderer>() != null && this.gameObject.GetComponent<Renderer>().material.HasProperty("_Color"))
        {
            netColor = this.gameObject.GetComponent<Renderer>().material.color;
        }
        else
        {
            netColor = Color.white;
        }
    }

    /// <summary>
    /// Unity method that triggers when a script is first registered or enabled.
    /// </summary>
    void OnEnable()
    {
        m_firstTake = true;
    }

    /// <summary>
    /// Unity method that is called every frame. Updates the position, orientation,
    /// and scale associated with the GameObject this script is attached to by
    /// querying the network for the transform it's supposed to be aligned with.
    /// </summary>
    void Update()
    {
        if (this.m_PhotonView == null || this.m_PhotonView.isMine == true || PhotonNetwork.connected == false)
        {
            return;
        }

        this.UpdatePosition();
        this.UpdateRotation();
        this.UpdateScale();
    }

    /// <summary>
    /// Queries the network to determine what local position the GameObject this
    /// script is attached to is supposed to be aligned with.
    /// </summary>
    void UpdatePosition()
    {
        if (this.m_PositionModel.SynchronizeEnabled == false || this.m_ReceivedNetworkUpdate == false)
        {
            return;
        }

        transform.localPosition = this.m_PositionControl.UpdatePosition(transform.localPosition);
    }

    /// <summary>
    /// Queries the network to determine what local orientation the GameObject this
    /// script is attached to is supposed to be aligned with.
    /// </summary>
    void UpdateRotation()
    {
        if (this.m_RotationModel.SynchronizeEnabled == false || this.m_ReceivedNetworkUpdate == false)
        {
            return;
        }

        transform.localRotation = this.m_RotationControl.GetRotation(transform.localRotation);
    }

    /// <summary>
    /// Queries the network to determine what local scale the GameObject this
    /// script is attached to is supposed to be aligned with.
    /// </summary>
    void UpdateScale()
    {
        if (this.m_ScaleModel.SynchronizeEnabled == false || this.m_ReceivedNetworkUpdate == false)
        {
            return;
        }

        transform.localScale = this.m_ScaleControl.GetScale(transform.localScale);
    }

    /// <summary>
    /// These values are synchronized to the remote objects if the interpolation mode
    /// or the extrapolation mode SynchronizeValues is used. Your movement script should pass on
    /// the current speed (in units/second) and turning speed (in angles/second) so the remote
    /// object can use them to predict the objects movement.
    /// </summary>
    /// <param name="speed">The current movement vector of the object in units/second.</param>
    /// <param name="turnSpeed">The current turn speed of the object in angles/second.</param>
    public void SetSynchronizedValues(Vector3 speed, float turnSpeed)
    {
        this.m_PositionControl.SetSynchronizedValues(speed, turnSpeed);
    }

    /// <summary>
    /// Handles Photon Serialization.
    /// </summary>
    /// 
    /// <param name="stream">
    /// The PhotonStream to write information to.
    /// </param>
    /// <param name="info">
    /// Information received from the event that triggered this method call.
    /// </param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        this.m_PositionControl.OnPhotonSerializeView(transform.localPosition, stream, info);
        this.m_RotationControl.OnPhotonSerializeView(transform.localRotation, stream, info);
        this.m_ScaleControl.OnPhotonSerializeView(transform.localScale, stream, info);
        //this.OnPhotonSerializeColor(stream, info);

        if (this.m_PhotonView.isMine == false && this.m_PositionModel.DrawErrorGizmo == true)
        {
            this.DoDrawEstimatedPositionError();
        }

        if (stream.isReading == true)
        {
            this.m_ReceivedNetworkUpdate = true;

            // force latest data to avoid initial drifts when player is instantiated.
            if (m_firstTake)
            {
                m_firstTake = false;
                this.transform.localPosition = this.m_PositionControl.GetNetworkPosition();
                this.transform.localRotation = this.m_RotationControl.GetNetworkRotation();
                this.transform.localScale = this.m_ScaleControl.GetNetworkScale();
            }

        }
    }

    /* Non-working serialize color code
    public void OnPhotonSerializeColor(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting == true)
        {
            Color rendColor = this.GetComponent<Renderer>().material.color;
            if (netColor == rendColor)
            {
                stream.SendNext(false);
                return;
            }
            stream.SendNext(rendColor.r);
            stream.SendNext(rendColor.g);
            stream.SendNext(rendColor.b);
            netColor = rendColor;
        }
        else
        {
            if((bool)stream.ReceiveNext() == false)
            {
                // No color sent
                return; 
            }

            float r = (float)stream.ReceiveNext();
            float g = (float)stream.ReceiveNext();
            float b = (float)stream.ReceiveNext();
            netColor = new Color(r, g, b);
            this.GetComponent<Renderer>().material.color = netColor;
        }
    }
    */

    /// <summary>
    /// Debug method to draw the estimated position error associated with a 
    /// GameObject's transform.
    /// </summary>
    void DoDrawEstimatedPositionError()
    {
        Vector3 targetPosition = this.m_PositionControl.GetNetworkPosition();

        // we are synchronizing the localPosition, so we need to add the parent position for a proper positioning.
        if (transform.parent != null)
        {
            targetPosition = transform.parent.position + targetPosition;
        }

        Debug.DrawLine(targetPosition, transform.position, Color.red, 2f);
        Debug.DrawLine(transform.position, transform.position + Vector3.up, Color.green, 2f);
        Debug.DrawLine(targetPosition, targetPosition + Vector3.up, Color.red, 2f);
    }

    /// <summary>
    /// Enables the synchronication of local GameObject position across all 
    /// ASL nodes.
    /// </summary>
    public void enableSyncPos()
    {
        this.m_PositionModel.SynchronizeEnabled = true;
    }
    /// <summary>
    /// Enables the synchronization of local GameObject rotation across all 
    /// ASL nodes.
    /// </summary>
    public void enableSyncRot()
    {
        this.m_RotationModel.SynchronizeEnabled = true;
    }
    /// <summary>
    /// Enables the synchronization of local GameObject scale across all
    /// ASL nodes.
    /// </summary>
    public void enableSyncScale()
    {
        this.m_ScaleModel.SynchronizeEnabled = true;
    }

    /// <summary>
    /// Changes the color associated with a very simple GameObject.
    /// 
    /// NOTE: Likely used for early, simple state synchronization tests and is not
    /// currently used in ASL.
    /// </summary>
    /// 
    /// <param name="r">
    /// The red component.
    /// </param>
    /// <param name="g">
    /// The green component.
    /// </param>
    /// <param name="b">
    /// The blue component.
    /// </param>
	[PunRPC]
	public void ChangeColorRPC(float r, float g, float b)
	{
		if (this.gameObject.GetComponent<Renderer>() == null)
		{
			Debug.LogWarning("RPC [ChangeColor] called on object with no Renderer");
			return;
		}

		this.gameObject.GetComponent<Renderer>().material.color = new Color(r, g, b);
	}

    /// <summary>
    /// Requests for a color change for the associated GameObject.
    /// 
    /// NOTE: Likely used for early, simple state synchronization tests and is not
    /// currently used in ASL.
    /// </summary>
	[PunRPC]
	public void RequestColorRPC()
	{
		Debug.Log("Request Color RPC");
		PhotonView phov = gameObject.GetComponent<PhotonView>();
		Color thisColor = gameObject.GetComponent<Renderer>().material.color;
		phov.RPC("ChangeColorRPC", PhotonTargets.All, thisColor.r, thisColor.g, thisColor.b);
	}

    /// <summary>
    /// Sets text associated with a TextMesh GameObject in the scene.
    /// </summary>
    /// 
    /// <param name="text">
    /// The text to display.
    /// </param>
	[PunRPC]
	public void setTextRPC(string text)
	{
		if (objText == null)
			createText();

		objText.text = text;
	}

    /// <summary>
    /// Creates a TextMesh GameObject that can display a message in the scene.
    /// </summary>
	private void createText()
	{
		GameObject objTextGO = new GameObject();
		objTextGO.transform.parent = gameObject.transform;
		objTextGO.transform.localPosition = new Vector3(0, gameObject.transform.localScale.y, 0);
		objText = objTextGO.AddComponent<TextMesh>();
		objText.anchor = TextAnchor.LowerCenter;
	}
    #endregion
}