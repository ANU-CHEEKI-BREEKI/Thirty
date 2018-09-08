using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace StringResourceClassGenerator
{
    public class CalassGenerator
    {
        List<string> props = new List<string>();

        string pathToNewClass;
        string pathToStringResourcesDirectory;

        public CalassGenerator(string pathToNewClass, string pathToStringResourcesDirectory)
        {
            this.pathToNewClass = pathToNewClass;
            this.pathToStringResourcesDirectory = pathToStringResourcesDirectory;
        }

        public void Generate()
        {
            using (var stream = File.Open(pathToNewClass, FileMode.Create))
            {
                using (TextWriter wr = new StreamWriter(stream))
                {
                    props.Clear();
                    WriteHeader(wr);

                    var names = Directory.GetFiles(pathToStringResourcesDirectory, "*.xml");
                    foreach (var name in names)
                    {
                        if (name.Contains(".TMP"))
                            continue;

                        using (var st = File.OpenRead(name))
                        {
                            using (TextReader txtReader = new StreamReader(st))
                            {
                                using (XmlReader reader = XmlReader.Create(txtReader))
                                {
                                    while (reader.ReadToFollowing("string"))
                                    {
                                        reader.MoveToAttribute("name");
                                        WriteProperty(reader.Value, wr);
                                    }
                                }
                            }
                        }
                    }
                    WriteFooter(wr);
                }
            }
        }

        public void WriteHeader(TextWriter wr)
        {
            wr.WriteLine("///");
            wr.WriteLine("///Это автоматически сгенерированный класс!");
            wr.WriteLine("///Не редактируйте его вручную!");
            wr.WriteLine("///Все внесённые исземения могут быть утеряны!");
            wr.WriteLine("///");
            wr.WriteLine("///Этот клас сгенерирован вспомагательной программой \"ResourcesClassGenerator\".");
            wr.WriteLine("///Для генерации этого класса должна быть запущена указанная выше программа.");
            wr.WriteLine("///");
            wr.WriteLine("public sealed class LocalizedStrings");
            wr.WriteLine("{");
        }

        public void WriteProperty(string name, TextWriter wr)
        {
            if (!props.Contains(name))
            {
                wr.WriteLine(
                    string.Format(
                        "    public static string {0} {{ get; private set; }}",
                        name
                    )
                );
                props.Add(name);
            }
        }

        public void WriteFooter(TextWriter wr)
        {
            wr.WriteLine("}");
        }
    }
}
