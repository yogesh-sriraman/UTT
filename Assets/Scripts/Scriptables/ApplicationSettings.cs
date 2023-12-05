using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace com.medcare360.utt
{
    [CreateAssetMenu(fileName = "AppSettings",
            menuName = "ScriptableObjects/AppSettings", order = 1)]
    public class ApplicationSettings : ScriptableObject
    {
        public UnityAction OnVersionChanged;

        [SerializeField]
        private string _version = "v1.2-BETA";
        public string Version
        {
            get => _version;
            set
            {
                if (_version == value)
                    return;

                _version = value;
                OnVersionChanged?.Invoke();
            }
        }

        public string pathToCAS = "C:\\Users\\yogesh.sriraman\\Desktop\\360CAS Hip v2.1\\360CAS Hip v2.1\\360CAS.exe";
    }
}