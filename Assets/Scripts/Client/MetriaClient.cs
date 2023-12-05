using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace com.medcare360.utt
{
    public class MetriaClient : ClientManager, IClientSide
    {
        public MetriaClient(ITrackerController trackerController)
        {
            /*TrackingSystemSelector.Instance.UpdateClientLookup(
                TRACKER_TYPE.METRIA, this, add: true);*/

            trackerController.OnClientInit.Invoke(TRACKER_TYPE.METRIA,
                this, true);
        }
        public void InitClientSide()
        {
            Debug.Log("Inside Metria Client");
            UDPConfig udpConfig = GetConfigs(TRACKER_TYPE.METRIA);
            
            ConnectAllUDPClients(udpConfig);
        }

        public void DeInitClientSide()
        {
            Debug.Log("Inside Metria Client DeInit");
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
            CreateBroadcast(data, TRACKER_TYPE.METRIA);
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
            return GetConfigs(TRACKER_TYPE.METRIA);
        }
    }

}