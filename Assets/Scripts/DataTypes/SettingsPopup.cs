using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace com.medcare360.utt
{
    public class SettingsPopup : Popup
    {
        public List<SettingsItem> items;

        public ApplicationSettings appSettings;

        public override void Initialize(UnityAction action = null)
        {
            base.Initialize(OnOkBtnClicked);
            InitializeItems();
        }

        private void InitializeItems()
        {
            items = new List<SettingsItem>(GetComponentsInChildren<SettingsItem>());
            foreach(SettingsItem item in items)
            {
                item.Initialize(appSettings);
            }
        }

        public void OnOkBtnClicked()
        {
            foreach(SettingsItem item in items)
            {
                item.UpdateItem();   
            }
        }
    }
}