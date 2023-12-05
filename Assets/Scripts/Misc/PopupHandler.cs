using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.medcare360.utt
{
    public class PopupHandler : MonoBehaviour
    {
        private static PopupHandler _instance;
        public static PopupHandler Instance
        {
            get => _instance;
            private set => _instance = value;
        }

        public List<Popup> popups;
        public Dictionary<MESSAGE_STATUS, Popup> popupLookup;

        private void InitializePopups()
        {
            popupLookup = new Dictionary<MESSAGE_STATUS, Popup>();
            popups = new List<Popup>(FindObjectsOfType<Popup>());
            foreach(Popup popup in popups)
            {
                popup.Initialize();
                popupLookup.Add(popup.popupType, popup);
            }
        }

        public void ShowPopup(string message, MESSAGE_STATUS status)
        {
            popupLookup[status].Show(message);
        }

        // Start is called before the first frame update
        void Start()
        {
            InitializePopups();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}