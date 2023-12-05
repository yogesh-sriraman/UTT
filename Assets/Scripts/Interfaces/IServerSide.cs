
namespace com.medcare360.utt
{
    public interface IServerSide
    {
        IClientSide CoupledClient { get; set; }
        TRACKER_TYPE trackerType { get; }
        void Initialize(IExeController exeController, IUdpController udpController,
            UIManager uiManager);
        void DeInitialize(IExeController exeController);
        void ProcessData(byte[] data);
        void ProcessData(Skeleton skeleton);
    }
}