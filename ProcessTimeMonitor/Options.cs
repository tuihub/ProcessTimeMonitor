using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessTimeMonitor
{
    internal class Options
    {
        [Option('p', "path", Required = true, HelpText = "App path to run.")]
        public string AppPath { get; set; } = null!;
        [Option("debug", Default = false, HelpText = "Enable debug log.")]
        public bool Debug { get; set; }
    }
}
