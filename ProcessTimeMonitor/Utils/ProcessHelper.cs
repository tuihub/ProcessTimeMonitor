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
            if (process.HasExited)
            {
                Log.Warn("WaitForAllToExit", $"Process(PID = {process.Id}) has exited");
                return;
            }
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
                        Log.Debug("WaitForAllToExit", $"Wait for child process {childProcess.GetProcessNameEx()}(path = {item["ExecutablePath"]}, PID = {childProcess.Id}, parent PID = {process.Id}) to exit");
                        WaitForAllToExit(childProcess, true);
                        Log.Debug("WaitForAllToExit", $"Child process {childProcess.GetProcessNameEx()}(path = {item["ExecutablePath"]}, PID = {childProcess.Id}, parent PID = {process.Id}) exited");
                    }
                }
            }
            Log.Debug("WaitForAllToExit", $"Wait for process {process.GetProcessNameEx()}(PID = {process.Id}) to exit");
            process.WaitForExit();
            Log.Debug("WaitForAllToExit", $"Process {process.GetProcessNameEx()}(PID = {process.Id}) exited" + (isChildProcess ? "" : $" with exit code {process.ExitCode}"));
        }
        public static async Task WaitForAllToExitFullAsync(this Process process, bool isChildProcess = false)
        {
            if (process.HasExited)
            {
                Log.Warn("WaitForAllToExitFullAsync", $"Process(PID = {process.Id}) has exited");
                return;
            }
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "SELECT * " +
                "FROM Win32_Process " +
                "WHERE ParentProcessId=" + process.Id);
            ManagementObjectCollection collection = searcher.Get();
            var taskList = new List<Task>();
            var taskDict = new Dictionary<Task, Process>();
            if (collection.Count > 0)
            {
                foreach (var item in collection)
                {
                    UInt32 childProcessId = (UInt32)item["ProcessId"];
                    if ((int)childProcessId != Process.GetCurrentProcess().Id)
                    {
                        Process childProcess = Process.GetProcessById((int)childProcessId);
                        Log.Debug("WaitForAllToExitFullAsync", $"Wait for child process {childProcess.GetProcessNameEx()}(path = {item["ExecutablePath"]}, PID = {childProcess.Id}, parent PID = {process.Id}) to exit");
                        var childTask = WaitForAllToExitFullAsync(childProcess, true);
                        Log.Debug("WaitForAllToExitFullAsync", $"Adding task(Id = {childTask.Id}) to taskDict");
                        taskDict.Add(childTask, childProcess);
                        taskList.Add(childTask);
                    }
                }
            }
            Log.Debug("WaitForAllToExitFullAsync", $"Wait for process {process.GetProcessNameEx()}(PID = {process.Id}) to exit");
            var curMainProcessTask = process.WaitForExitAsync();
            Log.Debug("WaitForAllToExitFullAsync", $"Adding task(Id = {curMainProcessTask.Id}) to taskDict");
            taskDict.Add(curMainProcessTask, process);
            taskList.Add(curMainProcessTask);
            while (taskList.Count > 0)
            {
                Task task = await Task.WhenAny(taskList);
                Log.Debug("WaitForAllToExitFullAsync", $"task(Id = {task.Id}) is done");
                var curProcess = taskDict.GetValueOrDefault(task);
                if (curProcess == null)
                {
                    Log.Warn("WaitForAllToExitFullAsync", $"task(Id = {task.Id}) not exist");
                    taskList.Remove(task);
                    continue;
                }
                if (task == curMainProcessTask)
                {
                    Log.Debug("WaitForAllToExitFullAsync", $"Process {curProcess.GetProcessNameEx()}(PID = {curProcess.Id}) exited" + (isChildProcess ? "" : $" with exit code {curProcess.ExitCode}"));
                }
                else
                {
                    Log.Debug("WaitForAllToExitFullAsync", $"Child process {curProcess.GetProcessNameEx()}(PID = {curProcess.Id}, parent PID = {process.Id}) exited");
                }
                taskList.Remove(task);
            }
        }
        public static async Task WaitForAllToExitAsync(this Process process, bool isChildProcess = false)
        {
            if (process.HasExited)
            {
                Log.Warn("WaitForAllToExitAsync", $"Process(PID = {process.Id}) has exited");
                return;
            }
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
                        Log.Debug("WaitForAllToExitAsync", $"Wait for child process {childProcess.GetProcessNameEx()}(path = {item["ExecutablePath"]}, PID = {childProcess.Id}, parent PID = {process.Id}) to exit");
                        await WaitForAllToExitAsync(childProcess, true);
                        Log.Debug("WaitForAllToExitAsync", $"Child process {childProcess.GetProcessNameEx()}(path = {item["ExecutablePath"]}, PID = {childProcess.Id}, parent PID = {process.Id}) exited");
                    }
                }
            }
            Log.Debug("WaitForAllToExitAsync", $"Wait for process {process.GetProcessNameEx()}(PID = {process.Id}) to exit");
            await process.WaitForExitAsync();
            Log.Debug("WaitForAllToExitAsync", $"Process {process.GetProcessNameEx()}(PID = {process.Id}) exited" + (isChildProcess ? "" : $" with exit code {process.ExitCode}"));
        }
        public static string GetProcessNameEx(this Process process)
        {
            try
            {
                return process.ProcessName;
            }
            catch (InvalidOperationException e)
            {
                Log.Debug("GetProcessNameEx/InvalidOperationException", e.Message);
                Log.Debug("GetProcessNameEx/InvalidOperationException", e.StackTrace);
                return "null";
            }
            catch (Exception e)
            {
                Log.Debug("GetProcessNameEx/Exception", e.Message);
                Log.Debug("GetProcessNameEx/Exception", e.StackTrace);
                return "unknown";
            }
        }
    }
}
