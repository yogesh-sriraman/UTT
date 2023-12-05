using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace com.medcare360.utt
{
    public class UDPManager : IUdpController
    {
        public UDPConfig[] udpConfigs;
        public Dictionary<TRACKER_TYPE, UDPConfig> udpLookup;


        public void AddToLookup(UDPConfig configs)
        {
            int size;
            try
            {
                size = udpConfigs.Length;
            }
            catch
            {
                size = 0;
                udpConfigs = new UDPConfig[size];
            }

            Array.Resize<UDPConfig>(ref udpConfigs, size + 1);
            udpConfigs[size] = configs;
            udpLookup.Add(configs.type, configs);
        }


        delegate void OnCompleteDelegate(AsyncOperationHandle<UDPConfig> obj,
            IExeController exeController);
        OnCompleteDelegate onCompleteDelegate;
        public void OnComplete(AsyncOperationHandle<UDPConfig> obj,
            IExeController exeController)
        {
            AddToLookup(obj.Result);
            ///TrackingSystemSelector.Instance.Initialize(this);
        }


        private UnityAction<IUdpController, IExeController> _onInitialized;
        public UnityAction<IUdpController, IExeController> OnInitialized
        {
            get => _onInitialized;
            set { _onInitialized = value; }
        }

        public void Initialize(UnityAction<IUdpController, IExeController> action,
            IExeController exeController)
        {
            OnInitialized = action;
            //udpLookup = new Dictionary<TRACKER_TYPE, UDPConfig>();


            udpLookup = new Dictionary<TRACKER_TYPE, UDPConfig>();
            /*for (int i = 0; i < udpConfigs.Length; i++)
            {
                UDPConfig udpConfig = udpConfigs[i];
                udpLookup.Add(udpConfig.type, udpConfig);
            }*/

            List<string> keys = new List<string>();
            keys.Add("UDPSettings");

            //StartCoroutine(LoadAddressablesWithKeys(keys, exeController));

            onCompleteDelegate = OnComplete;
            object[] loadDelegateParams = new object[1];
            loadDelegateParams[0] = exeController;

            object[] postLoadParams = new object[2];
            postLoadParams[0] = this;
            postLoadParams[1] = exeController;

            AddressablesManager.LoadSOWithKeys<UDPConfig>(keys,
                onCompleteDelegate, loadDelegateParams,
                OnInitialized, postLoadParams);
        }

        public UDPConfig GetConfig(TRACKER_TYPE type)
        {
            UDPConfig config;
            udpLookup.TryGetValue(type, out config);
            return config;
        }

        public bool IsValidTracker(TRACKER_TYPE type)
        {
            return udpLookup.ContainsKey(type);
        }


        public void SendSerializedData(byte[] data, IClientSide client)
        {
            client.ProcessData(data);
        }
    }
}