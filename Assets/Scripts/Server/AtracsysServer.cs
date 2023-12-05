
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

namespace com.medcare360.utt
{
    public class AtracsysServer : ServerManager, IServerSide
    {
        private bool success, stepsCompleted, dataBroadcastMsgSent;

        private IClientSide _client;
        public IClientSide CoupledClient
        {
            get { return _client; }
            set { _client = value; }
        }

        public TRACKER_TYPE trackerType
        {
            get => TRACKER_TYPE.ATRACSYS;
        }

        public AtracsysServer(ITrackerController trackerController)
        {
            trackerController.OnServerInit.Invoke(
                trackerType, this, true);
        }

        public void Initialize(IExeController exeController,
            IUdpController udpController, UIManager uiManager)
        {
            this.udpController = udpController;
            this.uiManager = uiManager;

            success = true;
            stepsCompleted = false;
            dataBroadcastMsgSent = false;

            Debug.Log("Inside Atracsys Server");
            ProcessManager.Instance.targetFps = -1;

            //ProcessManager.Instance.StartEXE(TRACKER_TYPE.ATRACSYS);

            bool exeStarted = exeController.StartEXE(trackerType);
            if(!exeStarted)
            {
                //TODO
            }

            udpServer = ProcessServer(this, trackerType, needsUDPCheck: false);
        }

        public void DeInitialize(IExeController exeController)
        {
            success = false;
            stepsCompleted = false;
            dataBroadcastMsgSent = false;
            Debug.Log("Inside Atracsys Server DeInit");
            string key = TRACKER_TYPE.ATRACSYS.ToString() + "_Server";
            ThreadManager.Instance.AbortThread(key);

            exeController.StopEXE(TRACKER_TYPE.ATRACSYS);
            ProcessManager.Instance.Reset();
            udpServer.Close();
            udpServer.Dispose();
        }

        public void ProcessData(Skeleton skeleton)
        {
            throw new System.NotImplementedException("Not done!");
        }

        public void ProcessData(byte[] data)
        {
            string packetStr = System.Text.Encoding.ASCII.GetString(data);

            UDPPacket udpPacket = JsonUtility.FromJson<UDPPacket>(packetStr);

            string header = udpPacket.header;
            string msg = "";
            switch (header)
            {
                case "wrapper":
                    if (uint.Parse(udpPacket.body) != 0)
                    {
                        msg = "Cannot initialize wrapper";
                        success = false;
                        Debug.LogError(msg);
                    }
                    else
                    {
                        msg = "Initializing";
                        success = true;
                        Debug.LogError(msg);
                    }
                    break;
                case "device":
                    if (uint.Parse(udpPacket.body) != 0)
                    {
                        msg = "Cannot find device";
                        success = false;
                        Debug.LogError(msg);
                    }
                    else
                    {
                        msg = "Device found";
                        success = true;
                        Debug.LogError(msg);
                    }
                    break;
                case "frame":
                    if (uint.Parse(udpPacket.body) != 0)
                    {
                        msg = "Cannot create frame";
                        success = false;
                        Debug.LogError(msg);
                    }
                    else
                    {
                        msg = "Frame created";
                        success = true;
                        Debug.LogError(msg);
                    }
                    break;
                case "geometry":
                    if (uint.Parse(udpPacket.body) != 0)
                    {
                        msg = "Cannot load geometry";
                        success = false;
                        Debug.LogError(msg);
                    }
                    else
                    {
                        msg = "Loaded geometries";
                        success = true;
                        Debug.LogError(msg);
                    }
                    break;
                case "data":
                    stepsCompleted = true;
                    uiManager.UpdateDataReception(success);
                    UpdateData(udpPacket.body);
                    break;
                case "error":
                    dataBroadcastMsgSent = false;
                    uiManager.ShowInformation(udpPacket.body,
                        MESSAGE_STATUS.ERROR);
                    break;
            }

            if(!stepsCompleted)
            {
                if(success)
                    uiManager.UpdateDataReception(msg);
                else
                    uiManager.UpdateDataReception(success);
            }
        }

        private void SetOrphanFiducial(Fiducial[] fiducials)
        {
            ProcessManager.Instance.fiducials = fiducials;
        }

        private void UpdateData(string dataStr)
        {
            Skeleton skeleton = JsonUtility.FromJson<Skeleton>(dataStr);
            int numMarkers;
            ProcessManager.Instance.markers = GetNumberOfMarkers(skeleton, out numMarkers);
            SetOrphanFiducial(skeleton.fiducials);

            byte[] bytes = EncodePacket(skeleton);
            udpController.SendSerializedData(bytes, CoupledClient);
            var bClients = CoupledClient.GetUDPConfigs().BroadcastClients;

            if(!dataBroadcastMsgSent)
            {
                dataBroadcastMsgSent = true;
                uiManager.UpdateDataBroadcast(true, bClients);
            }
        }

        private List<SkeletonMarker> GetNumberOfMarkers(Skeleton skeleton, out int numOfMarkers)
        {
            SkeletonMarker[] markers = skeleton.aMarkers;
            List<SkeletonMarker> skeletonMarkers = new List<SkeletonMarker>();
            int[] geoms = { 20, 72, 74, 75 };

            int numMarkers = 0;
            ProcessManager.Instance.geomLookup.Clear();

            for (int i = 0; i < geoms.Length; i++)
            {
                SkeletonMarker sm = markers[geoms[i]];
                int id = sm.markerSeriesNumber;
                if(id > 0)
                {
                    ++numMarkers;
                    skeletonMarkers.Add(sm);
                    ProcessManager.Instance.geomLookup.Add(id, sm);
                }
            }

            numOfMarkers = numMarkers;
            return skeletonMarkers;
        }
    }

}