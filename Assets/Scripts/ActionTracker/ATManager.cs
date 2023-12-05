using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.medcare360.utt
{
    public class ATManager : MonoBehaviour
    {
        public GOController goCController;

        #region Hard Coded Values
        [SerializeField]
        private int _htId = 20;
        #endregion

        private int _actionTrackerId;   //Movable tracker
        private int _hostTrackerId;     //Host tracker on which the action tracker moves

        [Header("Translation Options")]
        #region Translation
        public float thresholdTranslation; //4
        public bool ignoreTranslation;
        #endregion

        [Header("Rotation Options")]
        #region Rotation
        public float thresholdRotation;
        public float LowerRotationLimit
        {
            get { return thresholdRotation * 1.5f; }
        }

        public float UpperRotationLimit
        {
            get { return LowerRotationLimit + 5; }
        }

        #endregion

        [Header("Release Checks")]
        #region Release Checks
        public bool translationReleaseCheck;
        public bool rotationReleaseCheck;
        #endregion

        [Header("Keyboard Action Emulation")]
        public ActionEmulator actionEmulator;

        [Header("Offsets")]
        #region Offsets
        /// <summary>
        /// The position on the Pointer (Host) from which
        /// translation difference between this and Action tracker is calculated
        /// </summary>
        public Vector3 offset;
        #endregion

        private bool HostPresent
        {
            get
            {
                return ProcessManager.Instance.geomLookup.ContainsKey(_hostTrackerId);
            }
        }

        /* [Header("Tracker Dropdown")]
        public TMP_Dropdown hostOptions;
        public TMP_Dropdown actionTrackerOptions;


        public int[] trackerIDs; */

        /* private void InitializeTrackerIds()
        {
            string hostOption = hostOptions.options[hostOptions.value].text;
            string atOption = actionTrackerOptions.options[actionTrackerOptions.value].text;

            try
            {
                _htId = int.Parse(hostOption);
            }
            catch
            {
                _htId = -1;
            }

            try
            {
                _atId = int.Parse(atOption);
            }
            catch
            {
                _atId = -1;
            }

            InitializeTrackerIds(_atId, _htId);
        } */

        public void InitializeTrackerIds(int atId, int htId)
        {
            //_actionTrackerId = atId;
            _hostTrackerId = htId;
        }

        private void OnHostOptionChanged(int option)
        {

        }

        private void OnActionOptionChanged(int option)
        {

        }

        #region Remove After implementing ToolLot
        private void Start()
        {
            ignoreTranslation = false;
            translationReleaseCheck = false;
            rotationReleaseCheck = false;
            //InitializeDropdowns();
            //InitializeTrackerIds(_atId, _htId);
            actionEmulator.Initialize();
        }
        #endregion

        public void GetTrackerPair()
        {
            throw new System.NotImplementedException("Implement GetTrackerPair");
        }

        #region Get Position and Rotations

        public float hostPosOffset;

        public bool GetHostPosition(out Vector3 pos)
        {
            try
            {
                SkeletonMarker marker = ProcessManager.Instance
                    .geomLookup[_hostTrackerId];

                Vector3 direction = new Quaternion(marker.qx, marker.qy, marker.qz, marker.qr) * Vector3.right;
                Vector3 dirOffset = direction * hostPosOffset;
                Vector3 markerPos = new Vector3(marker.x, marker.y, marker.z);
                pos = dirOffset + markerPos;
                return true;
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.Message);
                pos = Vector3.zero;
                return false;
            }
        }

        public bool GetHostRotation(out Quaternion rot)
        {
            try
            {
                SkeletonMarker marker = ProcessManager.Instance
                    .geomLookup[_hostTrackerId];
                rot = new Quaternion(marker.qx, marker.qy, marker.qz, marker.qr);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                rot = Quaternion.identity;
                return false;
            }
        }

        public bool GetActionPosition(out Vector3 pos)
        {
            try
            {
                Fiducial fiducial = ProcessManager.Instance.orphanFiducial;
                pos = new Vector3(fiducial.x, fiducial.y, fiducial.z);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                pos = Vector3.zero;
                return false;
            }
        }
        #endregion

        private void CheckPositionAndSimulate(Vector3 hostPos, Vector3 actionPos,
            Quaternion hostRot)
        {
            Vector3 diff = actionPos - hostPos;
            Quaternion reverse = Quaternion.Inverse(hostRot);
            Vector3 originalOffset = reverse * diff;
            offset = new Vector3(originalOffset.y, Mathf.Abs(originalOffset.x), originalOffset.z);
            #region TODO
            float yDiff = offset.y;
            float xDiff = offset.x;
            float absXDiff = Mathf.Abs(xDiff);

            if (absXDiff < thresholdRotation && !ignoreTranslation)
            {
                if (translationReleaseCheck)
                {
                    if (yDiff >= 0 && yDiff <= 1.5f)
                    {
                        translationReleaseCheck = false;
                    }
                }
                else
                {
                    if (yDiff >= thresholdTranslation - 1 && yDiff <= thresholdTranslation + 1)
                    {
                        translationReleaseCheck = true;
                        actionEmulator.SimulateEnter();
                    }
                }
            }
            else
            {

                if (absXDiff >= 5)// xDiff <= -5 || xDiff >= 5)
                {
                    ignoreTranslation = true;
                }
                else
                {
                    ignoreTranslation = false;
                }

                if (rotationReleaseCheck)
                {
                    if (absXDiff <= 5)// xDiff >= -5 && xDiff <= 5)
                    {
                        rotationReleaseCheck = false;
                        ignoreTranslation = false;
                    }
                }
                else
                {
                    if (xDiff < -LowerRotationLimit && xDiff > -UpperRotationLimit)
                    {
                        rotationReleaseCheck = true;
                        actionEmulator.SimulateEsc();
                    }
                    else if (xDiff > LowerRotationLimit && xDiff < UpperRotationLimit)
                    {
                        rotationReleaseCheck = true;
                        actionEmulator.SimulateRightArrow();
                    }
                }
            }
            #endregion
        }
             

        private bool TrackersPresent()
        {

            if (ProcessManager.Instance.geomLookup == null)
                return false;

            if (!HostPresent)
                return false;

            if (ProcessManager.Instance.orphanFiducial == null)
                return false;

            if (!ProcessManager.Instance.orphanFiducial.valid)
                return false;

            return true;
        }

        private void CheckActions()
        {
            if (!TrackersPresent())
                return;

            bool hostPosSuccess = GetHostPosition(out Vector3 hostPos);
            bool hostRotSuccess = GetHostRotation(out Quaternion hostRot);

            bool actionPosSuccess = GetActionPosition(out Vector3 actionPos);

            if(hostPosSuccess && actionPosSuccess && hostRotSuccess)
            {
                if (goCController.pointer == null)
                    return;

                if (goCController.fiducial == null)
                    return;

                //currentTranslation = Vector3.Distance(hostPos, actionPos);
                
                CheckPositionAndSimulate(hostPos, actionPos, hostRot);
            }
        }

        private bool AreOptionsValid()
        {
            return _hostTrackerId != -1 && _actionTrackerId != -1;
        }

        private void UpdateActionFiducial()
        {
            Fiducial[] fiducials = ProcessManager.Instance.fiducials;

            if (fiducials == null)
            {
                return;
            }

            int len = fiducials.Length;

            if(len == 0)
            {
                return;
            }

            if(!HostPresent)
            {
                return;
            }


            float minDist = 20f;
            float minOffset = 20f;
            Fiducial minFiducial = null;

            for(int i = 0; i < len; i++)
            {
                Fiducial fiducial = fiducials[i];

                if(fiducial == null)
                {
                    continue;
                }
                /*if(fiducial.probability < 0.7f)
                {
                    continue;
                }*/
                if(!fiducial.valid)
                {
                    continue;
                }

                Vector3 fidPosition = new Vector3(fiducial.x, fiducial.y, fiducial.z);

                GetHostPosition(out Vector3 hostPos);
                GetHostRotation(out Quaternion hostRot);

                Vector3 offset = fidPosition - hostPos;
                Quaternion reverse = Quaternion.Inverse(hostRot);
                Vector3 originalOffset = reverse * offset;

                float distance = Vector3.Distance(fidPosition, hostPos);
                float absOffset = Mathf.Abs(originalOffset.x);
                if (absOffset < minOffset && distance < minDist)
                {
                    minDist = distance;
                    minOffset = absOffset;
                    minFiducial = fiducial;
                }
            }

            ProcessManager.Instance.orphanFiducial = minFiducial;

            bool isValid = (minFiducial == null)? false : minFiducial.valid;
            _actionTrackerId = isValid ? (int)minFiducial.index : -1;
        }

        private void Update()
        {
            UpdateActionFiducial();
            if (!AreOptionsValid())
                return;
            CheckActions();
        }

        private void Awake()
        {
            _hostTrackerId = 20;
        }

        /* public GameObject object1, object2;

        private void TestSomething()
        {
            Vector3 pos1 = object1.transform.position;
            Vector3 pos2 = object2.transform.position;

            Quaternion rot2 = object2.transform.rotation;

            Vector3 offset = pos1 - pos2;
            Quaternion reverse = Quaternion.Inverse(rot2);
            Vector3 originalOffset = reverse * offset;


            testOffset = originalOffset;
        } */
    }
}
