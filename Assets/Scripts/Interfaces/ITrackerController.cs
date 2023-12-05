using UnityEngine.Events;

namespace com.medcare360.utt
{
    public interface ITrackerController
    {
        IServerSide AtracsysServerSide { get; }
        void Initialize(IUdpController udpController, IExeController exeController);
        void ReInitialize();
        void DeInitClientServer();
        void InitializeUI(UIManager uiManager);
        UnityAction<TRACKER_TYPE, IClientSide, bool> OnClientInit { get; set; }
        UnityAction<TRACKER_TYPE, IServerSide, bool> OnServerInit { get; set; }
    }
}