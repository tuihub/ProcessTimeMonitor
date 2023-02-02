using CommandLine;
using ProcessTimeMonitor.Utils;
using SavedataManager.Utils;
using System.Diagnostics;

namespace ProcessTimeMonitor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Log.Debug("Main", "Starting program");
                Parser.Default.ParseArguments<Options>(args)
                    .WithParsed<Options>(Run)
                    .WithNotParsed(HandleParseError);
            }
            catch (Exception e)
            {
                Log.Error("Main", $"{e.Message}");
                Log.Debug("Main", $"{e.StackTrace}");
            }
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            if (errs.IsVersion())
            {
                Log.Debug("HandleParseError", "Running version");
                return;
            }
            if (errs.IsHelp())
            {
                Log.Debug("HandleParseError", "Running help");
                return;
            }
        }
        private static void Run(Options opts)
        {
            if (opts.Debug == true)
            {
                Global.LogLevel = LogLevel.DEBUG;
                Log.Debug("Run", "Setting Loglevel to DEBUG");
            }
            Process process = new Process();
            if (opts.UseShellExecute == true)
            {
                Log.Debug("Run", $"opts.UseShellExecute = {opts.UseShellExecute}, setting process.StartInfo.UseShellExecute to true");
                process.StartInfo.UseShellExecute = true;
            }
            var commandSeq = opts.CommandSeq;
            Log.Debug("Run", $"commandSeq = {String.Join(", ", commandSeq)}");
            process.StartInfo.FileName = commandSeq.First();
            Log.Debug("Run", $"process.StartInfo.FileName = {commandSeq.First()}");
            bool isFirstArg = true;
            foreach (var arg in commandSeq)
            {
                if (isFirstArg)
                {
                    Log.Debug("Run", $"isFirstArg = {isFirstArg}, skipping arg: {arg}");
                    isFirstArg = false;
                    Log.Debug("Run", $"Setting isFirstArg to false");
                    continue;
                }
                process.StartInfo.ArgumentList.Add(arg);
                Log.Debug("Run", $"Adding arg: {arg} to process.StartInfo.ArgumentList");
            }
            Log.Debug("Run", $"process.StartInfo.ArgumentList = {String.Join(", ", process.StartInfo.ArgumentList)}");
            var workDir = opts.Dir;
            Log.Debug("Run", $"workDir = {workDir}");
            if (String.IsNullOrEmpty(workDir))
            {
                workDir = Path.GetDirectoryName(process.StartInfo.FileName);
                Log.Debug("Run", $"workDir is null or empty, setting workDir to {Path.GetDirectoryName(process.StartInfo.FileName)}");
                //if (String.IsNullOrEmpty(workDir))
                //{
                //    workDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                //    Log.Debug("Run", $"Path.GetDirectoryName(process.StartInfo.FileName) is null or empty, setting workDir to {Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}");
                //}
            }
            if (String.IsNullOrEmpty(workDir) == true)
            {
                Log.Debug("Run", $"workDir is null or empty, doing nothing.");
            }
            else
            {
                Directory.SetCurrentDirectory(workDir);
                Log.Debug("Run", $"Setting CurrentDirectory to {workDir}");
                process.StartInfo.WorkingDirectory = workDir;
                Log.Debug("Run", $"Setting process.StartInfo.WorkingDirectory to {workDir}");
            }
            Log.Info("Run", $"Starting program: {process.StartInfo.FileName}");
            process.Start();
            var startDt = DateTime.Now;
            Log.Debug("Run", $"startDt = {startDt}");
            Log.Debug("Run", $"process.Id = {process.Id}");
            Log.Debug("Run", $"process.process.GetProcessNameEx = {process.GetProcessNameEx()}");
            string procMonName = "";
            if (opts.ProcessName == true || opts.ProcessPath == true)
            {
                if (opts.ProcessMonPath == null)
                {
                    Log.Error("Run/procMonName", $"ProcName or ProcPath mode, ProcessMonPath = null, exiting.");
                    Environment.Exit(0);
                }
                else
                {
                    if (OperatingSystem.IsWindows() == true)
                    {
                        Log.Debug("Run/procMonName", $"Windows platform, procMonName = GetFileNameWithoutExtension()");
                        procMonName = Path.GetFileNameWithoutExtension(opts.ProcessMonPath);
                    }
                    else
                    {
                        Log.Debug("Run/procMonName", $"Other platform, procMonName = GetFileName()");
                        procMonName = Path.GetFileName(opts.ProcessMonPath);
                    }
                }
            }
            if (opts.Simple == true)
                process.WaitForExit();
            else if (opts.ProcessName == true)
                ProcessHelper.WaitForProcToExit(procMonName);
            else if (opts.ProcessPath == true)
                ProcessHelper.WaitForProcToExit(procMonName, opts.ProcessMonPath);
            else if (opts.Sync == true)
                process.WaitForAllToExit();
            else if (opts.FullAsync == true)
                process.WaitForAllToExitFullAsync().Wait();
            else if (opts.Async == true)
                process.WaitForAllToExitAsync().Wait();
            var endDt = DateTime.Now;
            Log.Debug("Run", $"endDt = {endDt}");
            var timeElapsed = endDt - startDt;
            Log.Info("Run", $"timeElapsed = {timeElapsed}");
        }
    }
}