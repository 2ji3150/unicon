﻿using Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace unicon {
    class MangaConverter : IConverter {

        readonly FileInfo[] fileInfos;
        readonly ProcessStartInfo[] psis;
        string OutputFileExtension { get; } = ".zip";

        public MangaConverter(string path) {
            //scan
            DirectoryInfo di = new DirectoryInfo(path);
            fileInfos = di.EnumerateFiles("*.*", SearchOption.AllDirectories).ToArray();
            psis = new ArchivePreset().GetProcess(fileInfos).ToArray();
        }
        public void Convert() {
            if (!fileInfos.Any()) return;
            if (fileInfos.Any(fi => !fi.Extension.Equals(OutputFileExtension, StringComparison.OrdinalIgnoreCase) && fi.Directory.EnumerateFiles(fi.Name + ".*").Any(file => file.Extension.Equals(OutputFileExtension, StringComparison.OrdinalIgnoreCase)))) throw new IOException("outputFile already exsists!");
            Console.WriteLine();

            long deltaSum = 0;
            for (int i = 0; i < fileInfos.Length; i++) {

                Console.WriteLine($"Processing {fileInfos[i].Name} ... {i} of {fileInfos.Length}");

                Console.WriteLine("step 1 : extract");

                #region extract
                string inFile = fileInfos[i].FullName;
                string outFile = Path.ChangeExtension(inFile, OutputFileExtension);
                string tmpDir = Path.ChangeExtension(inFile, null) + "$TMP";
                string tmpFile = Path.ChangeExtension(tmpDir, OutputFileExtension);

                if (Directory.Exists(tmpDir)) throw new IOException($"{tmpDir} exsists");
                if (File.Exists(tmpFile)) throw new IOException($"{tmpFile} exsists");

                using Process p = Process.Start(psis[i]);
                p.WaitForExit();
                if (p.ExitCode != 0) throw new Exception($"ExitCode NEQ 0");

                #endregion

                Console.WriteLine("step 2 : optimize");

                //optimize
                Preset[] presets = new Preset[] { new MozjpegPreset(), new CwebpPreset() };
                new GeneralFileConverter(presets, tmpDir).Convert();

                //repack
                Console.WriteLine("step 3 : compress");
                ZipFile.CreateFromDirectory(tmpDir, tmpFile);
                Directory.Delete(tmpDir,true);

                //replace
                Console.WriteLine("step 4 : replace");
                FileInfo fi = new FileInfo(inFile);
                FileInfo fi_out = new FileInfo(tmpFile);

                long old_size = fi.Length;
                long new_size = fi_out.Length;

                fi.IsReadOnly = false;
                fi.Delete();
                fi_out.MoveTo(outFile);
                fi_out.IsReadOnly = true;

                long delta = old_size - new_size;
                deltaSum += delta;
                Console.WriteLine($"done ... {delta.ToSizeSuffix()} decresead\n");

            }

            Console.WriteLine($"\ncomplete!\n{fileInfos.Length} files processed\n{deltaSum.ToSizeSuffix()} decreased");
        }
    }
}