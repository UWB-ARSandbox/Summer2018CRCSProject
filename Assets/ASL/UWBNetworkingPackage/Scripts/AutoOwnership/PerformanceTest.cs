using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace UWBNetworkingPackage
{
    /// <summary>
    /// This class acted as a performance test demonstration script for the
    /// original implementation of the ownership functionality that was baked
    /// into ASL. Kept for archival purposes only.
    /// </summary>
    public class PerformanceTest : Photon.PunBehaviour
    {
        #region Fields
        #region Public Fields
        /// <summary>
        /// The number of primitives to be generated for the demonstration.
        /// </summary>
        public int COUNT = 250;

        /// <summary>
        /// The maximum number of primitives to be generated for the demonstration.
        /// </summary>
        public int MAX_COUNT = 900;
        #endregion

        #region Private Fields
        /// <summary>
        /// A reference to the stopwatch for easy access.
        /// </summary>
        private System.Diagnostics.Stopwatch stopwatch;

        /// <summary>
        /// The prefab to be used in place of the primitive.
        /// </summary>
        private string prefabName = "aslCube";
        #endregion
        #endregion

        #region Methods
        #region Public Methods
        /// <summary>
        /// Method for handling the spontaneous generation of a large number of
        /// primitives in the scene.
        /// </summary>
        public void CreateCubes()
        {
            stopwatch.Reset();

            Vector3 position;
            for (int i = 0; i < COUNT; i++)
            {
                stopwatch.Start();
                position = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-1.0f, 1.0f), Random.Range(0.0f, 5.0f));
                PhotonNetwork.Instantiate(prefabName, position, Quaternion.identity, 0);
                stopwatch.Stop();
            }

            Debug.LogWarning("Ownership Transferral metrics: Total time to create " + COUNT + " " + prefabName + " instantiations = " + stopwatch.ElapsedMilliseconds + "ms");

            PhotonNetwork.DestroyAll();
        }

        /// <summary>
        /// Method to track time consumed for generating remote instantiations
        /// of primitives for demonstration.
        /// </summary>
        /// 
        /// <param name="numInstantiations">
        /// The number of instantiations to instantiate.
        /// </param>
        [PunRPC]
        public void StressTestRemote(int numInstantiations)
        {
            if (numInstantiations > MAX_COUNT)
                numInstantiations = MAX_COUNT;

            stopwatch.Reset();

            Vector3 position;

            for (int i = 0; i < numInstantiations; ++i)
            {
                stopwatch.Start();
                position = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-1.0f, 1.0f), Random.Range(0.0f, 5.0f));
                PhotonNetwork.Instantiate(prefabName, position, Quaternion.identity, 0);
                stopwatch.Stop();
            }

            Debug.LogWarning("Ownership Transferral metrics: Total time to create " + MAX_COUNT + " " + prefabName + " instantiations = " + stopwatch.ElapsedMilliseconds + "ms");
        }

        /// <summary>
        /// A method to test for timing efficiency of ownership transferral.
        /// </summary>
        /// 
        /// <param name="numInstantiations">
        /// The number of times to test ownership transferral.
        /// </param>
        public void StressTest(int numInstantiations)
        {
            gameObject.GetPhotonView().RPC("StressTestRemote", PhotonTargets.Others, numInstantiations);
            StartCoroutine(WaitForTime(2));

            int numOps = 0;

            stopwatch.Reset();
            stopwatch.Start();

            var viewList = GameObject.FindObjectsOfType<PhotonView>();
            for (int i = 0; i < viewList.Length; i++)
            {
                int randNumGrabs = (int)(Random.Range(0, 1) * 10);
                string GrabRPCName = "Grab";
                if (ASL.Adapters.PUN.RPCManager.IsAnRPC(GrabRPCName))
                {
                    for (int j = 0; j < randNumGrabs; j++)
                    {
                        viewList[i].RPC(GrabRPCName, PhotonTargets.Others);
                        ++numOps;
                    }
                }
            }

            stopwatch.Stop();

            Debug.LogWarning("Ownership Transferral metrics: Total time to finish " + numOps + " grab operations = " + stopwatch.ElapsedMilliseconds + "ms");
        }
        
        /// <summary>
        /// ?
        /// </summary>
        /// <param name="numInstantiations"></param>
        public void StressTestSimultaneous(int numInstantiations)
        {
            Hashtable properties = new Hashtable();
            properties.Add("Start", true);
            properties.Add("NumInstantiations", numInstantiations);

            PhotonNetwork.room.SetCustomProperties(properties);
        }

        /// <summary>
        /// Tracks whether the number of requested instantiations changes at
        /// any point during runtime and reruns the tests given the new requested
        /// number of primitive instantiations.
        /// </summary>
        /// 
        /// <param name="propertiesThatChanged">
        /// The properties that changed, triggering the event.
        /// </param>
        public override void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
        {
            base.OnPhotonCustomRoomPropertiesChanged(propertiesThatChanged);

            if (propertiesThatChanged.ContainsKey("NumInstantiations"))
            {
                int numInstantiations = (int)propertiesThatChanged["NumInstantiations"];

                stopwatch.Reset();

                Vector3 position;
                for (int i = 0; i < numInstantiations; i++)
                {
                    stopwatch.Start();
                    position = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-1.0f, 1.0f), Random.Range(0.0f, 5.0f));
                    PhotonNetwork.Instantiate(prefabName, position, Quaternion.identity, 0);
                    stopwatch.Stop();
                }

                Debug.LogWarning("Ownership Transferral metrics: Total time to create " + numInstantiations + " " + prefabName + " instantiations = " + stopwatch.ElapsedMilliseconds + "ms");

                Queue<PhotonView> viewQueue = ShufflePhotonViews();

                stopwatch.Reset();
                stopwatch.Start();
                int numViews = viewQueue.Count;
                while (viewQueue.Count > 0)
                {
                    string GrabRPCName = "Grab";
                    viewQueue.Dequeue().RPC(GrabRPCName, PhotonTargets.Others);
                }
                stopwatch.Stop();

                Debug.LogWarning("Ownership Transferral metrics: Total time to try grabbing " + numViews + " " + prefabName + " = " + stopwatch.ElapsedMilliseconds + "ms");

                int numViewsOwned = 0;
                var viewArray = GameObject.FindObjectsOfType<PhotonView>();
                //foreach (PhotonView view in viewArray)
                for(int i = 0; i < viewArray.Length; i++)
                {
                    PhotonView view = viewArray[i];
                    
                    if (view.owner.Equals(PhotonNetwork.player.ID))
                    {
                        Debug.Log("Original Owner: " + (view.viewID / 1000) + "; Current Owner: " + view.owner);
                        ++numViewsOwned;
                    }
                }

                Debug.LogWarning("Ownership Transferral metrics: Total # of owned objects = " + numViewsOwned);
            }
        }
        #endregion
        
        #region Protected Methods
        /// <summary>
        /// Unity method that is called prior to runtime. Triggers before "Awake"
        /// methods. Initializes stopwatch.
        /// </summary>
        protected void Start()
        {
            stopwatch = new System.Diagnostics.Stopwatch();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Simple helper timer method to wait for a specified amount of time.
        /// </summary>
        /// 
        /// <param name="seconds">
        /// The number of seconds to wait.
        /// </param>
        /// 
        /// <returns>
        /// A yield enumerator that tracks how much time has passed.
        /// </returns>
        private IEnumerator WaitForTime(int seconds)
        {
            yield return new WaitForSeconds(seconds);
        }

        /// <summary>
        /// Helper method to shuffle photon views of GameObjects in scene to 
        /// randomize objects interacted with.
        /// </summary>
        /// <returns></returns>
        private Queue<PhotonView> ShufflePhotonViews()
        {
            // shuffle 
            var viewArray = GameObject.FindObjectsOfType<PhotonView>();
            Queue<PhotonView> viewQueue = new Queue<PhotonView>(viewArray);
            int numbersShuffled = 0;
            while (numbersShuffled < viewArray.Length)
            {
                int randNum = (int)(Random.Range(1, 5) * 6);
                Stack<PhotonView> tempStack = new Stack<PhotonView>();
                for (int j = 0; j < randNum; j++)
                {
                    tempStack.Push(viewQueue.Dequeue());
                }
                while (tempStack.Count > 0)
                {
                    viewQueue.Enqueue(tempStack.Pop());
                    ++numbersShuffled;
                }
            }

            return viewQueue;
        }
        #endregion
#endregion
    }
}