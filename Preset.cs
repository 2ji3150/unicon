using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace unicon {
    public abstract class Preset {
        public abstract string OutputFileExtention { get; }
        public abstract string BatchName { get; }
        public abstract string SearchPattern { get; }

        readonly string fileName;
        readonly HashSet<string> filter;

        public Preset() {
            fileName = Path.Combine("batch", $"{BatchName}.bat");
            filter = new HashSet<string>(SearchPattern.Split('|'), StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<ProcessStartInfo> GetProcess(FileInfo[] fis) {
            foreach (var fi in fis) {
                if (!filter.Contains(fi.Extension)) continue;
                if (!fi.Extension.Equals(OutputFileExtention, StringComparison.OrdinalIgnoreCase) && fi.Directory.EnumerateFiles(fi.Name + ".*").Any(fii => fii.Extension.Equals(OutputFileExtention, StringComparison.OrdinalIgnoreCase))) throw new IOException("outputFile already exsists!");

                var psi = new ProcessStartInfo(fileName, $@"""{fi.FullName}""") {
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                yield return psi;
            }
        }
    }

    class MozjpegPreset : Preset {
        public override string OutputFileExtention => ".jpg";
        public override string BatchName => "mozjpeg";
        public override string SearchPattern => ".jpg|.jpeg";
    }

    class CwebpPreset : Preset {
        public override string OutputFileExtention => ".webp";
        public override string BatchName => "cwebp";
        public override string SearchPattern => ".bmp|.png|.tif|.tiff|.webp";
    }

    class ArchivePreset : Preset {
        public override string OutputFileExtention => ".rar";
        public override string BatchName => "unpack";
        public override string SearchPattern => ".zip|.rar|.7z";
    }
}