using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuiHub.ProcessTimeMonitorLibrary
{
    public partial class ProcessTimeMonitor<T>
    {
        public async Task WaitForProcToExit(string name, string? path = null, int sleepCountLimit = 10, int sleepMilliseconds = 1000)
        {
            _logger?.LogDebug("WaitForProcToExit", $"name = {name}, path = {(path == null ? "null" : path)}");
            Process[] processes = Process.GetProcessesByName(name);
            int sleepCount = 0;
            while (processes.Length == 0)
            {
                if (sleepCount > sleepCountLimit)
                {
                    _logger?.LogWarning("WaitForProcToExit", $"Sleep time limit exceeded, exiting.");
                    throw new TimeoutException($"Sleep time(sleepCountLimit = {sleepCountLimit}, sleepMilliseconds = {sleepMilliseconds}) limit exceeded.");
                }
                _logger?.LogDebug("WaitForProcToExit", $"processes.Length == 0, sleeping for {sleepMilliseconds}ms({sleepCount} / {sleepCountLimit}).");
                sleepCount++;
                Thread.Sleep(sleepMilliseconds);
                processes = Process.GetProcessesByName(name);
                if (path != null)
                    processes = processes.Where(x => x.MainModule != null && x.MainModule.FileName == path).ToArray();
            }
            foreach (var process in processes)
                _logger?.LogDebug("WaitForProcToExit/GetProcessesByName", $"process [id = {process.Id}, name = {process.ProcessName}, path = {(process.MainModule == null ? "null" : process.MainModule.FileName)}]");
            _logger?.LogDebug("WaitForProcToExit", $"path = {(path == null ? "null" : path)}");
            if (path != null)
            {
                processes = processes.Where(x => x.MainModule != null && x.MainModule.FileName == path).ToArray();
                foreach (var process in processes)
                    _logger?.LogDebug("WaitForProcToExit/GetProcessesByNameFiltered", $"process [id = {process.Id}, name = {process.ProcessName}, path = {(process.MainModule == null ? "null" : process.MainModule.FileName)}]");
            }
            foreach (var process in processes)
            {
                _logger?.LogDebug("WaitForProcToExit", $"Wait for process[id = {process.Id}, name = {process.ProcessName}, path = {(process.MainModule == null ? "null" : process.MainModule.FileName)}] to exit.");
                await process.WaitForExitAsync();
            }
        }
    }
}
