using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace com.medcare360.utt
{
    public class TrackingSystemSelector : MonoBehaviour, ITrackerController
    {
        public TMP_Dropdown trackerDropdown;
        public TMP_Dropdown hostTrackerOptions;
        public TMP_Dropdown actionTrackerOptions;

        public int activeTrackingSystem;
        public IExeController exeController;
        public IUdpController udpController;
        private UIManager _uiManager;

        private IClientSide[] clients;
        private Dictionary<TRACKER_TYPE, IClientSide> clientLookup;
        private Dictionary<TRACKER_TYPE, IServerSide> serverLookup;

        
        private OptionLookup optionLookup;

        private Dictionary<int, TRACKER_TYPE> trackerLookup;


        #region ITrackerController Methods
        private UnityAction<TRACKER_TYPE, IClientSide, bool> _onClientInit;
        private UnityAction<TRACKER_TYPE, IServerSide, bool> _onServerInit;
        public UnityAction<TRACKER_TYPE, IClientSide, bool> OnClientInit
        {
            get => _onClientInit;
            set { _onClientInit = value; }
        }
        public UnityAction<TRACKER_TYPE, IServerSide, bool> OnServerInit
        {
            get => _onServerInit;
            set { _onServerInit = value; }
        }

        public IServerSide AtracsysServerSide => GetServer(1);

        delegate void OnLoadCompleteDelegate(AsyncOperationHandle<OptionLookup> obj);
        OnLoadCompleteDelegate onLoadComplete;
        public void OnOptionsLoadComplete(AsyncOperationHandle<OptionLookup> obj)
        {
            optionLookup = obj.Result;
        }

        delegate void PostLoadInitDelegate();
        PostLoadInitDelegate postLoadInit;
        public void PostLoadInit()
        {
            InitializeOptions();
            InitializeClients();
            InitializeServers();
            InitializeDropdown();
        }

        public void Initialize(IUdpController udpController, IExeController exeController)
        {
            this.udpController = udpController;
            this.exeController = exeController;

            activeTrackingSystem = 0;

            List<string> keys = new List<string>();
            keys.Add("TrackerOptions");

            onLoadComplete = OnOptionsLoadComplete;
            postLoadInit = PostLoadInit;

            AddressablesManager.LoadSOWithKeys<OptionLookup>(keys,
                onLoadComplete, null, postLoadInit, null);

            Invoke("ForceAtracsys", 1);
        }

        public void ForceAtracsys()
        {
            trackerDropdown.value = (int)TRACKER_TYPE.ATRACSYS;
        }

        public void OnClickReconnect()
        {
            UpdateTrackingSystem((int)TRACKER_TYPE.ATRACSYS);
        }

        public void ReInitialize()
        {
            trackerLookup = null;
            clientLookup = null;
            serverLookup = null;

            if (trackerDropdown != null)
            {
                trackerDropdown.onValueChanged.RemoveAllListeners();
                trackerDropdown.value = 0;
            }

            Initialize(udpController, exeController);
        }

        public void DeInitClientServer()
        {
            DeInitPreviousTrackingSystem();
        }

        public void InitializeUI(UIManager uiManager)
        {
            _uiManager = uiManager;
        }
        #endregion

        #region InitializeOptions

        private void InitializeOptions()
        {
            trackerLookup = new Dictionary<int, TRACKER_TYPE>();
            for(int i = 0; i < optionLookup.trackerOptions.Length; i++)
            {
                int key = optionLookup.trackerOptions[i].optionValue;
                TRACKER_TYPE type = optionLookup.trackerOptions[i].optionType;
                trackerLookup.Add(key, type);
            }
        }
        #endregion

        #region Initialize Clients
        private void InitializeClients()
        {
            OnClientInit = UpdateClientLookup;
            clientLookup = new Dictionary<TRACKER_TYPE, IClientSide>();
            GenericFunctions.GetClassesFromAssembly<IClientSide, ITrackerController>(
                typeof(IClientSide), this);

        }

        public void UpdateClientLookup(TRACKER_TYPE type, IClientSide client, bool add)
        {
            if(add)
            {
                clientLookup.Add(type, client);
            }
            else
            {
                clientLookup.Remove(type);
            }    
        }
        #endregion

        #region Initialize Servers
        private void InitializeServers()
        {
            OnServerInit = UpdateServerLookup;
            serverLookup = new Dictionary<TRACKER_TYPE, IServerSide>();
            GenericFunctions.GetClassesFromAssembly<IServerSide, ITrackerController>(
                typeof(IServerSide), this);
        }

        public void UpdateServerLookup(TRACKER_TYPE type, IServerSide server, bool add)
        {
            if (add)
            {
                serverLookup.Add(type, server);
                server.CoupledClient = clientLookup[type];
            }
            else
            {
                serverLookup.Remove(type);
            }
        }
        #endregion

        #region Initialize Dropdown
        private void InitializeDropdown()
        {
            trackerDropdown.onValueChanged
                .AddListener(val => UpdateTrackingSystem(val));
        }
        #endregion

        #region Tracking System methods
        private void UpdateTrackingSystem(int val)
        {
            DeInitPreviousTrackingSystem();
            InitializeNewTrackingSystem(val);
        }

        public void DeInitPreviousTrackingSystem()
        {
            if (!ValidTrackingSystem())
                return;

            DeInitializeClientAndServer();
        }

        private void InitializeNewTrackingSystem(int val)
        {
            activeTrackingSystem = val;

            if (!ValidTrackingSystem())
                return;

            InitializeClientAndServer();
        }

        private bool ValidTrackingSystem()
        {
            if (trackerLookup == null)
                return false;

            return trackerLookup.ContainsKey(activeTrackingSystem);
        }

        #endregion

        #region Client and Server methods
        private void InitializeClientAndServer()
        {
            IClientSide client = GetClient(activeTrackingSystem);
            client.Initialize(udpController);

            IServerSide server = GetServer(activeTrackingSystem);
            server.Initialize(exeController, udpController, _uiManager);
        }

        private void DeInitializeClientAndServer()
        {
            IClientSide client = GetClient(activeTrackingSystem);
            client.DeInitialize();

            IServerSide server = GetServer(activeTrackingSystem);
            server.DeInitialize(exeController);
        }

        public IClientSide GetClient(int clientVal)
        {
            IClientSide client;

            TRACKER_TYPE key = GetTracker(clientVal);

            clientLookup.TryGetValue(key, out client);
            return client;
        }

        public IServerSide GetServer(int serverVal)
        {
            IServerSide server;
            TRACKER_TYPE key = GetTracker(serverVal);

            serverLookup.TryGetValue(key, out server);
            return server;
        }

        private TRACKER_TYPE GetTracker(int val)
        {
            try
            {
                return trackerLookup[val];
            }
            catch
            {
                return TRACKER_TYPE.INVALID;
            }
        }

        public IClientSide GetActiveClient()
        {
            if(!ValidTrackingSystem())
                return null;

            return GetClient(activeTrackingSystem);
        }
        #endregion
    }

}