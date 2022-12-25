using CommandLine;
using SavedataManager.Utils;

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
        }
    }
}