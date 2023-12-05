using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using Client;

namespace com.medcare360.utt
{
    public class AtracsysClient : ClientManager, IClientSide
    {
        public AtracsysClient(ITrackerController trackerController)
        {
            /*TrackingSystemSelector.Instance.UpdateClientLookup(
                TRACKER_TYPE.ATRACSYS, this, add: true);*/
            trackerController.OnClientInit.Invoke(TRACKER_TYPE.ATRACSYS,
                this, true);
        }
        public void InitClientSide()
        {
            Debug.Log("Inside Atracsys Client");
            UDPConfig udpConfig = GetConfigs(TRACKER_TYPE.ATRACSYS);
            
            ConnectAllUDPClients(udpConfig);
        }

        public void DeInitClientSide()
        {
            Debug.Log("Inside Atracsys Client DeInit");
            CloseAndDisposeClients();
        }

        public void DisableDataBroadcast()
        {
            throw new System.NotImplementedException();
        }

        public void EnableDataBroadcast()
        {
            throw new System.NotImplementedException();
        }

        public void ProcessData(byte[] data)
        {
            CreateBroadcast(data, TRACKER_TYPE.ATRACSYS);
        }

        public void Initialize(IUdpController udpController)
        {
            this.udpController = udpController;
            InitClientSide();
        }

        public void DeInitialize()
        {
            DeInitClientSide();
        }

        public UDPConfig GetUDPConfigs()
        {
            return GetConfigs(TRACKER_TYPE.ATRACSYS);
        }
    }

}