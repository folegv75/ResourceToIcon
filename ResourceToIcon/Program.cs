using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceToIcon
{
    public class IconResource
    {
        List<string> TextBytes;

        public IconResource()
        {
            TextBytes = new List<string>();
        }

        public void AppendTextBytes(string ByteStr)
        {
            var s = ByteStr.Replace('\'',' ');
            TextBytes.Add(s.Trim());
        }
    }

    class Worker
    {
        public string Filename { get; set; }

        List<IconResource> Icons = new List<IconResource>();

        public void Work()
        {
            if (!File.Exists(Filename))
            {
                Console.WriteLine($"Файл не найден:{Filename}");
                return;
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
                            Icons.Add(currIcon);
                        } else
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

        }
    }

    class Program
    {
        

        static void Main(string[] args)
        {
            if (args.Count()!=1)
            {
                Console.WriteLine("Укажите файл в параметрах типа rc, содержащий текстовое описание иконок (Borland)");
                return;
            }
            var W = new Worker
            {
                Filename = args[0]
            };
            W.Work();
        }
    }
}
