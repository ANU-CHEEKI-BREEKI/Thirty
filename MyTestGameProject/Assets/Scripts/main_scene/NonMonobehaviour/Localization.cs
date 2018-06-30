using System;
using System.Reflection;
using UnityEngine;
using System.Xml;
using System.IO;

public sealed class Localization
{
    const string PATH_TO_LOCALIZATION_FILES = @"TextAssets\Localization\";
    public static SystemLanguage CurrentLanguage { get; private set; }
    
    /// <summary>
    /// Уставнавливает новый язык локализации, если для него есть определение.
    /// </summary>
    /// <param name="language">Новый язык локализации</param>
    /// <returns>Если язык локализации был изменен, возвратится true, иначе - false</returns>
    public static bool SetLanguage(SystemLanguage language)
    {
        SystemLanguage newLanguage;
        ReloadStrings(language, out newLanguage);

        var res = true;
        if (newLanguage == CurrentLanguage)
            res = false;
        else
            CurrentLanguage = newLanguage;

        return res;
    }

    /// <summary>
    /// Находит в соответствующем файле локализации нужные строки и устанавливает их значения соответствующим полям.
    /// <para>Так как не все языки поддерживаются, то загружен будет либо указанный язык, либо язык по умолчанию (английский)</para>
    /// </summary>
    /// <param name="language">Язык локализации для загрузки</param>
    /// <param name="newLanguage">Сюда будет занесено значение загруженого языка локализации</param>
    static void ReloadStrings(SystemLanguage language, out SystemLanguage newLanguage)
    {
        string languageFileName = string.Empty;

        switch (language)
        {
            case SystemLanguage.Russian:
                languageFileName = "RU";
                newLanguage = SystemLanguage.Russian;
                break;
            default:
                //english
                languageFileName = "EN";
                newLanguage = SystemLanguage.English;
                break;
        }

        TextAsset localisationFile = Resources.Load<TextAsset>(PATH_TO_LOCALIZATION_FILES + languageFileName);
        using (TextReader txtReader = new StringReader(localisationFile.text))
        {
            using (XmlReader reader = XmlReader.Create(txtReader))
            {
                while (reader.ReadToFollowing("string"))
                {
                    reader.MoveToAttribute("name");
                    var name = reader.Value;
                    reader.MoveToContent();
                    var value = reader.ReadElementContentAsString();

                    SetString(name, value);
                }
            }
        }
    }

    /// <summary>
    /// Устанавливает значение строкового ресурса по его имени
    /// </summary>
    /// <param name="name">Имя строкового ресурса</param>
    /// <param name="value">Новое значение строкового ресурса</param>
    static void SetString(string name, string value)
    {
        var property = typeof(LocalizedStrings).GetProperty(name, BindingFlags.Static | BindingFlags.Public);

        if (property == null)
            throw new Exception(string.Format("Поля строкового ресурса с именем \"{0}\" не существует.", name));

        property.SetValue(null, value, null);
    }

    /// <summary>
    /// Получает значение строкового ресурса по его имени
    /// </summary>
    /// <param name="name">Имя строкового ресурса</param>
    /// <returns>Значение строкового ресурса</returns>
    public static string GetString(string name)
    {
        string res = string.Empty;

        var property = typeof(LocalizedStrings).GetProperty(name, BindingFlags.Static | BindingFlags.Public);
        if(property != null)
            res = property.GetValue(null, null) as string;

        if (string.IsNullOrEmpty(res))
            res = LocalizedStrings.missing_string;

        return res;
    }
}
