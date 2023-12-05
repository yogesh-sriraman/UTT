using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace com.medcare360.utt
{
    public enum MESSAGE_STATUS
    {
        DEFAULT,
        ERROR,
        WARNING,
        NORMAL,
        INIT,
        SUCCESS,

        INVALID,
        COUNT
    }

    public static class ApplicationConfig
    {
        public static Dictionary<TRACKER_TYPE, string> devicePath;

        public static Dictionary<MESSAGE_STATUS, Color> messageColor =
            new Dictionary<MESSAGE_STATUS, Color>()
            {
                { MESSAGE_STATUS.DEFAULT, Color.white},
                { MESSAGE_STATUS.NORMAL, Color.white},
                { MESSAGE_STATUS.INVALID, Color.grey},
                { MESSAGE_STATUS.ERROR, Color.red},
                { MESSAGE_STATUS.WARNING, Color.yellow},
                { MESSAGE_STATUS.INIT, Color.white},
                { MESSAGE_STATUS.SUCCESS, Color.green}
            };

        //"v1.2-BETA"

        /*public static UnityAction OnVersionChanged;
        private static string _version;// = "v1.2-BETA";
        public static string Version
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

        public static string pathToCAS;// = "C:\\Users\\yogesh.sriraman\\Desktop\\360CAS Hip v2.1\\360CAS Hip v2.1\\360CAS.exe";*/
    }
}