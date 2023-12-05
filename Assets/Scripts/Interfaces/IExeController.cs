using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.medcare360.utt
{
    public interface IExeController
    {
        void FindEXE();
        bool StartEXE(TRACKER_TYPE type);
        void StopEXE(TRACKER_TYPE type);
    }
}