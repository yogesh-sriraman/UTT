using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.medcare360.utt
{
    public class UpdateCaller : SingletonManager<UpdateCaller>
    {
        public static System.Action OnUpdate;

        // Update is called once per frame
        void Update()
        {
            if (Application.targetFrameRate != ProcessManager.Instance.targetFps)
            {
                Application.targetFrameRate = ProcessManager.Instance.targetFps;
            }

            if (OnUpdate != null)
                OnUpdate();
        }
    }
}