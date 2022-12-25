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
        [Option("debug", Default = false, HelpText = "Enable debug log.")]
        public bool Debug { get; set; }
        [Option("full-async", Default = false, HelpText = "Enable full async mode(process monitoring).")]
        public bool FullAsync { get; set; }
        [Option('d', "dir", HelpText = "Set working directory.")]
        public string? Dir { get; set; }
        [Option('c', "command", Required = true, HelpText = "Command to run.")]
        public IEnumerable<string> CommandSeq { get; set; } = null!;
    }
}
