/// <summary>
/// Parent Class. Used as a generic skeleton to serialize and send data to CAS.
/// </summary>
[System.Serializable]
public class Skeleton
{
    
    public int version;
    public string hostname;
    public int frameNumber;
    public uint frameTime_sec, frameTime_nsec;
    public uint irqSequenceNum;
    public uint irqTime_sec, irqTime_nsec;

    /*public bool valid;
    public TRACKER_TYPE tType;*/

    public SkeletonMarker[] aMarkers;
    public Fiducial[] fiducials;
    public double[] aVoltages;

    #region CONSTRUCTORS
    public Skeleton()
    {
        version = 0;
        hostname = "";
        frameNumber = 0;
        frameTime_sec = 0;
        frameTime_nsec = 0;
        irqSequenceNum = 0;
        irqTime_sec = 0;
        irqTime_nsec = 0;
        aMarkers = null;
        fiducials = null;
        aVoltages = null;
    }

    public Skeleton(int version, string hostname, int frameNumber, uint frameTimeInSeconds,
        uint frameTimeInNanoSeconds, uint irqSequenceNumber, uint irqTimeInSeconds,
        uint irqTimeNanoSeconds, SkeletonMarker[] markerPositions, double[] voltages)
    {
        this.version = version; //Using "this" keyword since the argument name
                                //and member variable
        this.hostname = hostname;
        this.frameNumber = frameNumber;

        frameTime_sec = frameTimeInSeconds;
        frameTime_nsec = frameTimeInNanoSeconds;
        irqSequenceNum = irqSequenceNumber;
        irqTime_sec = irqTimeInSeconds;
        irqTime_nsec = irqTimeNanoSeconds;
        aMarkers = markerPositions;
        aVoltages = voltages;
    }
    #endregion
}