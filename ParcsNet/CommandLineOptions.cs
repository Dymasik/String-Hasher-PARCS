using CommandLine;
using Parcs.Module.CommandLine;

namespace ParcsNet
{
    class CommandLineOptions : BaseModuleOptions
    {
        [Option('t', Required = true, HelpText = "File path to the text.")]
        public string FilePath { get; set; }
        [Option('p', Required = true, HelpText = "Number of points.")]
        public int PointsNum { get; set; }
    }
}
