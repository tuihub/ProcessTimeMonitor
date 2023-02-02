using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessTimeMonitor
{
    interface ISyncOptions
    {
        [Option("sync", Default = false, SetName = "sync", HelpText = "Enable sync mode(process monitoring).")]
        bool Sync { get; set; }
        [Option("simple", Default = false, SetName = "sync", HelpText = "Enable simple mode(process monitoring, one process only).")]
        bool Simple { get; set; }
        [Option("proc-name", Default = false, SetName = "sync", HelpText = "Enable process name mode(process monitoring, one process only).")]
        bool ProcessName { get; set; }
        [Option("proc-path", Default = false, SetName = "sync", HelpText = "Enable process path mode(process monitoring, one process only).")]
        bool ProcessPath { get; set; }
        [Option("proc-mon-path", Required = false, HelpText = "Process executable path to monitor.")]
        public string? ProcessMonPath { get; set; }
    }
    interface IAsyncOptions
    {
        [Option("async", Default = false, SetName = "async", HelpText = "Enable async mode(process monitoring).")]
        bool Async { get; set; }
        [Option("full-async", Default = false, SetName = "async", HelpText = "Enable full async mode(process monitoring).")]
        bool FullAsync { get; set; }
    }
    internal class Options : ISyncOptions, IAsyncOptions
    {
        [Option("debug", Default = false, HelpText = "Enable debug log.")]
        public bool Debug { get; set; }
        public bool Sync { get; set; }
        public bool Simple { get; set; }
        public bool ProcessName { get; set; }
        public bool ProcessPath { get; set; }
        public string? ProcessMonPath { get; set; }
        public bool Async { get; set; }
        public bool FullAsync { get; set; }
        [Option('s', "shell", Default = false, HelpText = "Enable UseShellExecute.")]
        public bool UseShellExecute { get; set; }
        [Option('d', "dir", HelpText = "Working directory.")]
        public string? Dir { get; set; }
        [Option('c', "command", Required = true, HelpText = "Command to run.")]
        public IEnumerable<string> CommandSeq { get; set; } = null!;
    }
}
