using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;
using UnityEngine.XR;

public class TestKeySend : MonoBehaviour
{
    [DllImport("user32.dll")]
    public static extern int SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern void keybd_event(byte bVk,
        byte bScan, int dwFlags, int dwExtraInfo);

    IntPtr cas360handle;

    public bool start;

    private void Start()
    {
        start = false;
        Process cas360 = Process.GetProcessesByName("notepad").FirstOrDefault();
        cas360handle = cas360.MainWindowHandle;
        StartCoroutine(SimulateKeyStrokes());
    }

    private void KeepWriting(byte val)
    {
        keybd_event(val, 0, 1 | 0, 0);
        keybd_event(val, 0, 1 | 2, 0);
        keybd_event(0x0D, 0, 1 | 0, 0);
        keybd_event(0x0D, 0, 1 | 2, 0);
    }

    private void Update()
    {
    }

    private IEnumerator SimulateKeyStrokes()
    {
        while(true)
        {
            if(start)
            {
                KeepWriting(KeyCodes.KC_A_KEY);
                yield return new WaitForSeconds(1);
            }
            yield return null;
        }
    }

    public void OnClickSimulateF2()
    {
        SetForegroundWindow(cas360handle);
        keybd_event(0x71, 0, 1 | 0, 0);
        keybd_event(0x71, 0, 1 | 2, 0);
    }

    public void SimulateEsc()
    {
        SetForegroundWindow(cas360handle);
        keybd_event(0x1B, 0, 1 | 0, 0);
        keybd_event(0x1B, 0, 1 | 2, 0);
    }

    public void SimulateRightArrow()
    {
        SetForegroundWindow(cas360handle);
        keybd_event(0x27, 0, 1 | 0, 0);
        keybd_event(0x27, 0, 1 | 2, 0);
    }

    public void SimulateEnter()
    {
        SetForegroundWindow(cas360handle);
        keybd_event(0x0D, 0, 1 | 0, 0);
        keybd_event(0x0D, 0, 1 | 2, 0);
    }
}
