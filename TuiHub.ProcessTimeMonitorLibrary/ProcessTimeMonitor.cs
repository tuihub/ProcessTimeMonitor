using Microsoft.Extensions.Logging;

namespace TuiHub.ProcessTimeMonitorLibrary
{
    public partial class ProcessTimeMonitor<T>
    {
        private readonly ILogger<T>? _logger;
        public ProcessTimeMonitor(ILogger<T>? logger)
        {
            _logger = logger;
        }
    }
}