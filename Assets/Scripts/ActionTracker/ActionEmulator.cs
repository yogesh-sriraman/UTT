using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace com.medcare360.utt
{
    public class ActionEmulator : MonoBehaviour
    {
        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk,
            byte bScan, int dwFlags, int dwExtraInfo);

        IntPtr cas360handle;

        private void Start()
        {
            cas360handle = IntPtr.Zero;
        }

        public void Initialize()
        {
            FindCas();
        }

        public void BringCasToForeground()
        {
            if (cas360handle == IntPtr.Zero)
            {
                FindCas();
                if (cas360handle == IntPtr.Zero)
                {
                    UnityEngine.Debug.LogError("CAS process cannot be found");
                    return;
                }
            }
            
            SetForegroundWindow(cas360handle);
        }

        public void FindCas()
        {
            try
            {
                Process cas360 = Process.GetProcessesByName("360CAS").FirstOrDefault();
                cas360handle = cas360.MainWindowHandle;
            }
            catch
            {
                UnityEngine.Debug.LogError("Could not find CAS");
                cas360handle = IntPtr.Zero;
            }
        }

        private void MakeKeyboardEvent(byte key)
        {
            keybd_event(key, 0, 1 | 0, 0); // 1|0 simulates key pressed
            keybd_event(key, 0, 1 | 2, 0); // 1|2 simulates key released
        }

        public void SimulateEsc()
        {
            MakeKeyboardEvent(KeyCodes.KC_ESC);
            /*keybd_event(KeyCodes.KC_ESC, 0, 1 | 0, 0);
            keybd_event(KeyCodes.KC_ESC, 0, 1 | 2, 0);*/
        }

        public void SimulateRightArrow()
        {
            MakeKeyboardEvent(KeyCodes.KC_R_ARROW);

            /*keybd_event(KeyCodes.KC_R_ARROW, 0, 1 | 0, 0);
            keybd_event(KeyCodes.KC_R_ARROW, 0, 1 | 2, 0);*/
        }

        public void SimulateEnter()
        {
            MakeKeyboardEvent(KeyCodes.KC_ENTER);
            /*keybd_event(KeyCodes.KC_ENTER, 0, 1 | 0, 0);
            keybd_event(KeyCodes.KC_ENTER, 0, 1 | 2, 0);*/
        }
    }
}