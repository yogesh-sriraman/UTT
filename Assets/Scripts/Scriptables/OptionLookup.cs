using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.medcare360.utt
{
    [CreateAssetMenu(fileName = "OptionLoopup",
        menuName = "ScriptableObjects/TrackerOptions", order = 1)]
    public class OptionLookup : ScriptableObject
    {
        public TrackerOption[] trackerOptions;
    }

    [System.Serializable]
    public class TrackerOption
    {
        public int optionValue;
        public TRACKER_TYPE optionType;
    }
}