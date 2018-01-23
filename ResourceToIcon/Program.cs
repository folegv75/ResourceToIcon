using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class IconResource
    {
        public const int IconLength = 16 * 48 - 2;

        List<string> TextBytes;
        public byte[] Bin;

        public IconResource()
        {
            TextBytes = new List<string>();
        }

        public void AppendTextBytes(string ByteStr)
        {
            var s = ByteStr.Replace('\'', ' ');
            TextBytes.Add(s.Trim());
        }

        public void ConvertToBin()
        {
            Bin = new byte[IconLength];
            int BinIndex = 0;
            foreach (var someStr in TextBytes)
            {
                var HexChars = someStr.Split(' ');
                for (int i = 0; i < HexChars.Length; i++)
                {

                    Bin[BinIndex] = Convert.ToByte(HexChars[i], 16);
                    BinIndex++;
                }
            }
        }
    }

    class Worker
    {
        public string Filename { get; set; }
        public string OutputDir;

        List<IconResource> Icons = new List<IconResource>();

        bool ReadTextIcon()
        {
            if (!File.Exists(Filename))
            {
                Console.WriteLine($"Файл не найден:{Filename}");
                return false;
            }

            using (StreamReader sr = new StreamReader(Filename))
            {

                string line;
                bool IsIconStart = false;
                IconResource currIcon = null;
                while ((line = sr.ReadLine()) != null)
                {
                    if (IsIconStart)
                    {
                        if (line == "}")
                        {
                            IsIconStart = false;
                            Icons.Add(currIcon);
                        }
                        else
                        {
                            currIcon.AppendTextBytes(line);
                        }

                    }
                    else
                    {
                        if (line == "{")
                        {
                            IsIconStart = true;
                            currIcon = new IconResource();
                        }
                    }
                }
            }
            return true;
        }

        void ConvertTextToByte()
        {
            foreach (var someIcon in Icons) someIcon.ConvertToBin();
        }

        void SaveToIconFile()
        {
            int IconCount = 1;
            foreach (var someIcon in Icons)
            {
                string fileName = OutputDir + "\\Icon" + string.Format("{0:d3}", IconCount) + ".ico";
                using (BinaryWriter bw = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                {
                    bw.Write(someIcon.Bin, 0, IconResource.IconLength);
                }
                IconCount++;
            }
        }

        public void Work()
        {
            if (!ReadTextIcon()) return;
            ConvertTextToByte();
            SaveToIconFile();

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() != 1)
            {
                Console.WriteLine("Укажите файл в параметрах типа rc, содержащий текстовое описание иконок (Borland)");
                return;
            }
            var W = new Worker
            {
                Filename = args[0]
            };
            var f = new FileInfo(W.Filename);

            W.OutputDir = f.DirectoryName;
            W.Work();
        }
    }
}
