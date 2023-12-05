using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.medcare360.utt;
public class GOController : MonoBehaviour
{
    public List<GameObject> gameObjects;
    public GameObject pointer, fiducial;

    // Start is called before the first frame update
    void Start()
    {
        ClearGO();
    }

    private void ClearGO()
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].SetActive(false);
        }
        fiducial.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.targetFrameRate != ProcessManager.Instance.targetFps)
        {
            Application.targetFrameRate = ProcessManager.Instance.targetFps;
        }

        if (ProcessManager.Instance.markers.Count == 0)
        {
            ClearGO();
            return;
        }

        bool pointerFound = false;
        for(int i = 0; i < ProcessManager.Instance.markers.Count; i++)
        {
            if(i < gameObjects.Count)
            {
                gameObjects[i].SetActive(true);
                SkeletonMarker marker = ProcessManager.Instance.markers[i];
                gameObjects[i].transform.rotation = new Quaternion(
                    marker.qx, marker.qy, marker.qz, marker.qr);

                gameObjects[i].transform.position = new Vector3(
                    marker.x / 100, marker.y / 100, marker.z / 100);

                if(marker.markerSeriesNumber == 20)
                {
                    pointer = gameObjects[i];
                    pointerFound = true;
                }
            }
        }

        if(!pointerFound)
        {
            pointer = null;
        }

        if(ProcessManager.Instance.markers.Count < gameObjects.Count)
        {
            for(int i = ProcessManager.Instance.markers.Count; i < gameObjects.Count; i++)
            {
                gameObjects[i].SetActive(false);
            }
        }

        Fiducial fid = ProcessManager.Instance.orphanFiducial;
        if(fid == null || !fid.valid)
        {
            fiducial.SetActive(false);
        }
        else
        {
            fiducial.SetActive(true);
            fiducial.transform.position = new Vector3(fid.x / 100, fid.y / 100, fid.z / 100);
            fiducial.transform.rotation = Quaternion.identity;
        }
    }
}
