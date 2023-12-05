using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace com.medcare360.utt
{
    public class SettingsItem : MonoBehaviour
    {
        public AppSettings settingsType;

        public TextMeshProUGUI settingsName;
        public TMP_InputField inputField;

        public ApplicationSettings appSettings;

        public void UpdateItem()
        {
            switch(settingsType)
            {
                case AppSettings.Version:
                    appSettings.Version = inputField.text;
                    break;
                case AppSettings.CASPath:
                    appSettings.pathToCAS = inputField.text;
                    break;
            }
        }

        public void Initialize(ApplicationSettings applicationSettings)
        {
            appSettings = applicationSettings;
            settingsName.text = settingsType.ToString();
            switch(settingsType)
            {
                case AppSettings.Version:
                    inputField.text = appSettings.Version;
                    break;
                case AppSettings.CASPath:
                    inputField.text = appSettings.pathToCAS;
                    break;
            }
        }
    }
}