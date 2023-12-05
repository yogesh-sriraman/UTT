using System.Collections.Generic;
using Client;
using UnityEngine;

[CreateAssetMenu(fileName ="UDPConfig", menuName ="ScriptableObjects/UDPConfig", order =1)]
public class UDPConfig : ScriptableObject
{
    public TRACKER_TYPE type;

    public string serverIP;
    public int serverPort;
    // public string clientIP;
    // public int clientPort;

    [SerializeField]
    private List<ClientIPPortTuple> _broadcaseClients;
    public IReadOnlyList<ClientIPPortTuple> BroadcastClients
    { get => _broadcaseClients; }
    public int bufferSize;

    public bool exclusiveAddressUse;
    public bool enableBroadcast;
    public bool dontFragment;
}