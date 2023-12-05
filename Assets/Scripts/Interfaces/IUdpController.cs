
using UnityEngine.Events;

namespace com.medcare360.utt
{
    public interface IUdpController
    {
        UnityAction<IUdpController, IExeController> OnInitialized { get; set; }
        void Initialize(UnityAction<IUdpController, IExeController> action,
            IExeController exeController);
        UDPConfig GetConfig(TRACKER_TYPE type);
        bool IsValidTracker(TRACKER_TYPE type);
        void SendSerializedData(byte[] data, IClientSide client);
    }
}