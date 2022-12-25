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
        public static async Task WaitForAllToExitAsyncDebug(this Process process, bool isChildProcess = false)
        {
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
                        Log.Debug("WaitForAllToExitAsyncDebug", $"Wait for child process {childProcess.ProcessName}(path = {item["ExecutablePath"]}, PID = {childProcess.Id}, parent PID = {process.Id}) to exit");
                        var childTask = WaitForAllToExitAsyncDebug(childProcess, true);
                        Log.Debug("WaitForAllToExitAsyncDebug", $"Adding task(Id = {childTask.Id}) to taskIdDict");
                        taskDict.Add(childTask, childProcess);
                        taskList.Add(childTask);
                    }
                }
            }
            Log.Debug("WaitForAllToExitAsyncDebug", $"Wait for process {process.ProcessName}(PID = {process.Id}) to exit");
            var curMainProcessTask = process.WaitForExitAsync();
            Log.Debug("WaitForAllToExitAsyncDebug", $"Adding task(Id = {curMainProcessTask.Id}) to taskIdDict");
            taskDict.Add(curMainProcessTask, process);
            taskList.Add(curMainProcessTask);
            while (taskList.Count > 0)
            {
                Task task = await Task.WhenAny(taskList);
                Log.Debug("WaitForAllToExitAsyncDebug", $"task(Id = {task.Id}) is done");
                var curProcess = taskDict.GetValueOrDefault(task);
                if (curProcess == null)
                {
                    Log.Warn("WaitForAllToExitAsyncDebug", $"task(Id = {task.Id}) not exist");
                    taskList.Remove(task);
                    continue;
                }
                if (task == curMainProcessTask)
                {
                    Log.Debug("WaitForAllToExitAsyncDebug", $"Process {curProcess.ProcessName}(PID = {curProcess.Id}) exited" + (isChildProcess ? "" : $" with exit code {curProcess.ExitCode}"));
                }
                else
                {
                    Log.Debug("WaitForAllToExitAsyncDebug", $"Child process {curProcess.ProcessName}(PID = {curProcess.Id}, parent PID = {process.Id}) exited");
                }
                taskList.Remove(task);
            }
        }
        public static async Task WaitForAllToExitAsync(this Process process, bool isChildProcess = false)
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
                        Log.Debug("WaitForAllToExitAsync", $"Wait for child process {childProcess.ProcessName}(path = {item["ExecutablePath"]}, PID = {childProcess.Id}, parent PID = {process.Id}) to exit");
                        await WaitForAllToExitAsync(childProcess, true);
                        Log.Debug("WaitForAllToExitAsync", $"Child process {childProcess.ProcessName}(path = {item["ExecutablePath"]}, PID = {childProcess.Id}, parent PID = {process.Id}) exited");
                    }
                }
            }
            Log.Debug("WaitForAllToExitAsync", $"Wait for process {process.ProcessName}(PID = {process.Id}) to exit");
            await process.WaitForExitAsync();
            Log.Debug("WaitForAllToExitAsync", $"Process {process.ProcessName}(PID = {process.Id}) exited" + (isChildProcess ? "" : $" with exit code {process.ExitCode}"));
        }
    }
}
