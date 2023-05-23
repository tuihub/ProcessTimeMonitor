using Microsoft.Extensions.Logging;

namespace TuiHub.ProcessTimeMonitorLibrary
{
    public partial class ProcessTimeMonitor
    {
        private readonly ILogger? _logger;
        public ProcessTimeMonitor(ILogger? logger = null)
        {
            _logger = logger;
        }
    }
}