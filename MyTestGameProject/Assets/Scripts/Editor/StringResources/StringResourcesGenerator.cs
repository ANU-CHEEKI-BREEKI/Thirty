using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace StringResourceClassGenerator
{
    public class StringResourcesGenerator
    {
        string pathToStringResourcesDirectory;
        List<string> allPropNames = new List<string>();
        List<string> allFilesName = new List<string>();
        Dictionary<string, Sheet> sheets = new Dictionary<string, Sheet>();

        public string[] AllFilesName { get { return allFilesName.ToArray(); } }
        public string[] AllPropNames { get { return allPropNames.ToArray(); } }
        public Sheet GetSheet(string lang)
        {
            if (sheets.ContainsKey(lang))
                return sheets[lang].GetCopy();
            else
                throw new System.InvalidProgramException();
        }

        /// <summary>
        /// Перед любы использованием генератора нужо вызвать Init()
        /// </summary>
        /// <param name="pathToStringResourcesDirectory"></param>
        public StringResourcesGenerator(string pathToStringResourcesDirectory)
        {
            this.pathToStringResourcesDirectory = pathToStringResourcesDirectory;
        }

        /// <summary>
        /// Иначиализировать генератор, прочитав все сущетсвующие файлы ресурсов. 
        /// <para>Нужо вызвать перед любым использованием генератора.</para>
        /// </summary>
        public void Init()
        {
            allPropNames.Clear();
            sheets.Clear();
            allFilesName.Clear();

            var names = Directory.GetFiles(pathToStringResourcesDirectory, "*.xml");
            foreach (var name in names)
            {
                if (name.Contains(".TMP"))
                    continue;

                Sheet sheet = new Sheet() { name = name };
                
                using (var st = File.OpenRead(name))
                {
                    using (TextReader txtReader = new StreamReader(st))
                    {
                        using (XmlReader reader = XmlReader.Create(txtReader))
                        {
                            while (reader.ReadToFollowing("string"))
                            {
                                reader.MoveToAttribute("name");
                                var n = reader.Value;
                                reader.MoveToContent();
                                reader.Read();
                                var v = reader.Value;

                                sheet.AddProp(n, v);

                                if (!allPropNames.Contains(n))
                                    allPropNames.Add(n);
                            }
                        }
                    }
                }

                if (!sheets.ContainsKey(name))
                    sheets.Add(sheet.name, sheet);
                else
                    sheets[sheet.name] = sheet;

                if (!allFilesName.Contains(name))
                    allFilesName.Add(name); 
            }
        }

        /// <summary>
        /// Добавляет/удаляет поля во ВСЕХ файлов ресуров, но заполняет значениями только один выбранный файл
        /// </summary>
        /// <param name="sheet">Инфа о файле, которому нужно назначать значение полей</param>
        /// <param name="allPropsNames">Все поля. Если чего то будет нехватать</param>
        /// <param name="deleteIfNoInProps">Если имени поля нет в массиве allPropsNames, то удалять ли такое поле из файлов?</param>
        public void Generate(Sheet sheet, string[] allPropsNames = null, bool deleteIfNoInProps = false)
        {
            if (allPropsNames != null)
            {
                allPropNames.Clear();
                allPropNames.AddRange(allPropsNames);
            }

            var names = Directory.GetFiles(pathToStringResourcesDirectory, "*.xml");

            foreach (var sh in sheets.Values)
            {
                using (var file = File.Open(sh.name, FileMode.Create))
                {
                    using (TextWriter writer = new StreamWriter(file))
                    {
                        //пишем шапку
                        WriteHeader(writer);
                        
                        //проходим по ВСЕМ именам элементов
                        //чтобы во всех файлах элементы были в одном порядке
                        foreach (var pname in allPropNames)
                        {
                            //записываем элементы
                            if (sh.name == sheet.name)
                            {
                                if (sheet.props.ContainsKey(pname))
                                    //записываем новые зачения элементов
                                    WriteProperty(pname, sheet.props[pname], writer);
                                else
                                    //записываем новые элементы с пустыми строками внутри
                                    WriteProperty(pname, string.Empty, writer);
                            }
                            else
                            {
                                if (sh.props.ContainsKey(pname))
                                    //запсываем все элементы такими как они и были
                                    WriteProperty(pname, sh.props[pname], writer);
                                else
                                    //записываем новые элементы с пустыми строками внутри
                                    WriteProperty(pname, string.Empty, writer);
                            }
                        }
                        //пишем футер
                        WriteFooter(writer);
                    }
                }
            }
        }

        public void WriteHeader(TextWriter wr)
        {
            wr.WriteLine("<?xml version = \"1.0\" encoding = \"utf - 8\" ?>");
            wr.WriteLine("<!--Этот файл сгенерирован редактором-->");
            wr.WriteLine("<!--В принципе, изменять можно и вручную...-->");
            wr.WriteLine("<!--Редактор сделан для удобства, чтобы прям из Unity редактировать-->");
            wr.WriteLine("<resources>");
        }

        public void WriteProperty(string name, string value, TextWriter wr)
        {
            wr.WriteLine(
                string.Format(
                    "<string name = \"{0}\">{1}</string>",
                    name,
                    value
                )
            );
        }

        public void WriteFooter(TextWriter wr)
        {
            wr.WriteLine("</resources>");
        }
    }

    public class Sheet
    {
        public string name = string.Empty;
        /// <summary>
        /// Не меняй извне, пожалуйста!
        /// </summary>
        public Dictionary<string, string> props = new Dictionary<string, string>();

        public void AddProp(string name, string value)
        {
            if (!props.ContainsKey(name))
                props.Add(name, value);
            else
                props[name] = value;
        }
     
        public Sheet GetCopy()
        {
            return new Sheet() { name = this.name, props = new Dictionary<string, string>(this.props) };
        }
    }
}