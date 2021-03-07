using System;
using System.Diagnostics;
using System.IO;

namespace unicon {
    class Program {
        enum Mode { mozjpeg, cwebp, manga }
        static void Main() {
            Console.Write("Enter the Directory FullPath for processing:");
            string path = Console.ReadLine();
            if (!Directory.Exists(path)) return;
            foreach (Mode m in (Mode[])Enum.GetValues(typeof(Mode))) Console.WriteLine($"{(int)m}.{m}");
            Console.Write("Choose the mode number:");
            if (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out int num)) return;
            Stopwatch sw = new();
            sw.Start();
            IConverter SimpleFactory(int num) {
                return ((Mode)num) switch {
                    Mode.cwebp => new GeneralFileConverter(new Preset[] { new CwebpPreset() }, path),
                    Mode.mozjpeg => new GeneralFileConverter(new Preset[] { new MozjpegPreset() }, path),
                    Mode.manga => new MangaConverter(path),
                    _ => new GeneralFileConverter(new Preset[] { new MozjpegPreset() }, path),
                };
            }
            try {
                IConverter converter = SimpleFactory(num);
                converter.Convert();
                sw.Stop();
                TimeSpan ts = sw.Elapsed;
                Console.WriteLine($"...done\n\nRuntime {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:00}");
            }
            catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
            }
            finally {
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadLine();
            }
        }
    }
}