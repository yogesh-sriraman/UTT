using Client;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Application entry point.<br />
/// This object is a hub for connecting different parts of the application.<br />
/// Any class, to access another class has to go through this class.<br />
/// Controls the initialization order of other objects.
/// </summary>
/// 
namespace com.medcare360.utt
{
    public class ProcessManager : SingletonManager<ProcessManager>
    {
        [SerializeField]
        private TextMeshProUGUI versionText;
        public int targetFps;

        public List<SkeletonMarker> markers;

        public Dictionary<int, SkeletonMarker> geomLookup;

        public Fiducial[] fiducials;
        public Fiducial orphanFiducial;
        public Process atracsysHandle;

        public IExeController exeController;
        public IUdpController udpController;
        public ITrackerController trackerController;

        public UIManager uiManager;

        public PopupHandler popupHandler;

        public ApplicationSettings appSettings;

        private void Awake()
        {
            InitializeConfigs();
        }

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
        }

        public void DisplayVersion()
        {
            versionText.text = appSettings.Version;
        }

        private void Initialize()
        {
            appSettings.OnVersionChanged += DisplayVersion;
            versionText.text = appSettings.Version;
            targetFps = -1;
            markers = new List<SkeletonMarker>();
            fiducials = null;
            geomLookup = new Dictionary<int, SkeletonMarker>();


            //InitializeConfigs();
            
            //atracsysHandle = new Process();
            uiManager.Initialize(
                () => OnClickResetConnection());
            ThreadManager.Instance.Initialize();
        }


        private void InitializeConfigs()
        {

            trackerController = GenericFunctions
                .GetInterfaceOfTypeFromScene<ITrackerController>(typeof(ITrackerController));

            trackerController.InitializeUI(uiManager);

            udpController = GenericFunctions
                .CreateClassOfType<IUdpController>(typeof(IUdpController));

            exeController = GenericFunctions
                .CreateClassOfType<IExeController>(typeof(IExeController));

            UnityAction<IUdpController, IExeController> action = trackerController.Initialize;

            udpController.Initialize(action, exeController);


            /*udpLookup = new Dictionary<TRACKER_TYPE, UDPConfig>();
            for (int i = 0; i < udpConfigs.Length; i++)
            {
                UDPConfig udpConfig = udpConfigs[i];
                udpLookup.Add(udpConfig.type, udpConfig);
            }*/
        }

        private void OnApplicationQuit()
        {
            trackerController.DeInitClientServer();
            exeController.StopEXE(TRACKER_TYPE.ATRACSYS);
        }

        public void FindEXE()
        {
            exeController.FindEXE();
        }

        public void OnClickResetConnection()
        {
            uiManager.HideResetButton();

            trackerController.DeInitClientServer();
            exeController.StopEXE(TRACKER_TYPE.ATRACSYS);

            trackerController.ReInitialize();


            markers = new List<SkeletonMarker>();
        }



        /*public void SendSerializedData(byte[] data, IServerSide server)
        {
            UDPConfig udpConfig = GetConfig(server.trackerType);
            UpdateDataBroadcast(success: true, udpConfig.broadcastClients);
            IClientSide activeClient = TrackingSystemSelector.Instance.GetActiveClient();
            activeClient.ProcessData(data);
        }*/

        public void UpdateDataBroadcast(bool success, List<ClientIPPortTuple> clients = null,
            string msg = null)
        {
            uiManager.UpdateDataBroadcast(success, clients, msg);
        }

        public void Reset()
        {
            markers = new List<SkeletonMarker>();
            fiducials = null;
            geomLookup = new Dictionary<int, SkeletonMarker>();
            ResetDataReception();
            UpdateDataBroadcast(false);
        }

        public void ResetDataReception()
        {
            uiManager.ResetDataReception();
        }

        private void Update()
        {
            if(Application.targetFrameRate != targetFps)
            {
                Application.targetFrameRate = targetFps;
            }
            uiManager.DisplayTrackerInfo(markers);
        }

        public void UpdateTrackerLookup(TRACKER_TYPE type, IDllConnect dll)
        {
            //dllManager.UpdateTrackerLookup(type, dll);
        }

        public void CloseApplication()
        {
            Application.Quit();
        }

        #region ConnectToCAS

        public Process cas360handle;

        private bool FindCAS()
        {
            Process cas360 = Process.GetProcessesByName("360CAS").FirstOrDefault();
            if(cas360 == null)
            {
                cas360handle = null;
                return false;
            }
            else
            {
                cas360handle = cas360;
            }

            return true;
        }

        private bool StartCAS()
        {
            cas360handle = new Process();
            cas360handle.StartInfo.UseShellExecute = true;
            cas360handle.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            cas360handle.StartInfo.FileName = appSettings.pathToCAS;
            return cas360handle.Start();
        }

        public void OnClickOpenCAS()
        {
            if(FindCAS())
            {
                //show popup
                UnityEngine.Debug.LogError("CAS already running!");
                popupHandler.ShowPopup("CAS already running!\nPlease close the current 360CAS App.", MESSAGE_STATUS.ERROR);
            }
            else
            {
                StartCAS();
            }
        }
        #endregion
    }
}