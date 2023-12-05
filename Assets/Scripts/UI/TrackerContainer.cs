using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace com.medcare360.utt
{
    public class TrackerContainer : MonoBehaviour
    {
        public TextMeshProUGUI geometry;
        public TextMeshProUGUI positionX;
        public TextMeshProUGUI positionY;
        public TextMeshProUGUI positionZ;
        public TextMeshProUGUI rotationX;
        public TextMeshProUGUI rotationY;
        public TextMeshProUGUI rotationZ;

        public void Initialize()
        {
            geometry = transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
            positionX = transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
            positionY = transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
            positionZ = transform.GetChild(3).GetComponentInChildren<TextMeshProUGUI>();
            rotationX = transform.GetChild(4).GetComponentInChildren<TextMeshProUGUI>();
            rotationY = transform.GetChild(5).GetComponentInChildren<TextMeshProUGUI>();
            rotationZ = transform.GetChild(6).GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetInfo(SkeletonMarker marker)
        {
            geometry.text = marker.markerSeriesNumber.ToString();

            positionX.text = marker.x.ToString();
            positionY.text = marker.y.ToString();
            positionZ.text = marker.z.ToString();

            Quaternion rotQ = new Quaternion(marker.qx, marker.qy, marker.qz, marker.qr);
            Vector3 eulerAngles = rotQ.eulerAngles;

            rotationX.text = eulerAngles.x.ToString();
            rotationY.text = eulerAngles.y.ToString();
            rotationZ.text = eulerAngles.z.ToString();
        }
    }
}
