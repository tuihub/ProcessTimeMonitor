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
                Log.Debug("Run", "Setting Loglevel to DEBUG");
                Global.LogLevel = LogLevel.DEBUG;
            }
            Process process = new Process();
            var commandSeq = opts.CommandSeq;
            Log.Debug("Run", $"commandSeq = {commandSeq}");
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
            if (workDir == null)
            {
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(process.StartInfo.FileName);
                Log.Debug("Run", $"workDir = {workDir}, setting workDir to {Path.GetDirectoryName(process.StartInfo.FileName)}");
            }
            else
            {
                process.StartInfo.WorkingDirectory = workDir;
                Log.Debug("Run", $"Setting workDir to {workDir}");
            }
            Log.Info("Run", $"Starting program: {process.StartInfo.FileName}");
            process.Start();
            var startDt = DateTime.Now;
            Log.Debug("Run", $"startDt = {startDt}");
            Log.Debug("Run", $"process.Id = {process.Id}");
            Log.Debug("Run", $"process.ProcessName = {process.ProcessName}");
            process.WaitForAllToExit();
            var endDt = DateTime.Now;
            Log.Debug("Run", $"endDt = {endDt}");
            var timeElapsed = endDt - startDt;
            Log.Info("Run", $"timeElapsed = {timeElapsed}");
        }
    }
}