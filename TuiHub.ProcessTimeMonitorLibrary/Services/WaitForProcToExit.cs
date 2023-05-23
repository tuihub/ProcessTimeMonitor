using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuiHub.ProcessTimeMonitorLibrary
{
    public partial class ProcessTimeMonitor
    {
        public async Task<(DateTime start, DateTime end, int exitCode)> WaitForProcToExit(string name, string? path = null, DateTime? startDT = null, int sleepCountLimit = 10, int sleepMilliseconds = 1000)
        {
            startDT ??= DateTime.MinValue;
            _logger?.LogDebug($"name = {name}, path = {(path == null ? "null" : path)}");
            // must use array, otherwise may cause "Only part of a ReadProcessMemory or WriteProcessMemory request was completed" exception
            var processes = Process.GetProcessesByName(name).Where(x => x.StartTime >= startDT).ToArray();
            if (path != null)
                processes = processes.Where(x => x.MainModule != null && x.MainModule.FileName == path).ToArray();
            int sleepCount = 0;
            while (processes.Count() == 0)
            {
                if (sleepCount > sleepCountLimit)
                {
                    _logger?.LogWarning($"Sleep time limit exceeded, exiting.");
                    throw new TimeoutException($"Sleep time(sleepCountLimit = {sleepCountLimit}, sleepMilliseconds = {sleepMilliseconds}) limit exceeded.");
                }
                _logger?.LogDebug($"processes.Length == 0, sleeping for {sleepMilliseconds}ms({sleepCount} / {sleepCountLimit}).");
                sleepCount++;
                await Task.Delay(sleepMilliseconds);
                processes = Process.GetProcessesByName(name).Where(x => x.StartTime >= startDT).ToArray();
                if (path != null)
                    processes = processes.Where(x => x.MainModule != null && x.MainModule.FileName == path).ToArray();
            }
            foreach (var process in processes)
                _logger?.LogDebug($"GetProcessesByName, process [id = {process.Id}, name = {process.ProcessName}, path = {(process.MainModule == null ? "null" : process.MainModule.FileName)}]");
            var start = DateTime.Now;
            foreach (var process in processes)
            {
                _logger?.LogDebug($"Wait for process[id = {process.Id}, name = {process.ProcessName}, path = {(process.MainModule == null ? "null" : process.MainModule.FileName)}] to exit.");
                await process.WaitForExitAsync();
            }
            var end = DateTime.Now;
            int? exitCode = null;
            foreach (var process in processes)
            {
                if (exitCode == null || (exitCode == 0 && process.ExitCode != 0))
                    exitCode ??= process.ExitCode;
            }
            exitCode ??= 0;
            return (start, end, (int)exitCode);
        }
    }
}
