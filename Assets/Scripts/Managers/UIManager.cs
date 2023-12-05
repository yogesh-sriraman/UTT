using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;
using Client;
using UnityEngine.Events;

namespace com.medcare360.utt
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private Sprite successStatus, errorStatus, msgStatus, inactiveStatus;

        [SerializeField]
        private TextMeshProUGUI dataReceptionText;

        [SerializeField]
        private TextMeshProUGUI informationDisplayText;

        [SerializeField]
        private Transform trackerMainContainer;

        [SerializeField]
        private TrackerContainer[] trackerInfoContainers;

        [SerializeField]
        private Button openCASBtn;

        [SerializeField]
        private Button settingsBtn;

        public bool displayCleared;

        public void Initialize(UnityAction action)
        {
            int numTrackerContainers = trackerMainContainer.childCount;
            trackerInfoContainers = new TrackerContainer[numTrackerContainers];

            dataReceptionText.text = "Select Tracker";
            informationDisplayText.text = "";
            for(int i = 0; i < numTrackerContainers; i++)
            {
                trackerInfoContainers[i] = trackerMainContainer.GetChild(i)
                    .GetComponent<TrackerContainer>();
                trackerInfoContainers[i].Initialize();
                trackerInfoContainers[i].gameObject.SetActive(false);
            }
            displayCleared = true;
        }

        public void HideResetButton()
        {
            throw new System.NotImplementedException();
            //background.raycastTarget = false;
            //reconnectBtn.gameObject.SetActive(false);
        }

        private void ClearDisplay()
        {
            if(displayCleared)
            {
                return;
            }

            for(int i = 0; i < trackerInfoContainers.Length; i++)
            {
                trackerInfoContainers[i].gameObject.SetActive(false);
            }
            displayCleared = true;
        }

        public void DisplayTrackerInfo(List<SkeletonMarker> markers = null)
        {
            ClearDisplay();

            if (markers == null || markers.Count == 0)
                return;

            displayCleared = false;
            for (int i = 0; i < markers.Count; i++)
            {
                TrackerContainer newTrackerContainer = trackerInfoContainers[i];
                newTrackerContainer.gameObject.SetActive(true);

                UpdateContainerContent(newTrackerContainer, markers[i]);
            }
        }

        private void UpdateContainerContent(TrackerContainer container, SkeletonMarker marker)
        {
            container.SetInfo(marker);
        }

        public void UpdateDataReception(string msg)
        {
            dataReceptionText.text = msg;
            dataReceptionText.color = ApplicationConfig.messageColor[MESSAGE_STATUS.INIT];

            openCASBtn.image.sprite = inactiveStatus;
            openCASBtn.interactable = false;
        }

        public void UpdateDataReception(bool success)
        {
            if(success)
            {
                dataReceptionText.text = "Receiving Data";
                dataReceptionText.color = ApplicationConfig.messageColor[MESSAGE_STATUS.SUCCESS];
                

                openCASBtn.image.sprite = successStatus;
                openCASBtn.interactable = true;
            }
            else
            {
                dataReceptionText.text = "No Device Found";
                dataReceptionText.color = ApplicationConfig.messageColor[MESSAGE_STATUS.ERROR];


                openCASBtn.image.sprite = inactiveStatus;
                openCASBtn.interactable = false;
            }
        }

        public void UpdateDataBroadcast(bool success, IReadOnlyList<ClientIPPortTuple> clients = null,
            string msg = null)
        {
            MESSAGE_STATUS status = MESSAGE_STATUS.DEFAULT;
            if(success)
            {
                status = MESSAGE_STATUS.NORMAL;
                msg = "Sending data to\n";//\nIP - 127.0.0.1\nPort - 61115";
                int i = 1;
                foreach(ClientIPPortTuple client in clients)
                {
                    msg += $"{i++}. IP : {client.clientIP} - Port : {client.clientPort}\n";
                }
            }
            else
            {
                status = MESSAGE_STATUS.ERROR;
                //msg = "";
            }

            ShowInformation(msg, status);
        }

        public void ShowInformation(string msg, MESSAGE_STATUS status)
        {
            informationDisplayText.text = msg;
            informationDisplayText.color = ApplicationConfig.messageColor[status];
        }

        public void ResetDataReception()
        {
            dataReceptionText.text = "Select Tracker";
            dataReceptionText.color = ApplicationConfig.messageColor[MESSAGE_STATUS.NORMAL];
            
        }

        public void OnClickCloseBtn()
        {
            ProcessManager.Instance.CloseApplication();
        }

        public void UpdateHostTrackerOptions()
        {

        }

        #region SettingsButton
        public PopupHandler popupHandler;
        public void OnClickSettingsBtn()
        {
            popupHandler.ShowPopup("Settings", MESSAGE_STATUS.NORMAL);
        }
        #endregion
    }
}