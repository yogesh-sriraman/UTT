using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.medcare360.utt
{
    public class MetriaServer : ServerManager, IServerSide
    {
        private IClientSide _client;
        public IClientSide CoupledClient
        {
            get { return _client; }
            set { _client = value; }
        }
        public MetriaServer(ITrackerController trackerController)
        {
            trackerController.OnServerInit.Invoke(trackerType, this, true);
            /*TrackingSystemSelector.Instance.UpdateServerLookup(
                TRACKER_TYPE.METRIA, this, add: true);*/
        }

        public TRACKER_TYPE trackerType
        {
            get => TRACKER_TYPE.METRIA;
        }


        public void DeInitialize(IExeController exeController)
        {
            Debug.Log("Inside Metria Server DeInit");
            string key = TRACKER_TYPE.METRIA.ToString() + "_Server";
            ThreadManager.Instance.AbortThread(key);
            ProcessManager.Instance.Reset();

            udpServer.Close();
            udpServer.Dispose();
        }

        public void Initialize(IExeController exeController,
            IUdpController udpController, UIManager uiManager)
        {
            this.udpController = udpController;
            this.uiManager = uiManager;
            ProcessManager.Instance.targetFps = 90;
            Debug.Log("Inside Metria Server");
            udpServer = ProcessServer(this, trackerType, needsUDPCheck: true);
        }

        public void ProcessData(Skeleton skeleton)
        {
            throw new System.NotImplementedException("Not done!");
        }

        public void ProcessData(byte[] data)
        {
            UpdateData(data);
        }

        private void UpdateData(byte[] data)
        {
            Skeleton skeleton = DecodeSuperPacket(data);
            int numMarkers;
            ProcessManager.Instance.markers = GetNumberOfMarkers(skeleton, out numMarkers);

            udpController.SendSerializedData(data, CoupledClient);
        }

        private List<SkeletonMarker> GetNumberOfMarkers(Skeleton skeleton, out int numOfMarkers)
        {
            SkeletonMarker[] markers = skeleton.aMarkers;
            List<SkeletonMarker> skeletonMarkers = new List<SkeletonMarker>();

            int numMarkers = 0;

            for (int i = 0; i < markers.Length; i++)
            {
               SkeletonMarker marker = markers[i];

                if (marker.markerSeriesNumber != 0)
                {
                    ++numMarkers;
                    skeletonMarkers.Add(marker);
                    ProcessManager.Instance.geomLookup.Add(marker.markerSeriesNumber, marker);
                }
            }

            numOfMarkers = numMarkers;
            return skeletonMarkers;
        }

    }

}