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
            process.StartInfo.FileName = commandSeq.First();
            bool isFirstArg = true;
            foreach (var arg in commandSeq)
            {
                if (isFirstArg)
                {
                    isFirstArg = false;
                    continue;
                }
                process.StartInfo.ArgumentList.Add(arg);
            }
            var workDir = opts.Dir;
            if (workDir == null)
            {
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(process.StartInfo.FileName);
            }
            else
            {
                process.StartInfo.WorkingDirectory = workDir;
            }
            process.Start();
            process.WaitForAllToExit();
        }
    }
}