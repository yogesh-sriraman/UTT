using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Client;
using UnityEngine;

namespace com.medcare360.utt
{
    public class ClientManager
    {
        protected IUdpController udpController;
        protected UdpClient[] udpClient;

        protected UDPConfig GetConfigs(TRACKER_TYPE type)
        {
            //return ProcessManager.Instance.GetConfig(type);
            return udpController.GetConfig(type);
        }
        public void CreateBroadcast(byte[] data, TRACKER_TYPE type)
        {
            if (data == null)
                return;

            if (type == TRACKER_TYPE.DEFAULT || type == TRACKER_TYPE.INVALID)
                return;

            //Loop through all broadcastClients and send data to them
            foreach (UdpClient client in udpClient)
            {
                client.Send(data, data.Length);
            }
        }
        
        protected void ConnectAllUDPClients(UDPConfig udpConfig)
        {
            if (udpConfig.BroadcastClients == null)
            {
                Debug.LogError("No broadcast clients found");
                return;
            }
            
            if(udpConfig.BroadcastClients.Count == 0)
            {
                Debug.LogError("No broadcast clients found");
                return;
            }
            
            udpClient = new UdpClient[udpConfig.BroadcastClients.Count];
            //Loop through all broadcastClients and connect to them
            for (int i = 0; i < udpConfig.BroadcastClients.Count; i++)
            {
                udpClient[i] = new UdpClient();
                IPAddress address = IPAddress.Parse(udpConfig.BroadcastClients[i].clientIP);
                udpClient[i].Connect(address, udpConfig.BroadcastClients[i].clientPort);
            }
        }
        
        protected void CloseAndDisposeClients()
        {
            //Loop through all broadcastClients and close and dispose them
            if (udpClient == null)
                return;
            foreach (UdpClient client in udpClient)
            {
                client.Close();
                client.Dispose();
            }
        }
    }

}