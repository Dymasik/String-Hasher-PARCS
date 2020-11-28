using CommandLine;
using Parcs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace ParcsNet
{
    class HashStringModule : MainModule
    {
        private static CommandLineOptions options;

        public static void Main(string[] args)
        {
            if (args != null)
            {
                var result = Parser.Default
                    .ParseArguments<CommandLineOptions>(args)
                    .WithParsed(parsed => options = parsed);
                if (result.Tag == ParserResultType.NotParsed)
                {
                    throw new ArgumentException($@"Cannot parse the arguments. Possible usages:{options.GetUsage()}");
                }
            }

            (new HashStringModule()).RunModule(options);
            Console.ReadKey();
        }

        private static IEnumerable<HashStringOptions> DivideByParts(HashStringOptions textOptions, int parts)
        {
            var lastIndex = 0;
            var len = textOptions.Text.Length / parts;
            var remains = textOptions.Text.Length % parts;
            while (parts-- > 0) {
                yield return new HashStringOptions
                {
                    Text = textOptions.Text.Substring(lastIndex, len + (parts == 1 ? remains : 0)),
                    StartPosition = lastIndex
                };
                lastIndex += len; 
            } 
        }

        private static BigInteger Join(IList<BigInteger> hashes)
        {
            var hash = new BigInteger(0);
            foreach (var partHash in hashes) {
                hash += partHash;
            }
            return hash;
        }

        private static void SaveHash(BigInteger hash, long time)
        {
            var text = $"{hash}${time}";
            var filePath = "./result.txt";

            Console.WriteLine("Point6");
            File.WriteAllText(filePath, text);
        }

        public override void Run(ModuleInfo info, CancellationToken token = default)
        {
            var textFilePath = options.FilePath;
            string text;

            try
            {
                text = File.ReadAllText(textFilePath);
            }

            catch (FileNotFoundException)
            {
                return;
            }

            int[] possibleValues = { 1, 2, 4, 8, 16, 32 };

            int pointsNum = options.PointsNum;

            if (!possibleValues.Contains(pointsNum))
            {
                return;
            }
            Console.WriteLine("Point2");


            var points = new IPoint[pointsNum];
            var channels = new IChannel[pointsNum];
            for (int i = 0; i < pointsNum; ++i)
            {
                points[i] = info.CreatePoint();
                channels[i] = points[i].CreateChannel();
                points[i].ExecuteClass("ParcsNet.StringHasher");
            }

            BigInteger hash = 0;
            DateTime time = DateTime.Now;

            var textOptions = new HashStringOptions
            {
                Text = text,
                StartPosition = 0
            };

            Console.WriteLine("Point3");
            var watch = Stopwatch.StartNew();
            switch (pointsNum)
            {
                case 1:
                    channels[0].WriteObject(textOptions);
                    hash = channels[0].ReadObject<BigInteger>();
                    break;
                case 2:
                case 4:
                case 8:
                case 16:
                case 32:
                    {
                        var splitedText = DivideByParts(textOptions, pointsNum).ToArray();
                        for (int i = 0; i < splitedText.Length; i++)
                        {
                            channels[i].WriteObject(splitedText[i]);
                        }

                        hash = Join(channels.Select(c => c.ReadObject<BigInteger>()).ToArray());

                        Console.WriteLine("Point4 " + hash);
                    }
                    break;
                default:
                    return;
            }
            watch.Stop();

            Console.WriteLine("Point5");
            SaveHash(hash, watch.ElapsedMilliseconds);
        }
    }
}
