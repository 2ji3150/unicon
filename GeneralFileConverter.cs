using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace unicon {
    class GeneralFileConverter : IConverter {
        readonly ProcessStartInfo[] psis;

        public GeneralFileConverter(Preset[] presets, string path) {
            //scan
            DirectoryInfo di = new(path);
            var fileInfos = di.EnumerateFiles("*.*", SearchOption.AllDirectories).ToArray();
            psis = presets.SelectMany(preset => preset.GetProcess(fileInfos)).ToArray();
        }
        public void Convert() {
            if (psis.Length == 0) return;

            var counter = 0;
            Console.WriteLine($"\n\n{psis.Length} files detected\n");

            var flattenBlock = new TransformManyBlock<ProcessStartInfo[], ProcessStartInfo>(psi => psi);
            var processBlock = new TransformBlock<ProcessStartInfo, bool>(async psi => {
                var tcs = new TaskCompletionSource<bool>();
                using Process p = new() { StartInfo = psi, EnableRaisingEvents = true };
                p.Exited += (s, a) => {
                    if (p.ExitCode == 0) tcs.SetResult(true);
                    else tcs.SetException(new Exception($"ExitCode NEQ 0"));
                };
                p.Start();
                return await tcs.Task;
            }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 3 });
            var progressBlock = new ActionBlock<bool>(success => Console.Write($"\r{++counter}/{psis.Length}"));

            var dataflowLinkOptions = new DataflowLinkOptions() { PropagateCompletion = true };
            flattenBlock.LinkTo(processBlock, dataflowLinkOptions);
            processBlock.LinkTo(progressBlock, dataflowLinkOptions);

            flattenBlock.Post(psis);
            flattenBlock.Complete();
            progressBlock.Completion.Wait();
            Console.WriteLine();
        }
    }
}