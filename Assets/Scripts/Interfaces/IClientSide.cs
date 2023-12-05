namespace com.medcare360.utt
{
    public interface IClientSide
    {
        UDPConfig GetUDPConfigs();
        void Initialize(IUdpController udpController);
        void DeInitialize();
        void EnableDataBroadcast();
        void DisableDataBroadcast();
        void ProcessData(byte[] data);
    }
}