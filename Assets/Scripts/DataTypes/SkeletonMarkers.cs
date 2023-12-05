[System.Serializable]
public class SkeletonMarker
{
    public int index;
    public int markerSeriesNumber;
    public float markerWidth;
    public float markerThickness;
    public float brightness;
    public float focusMetric;
    public float blurRadius;
    public float x, y, z;
    public float qr, qx, qy, qz;
    public uint flags;

    public SkeletonMarker()
    {
        index = -1;
        markerSeriesNumber = -1;
        markerWidth = 0;
        markerThickness = 0;
        brightness = 0;
        focusMetric = 0;
        blurRadius = 0;
        x = 0;
        y = 0;
        z = 0;
        qr = 0;
        qx = 0;
        qy = 0;
        qz = 0;
        flags = 0;
    }

    public SkeletonMarker(
            int index = 0,
            int markerSeriesNumber = 0,
            float markerWidth = 0,
            float markerThickness = 0,
            float brightness = 0,
            float focusMetric = 0,
            float blurRadius = 0,
            float x = 0,
            float y = 0,
            float z = 0,
            float qr = 0,
            float qx = 0,
            float qy = 0,
            float qz = 0,
            uint flags = 0)
    {
        // Decode marker identification
        this.index = index;
        this.markerSeriesNumber = markerSeriesNumber;
        this.markerWidth = markerWidth;
        this.markerThickness = markerThickness;
        this.brightness = brightness;
        this.focusMetric = focusMetric;
        this.blurRadius = blurRadius;

        // Decode marker position
        this.x = x;
        this.y = y;
        this.z = z;
        this.qr = qr;
        this.qx = qx;
        this.qy = qy;
        this.qz = qz;

        // Decode marker flags
        this.flags = flags;

        /*this.pos3 = new UnityEngine.Vector3(X, Y, Z);
        this.rotQ = new UnityEngine.Quaternion(qx, qy, qz, qr);*/
    }

    public SkeletonMarker(SkeletonMarker skeletonMarker)
    {
        index = skeletonMarker.index;
        markerSeriesNumber = skeletonMarker.markerSeriesNumber;
        markerWidth = skeletonMarker.markerWidth;
        markerThickness = skeletonMarker.markerThickness;
        brightness = skeletonMarker.brightness;
        focusMetric = skeletonMarker.focusMetric;
        blurRadius = skeletonMarker.blurRadius;
        x = skeletonMarker.x;
        y = skeletonMarker.y;
        z = skeletonMarker.z;
        qr = skeletonMarker.qr;
        qx = skeletonMarker.qx;
        qy = skeletonMarker.qy;
        qz = skeletonMarker.qz;
        flags = skeletonMarker.flags;

    }
}
