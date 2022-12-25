using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessTimeMonitor
{
    public static class Global
    {
        public static LogLevel LogLevel = LogLevel.INFO;
    }

    public enum LogLevel
    {
        NONE = 0,
        ERROR = 1,
        WARN = 2,
        INFO = 3,
        DEBUG = 4,
    }
}
