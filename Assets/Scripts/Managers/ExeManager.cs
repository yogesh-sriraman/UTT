using com.medcare360.utt;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;


namespace com.medcare360.utt
{
    public class ExeManager : IExeController
    {

        private string exeFilePath;

        public Process atracsysHandle;

        public void FindEXE()
        {
            string ettParent = Directory.GetParent(Application.dataPath).FullName;


            string exeFileAbsPath = Directory.GetFiles(ettParent,
                    "AtracsysConn.exe", SearchOption.AllDirectories).FirstOrDefault();

            exeFilePath = Path.GetRelativePath(ettParent, exeFileAbsPath);
        }

        private bool IsPathValid()
        {
            if (exeFilePath == null || exeFilePath == "")
                return false;

            if (exeFilePath.Trim() == "")
                return false;

            if (!File.Exists(exeFilePath))
                return false;

            string extension = Path.GetExtension(exeFilePath).Trim();

            return (extension == ".exe");
        }

        private void CleanupExistingProcess()
        {
            Process runningProcess = Process.GetProcessesByName("AtracsysConn")
                .FirstOrDefault();

            if (runningProcess == null)
                return;

            if (atracsysHandle == runningProcess)
                return;

            runningProcess.Kill();
        }

        public bool StartEXE(TRACKER_TYPE type)
        {
            CleanupExistingProcess();
            if (atracsysHandle == null)
            {
                atracsysHandle = new Process();
            }
            atracsysHandle.StartInfo.UseShellExecute = true;
            atracsysHandle.StartInfo.CreateNoWindow = true;
            atracsysHandle.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            atracsysHandle.StartInfo.FileName = Application.dataPath + "\\AtracsysExe\\Plugins\\AtracsysConn.exe";
            return atracsysHandle.Start();
        }

        public void StopEXE(TRACKER_TYPE type)
        {
            if (atracsysHandle == null)
                return;

            if (atracsysHandle.HasExited)
                return;

            atracsysHandle.Kill();
        }
    }
}