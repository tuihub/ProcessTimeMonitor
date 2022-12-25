using SavedataManager.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ProcessTimeMonitor.Utils
{
    public static class ProcessHelper
    {
        // from https://stackoverflow.com/questions/7189117/find-all-child-processes-of-my-own-net-process-find-out-if-a-given-process-is
        public static void WaitForAllToExit(this Process process, bool isChildProcess = false)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "SELECT * " +
                "FROM Win32_Process " +
                "WHERE ParentProcessId=" + process.Id);
            ManagementObjectCollection collection = searcher.Get();
            if (collection.Count > 0)
            {
                foreach (var item in collection)
                {
                    UInt32 childProcessId = (UInt32)item["ProcessId"];
                    if ((int)childProcessId != Process.GetCurrentProcess().Id)
                    {
                        Process childProcess = Process.GetProcessById((int)childProcessId);
                        Log.Debug("WaitForAllToExit", $"Wait for child process {childProcess.ProcessName}(path = {item["ExecutablePath"]}, PID = {childProcess.Id}, parent PID = {process.Id}) to exit");
                        WaitForAllToExit(childProcess, true);
                        Log.Debug("WaitForAllToExit", $"Child process {childProcess.ProcessName}(path = {item["ExecutablePath"]}, PID = {childProcess.Id}, parent PID = {process.Id}) exited");
                    }
                }
            }
            Log.Debug("WaitForAllToExit", $"Wait for process {process.ProcessName}(PID = {process.Id}) to exit");
            process.WaitForExit();
            Log.Debug("WaitForAllToExit", $"Process {process.ProcessName}(PID = {process.Id}) exited" + (isChildProcess ? "" : $" with exit code {process.ExitCode}"));
        }
    }
}
