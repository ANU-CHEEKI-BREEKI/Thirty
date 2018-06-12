using System;
using System.Reflection;
using UnityEngine;
using System.Xml;
using System.IO;

public sealed class Localization
{
    const string PATH_TO_LOCALIZATION_FILES = @"TextAssets\Localization\";
    public static SystemLanguage CurrentLanguage { get; private set; }

    #region strings;

    static public string current_languade { get; private set; }

    static public string loading { get; private set; }
    static public string level_generading { get; private set; }
    static public string resoures_loadind { get; private set; }

    static public string play_game { get; private set; }
    static public string settings { get; private set; }
    static public string squad { get; private set; }
    static public string developers { get; private set; }


    static public string healtarget_weakest { get; private set; }
    static public string healtarget_strongest { get; private set; }
    static public string healtarget_all { get; private set; }

    static public string error { get; private set; }

    static public string formation_ranks_name { get; private set; }
    static public string formation_ranks_description { get; private set; }
    static public string formation_phalanx_name { get; private set; }
    static public string formation_phalanx_description { get; private set; }
    static public string formation_shields_name { get; private set; }
    static public string formation_shields_description { get; private set; }

    static public string condition_inFight_name { get; private set; }
    static public string condition_inFight_description { get; private set; }

    static public string health { get; private set; }
    static public string squadHealth { get; private set; }
    static public string armour { get; private set; }
    static public string baseDamage { get; private set; }
    static public string armourDamage { get; private set; }
    static public string attack { get; private set; }
    static public string defence { get; private set; }
    static public string defenceHalfSector { get; private set; }
    static public string missileBlock { get; private set; }
    static public string attackDistance { get; private set; }
    static public string speed { get; private set; }
    static public string acceleration { get; private set; }
    static public string rotationSpeed { get; private set; }
    static public string chargeImpact { get; private set; }
    static public string chargeDeflect { get; private set; }
    static public string chargeDamage { get; private set; }
    static public string defenceGoingThrough { get; private set; }
    static public string flyingSpeed { get; private set; }

    static public string arrowWalley_radius { get; private set; }
    static public string arrowWalley_arrowscount { get; private set; }
    static public string cooldown { get; private set; }
    static public string duration { get; private set; }

    static public string cost { get; private set; }
    static public string mass { get; private set; }
    static public string weaponConstraint_cantReformPhalanx { get; private set; }
    static public string weaponConstraint_cantReformPhalanxInFight { get; private set; }
    static public string weaponConstraint_cantUseShield { get; private set; }

    static public string attention { get; private set; }

    static public string yes { get; private set; }
    static public string no { get; private set; }
    static public string ok { get; private set; }
    static public string cancel { get; private set; }
    static public string upgrade { get; private set; }

    static public string gold { get; private set; }
    static public string silver { get; private set; }
    static public string expirience { get; private set; }

    static public string name { get; private set; }
    static public string description { get; private set; }
    static public string equipment { get; private set; }
    static public string inventory { get; private set; }
    static public string consumables { get; private set; }
    static public string equipment_market { get; private set; }
    static public string consumables_market { get; private set; }
    static public string equipment_ground { get; private set; }
    
    static public string equipment_body_chainmail_name { get; private set; }
    static public string equipment_body_chainmail_description { get; private set; }
    static public string equipment_body_linotorax_name { get; private set; }
    static public string equipment_body_linotorax_description { get; private set; }
    static public string equipment_body_anatomic_armour_name { get; private set; }
    static public string equipment_body_anatomic_armour_description { get; private set; }
    static public string equipment_body_none_name { get; private set; }
    static public string equipment_body_none_description { get; private set; }
    static public string equipment_helmet_corinthian_close_name { get; private set; }
    static public string equipment_helmet_corinthian_close_description { get; private set; }
    static public string equipment_helmet_thrace_open_name { get; private set; }
    static public string equipment_helmet_thrace_open_description { get; private set; }
    static public string equipment_helmet_none_name { get; private set; }
    static public string equipment_helmet_none_description { get; private set; }
    static public string equipment_shield_hoplon_name { get; private set; }
    static public string equipment_shield_hoplon_description { get; private set; }
    static public string equipment_shield_bakler_name { get; private set; }
    static public string equipment_shield_bakler_description { get; private set; }
    static public string equipment_shield_none_name { get; private set; }
    static public string equipment_shield_none_description { get; private set; }
    static public string equipment_weapon_doru_name { get; private set; }
    static public string equipment_weapon_doru_description { get; private set; }
    static public string equipment_weapon_sharp_doru_name { get; private set; }
    static public string equipment_weapon_sharp_doru_description { get; private set; }
    static public string equipment_weapon_kopis_name { get; private set; }
    static public string equipment_weapon_kopis_description { get; private set; }
    static public string equipment_weapon_ksifos_name { get; private set; }
    static public string equipment_weapon_ksifos_description { get; private set; }
    static public string equipment_weapon_sarrisa_name { get; private set; }
    static public string equipment_weapon_sarrisa_description { get; private set; }
    static public string equipment_weapon_wings_sarrisa_name { get; private set; }
    static public string equipment_weapon_wings_sarrisa_description { get; private set; }

    static public string skill_arrowWalley_name { get; private set; }
    static public string skill_arrowWalley_description { get; private set; }
    static public string skill_charge_name { get; private set; }
    static public string skill_charge_description { get; private set; }


    static public string consumable_pilums_name { get; private set; }
    static public string consumable_pilums_description { get; private set; }
    static public string consumable_pilums_light_name { get; private set; }
    static public string consumable_pilums_light_description { get; private set; }
    static public string consumable_pilums_heavy_name { get; private set; }
    static public string consumable_pilums_heavy_description { get; private set; }
    static public string consumable_pilums_perfect_name { get; private set; }
    static public string consumable_pilums_perfect_description { get; private set; }

    static public string skill_upgrade_arrowWalley_startunlock_name { get; private set; }
    static public string skill_upgrade_arrowWalley_startunlock_description { get; private set; }
    static public string skill_upgrade_arrowWalley_basedamage_largebow_name { get; private set; }
    static public string skill_upgrade_arrowWalley_basedamage_largebow_description { get; private set; }
    static public string skill_upgrade_arrowWalley_basedamage_weightarrows_name { get; private set; }
    static public string skill_upgrade_arrowWalley_basedamage_weightarrows_description { get; private set; }
    static public string skill_upgrade_arrowWalley_armourdamage_sharparrows_name { get; private set; }
    static public string skill_upgrade_arrowWalley_armourdamage_sharparrows_description { get; private set; }
    static public string skill_upgrade_arrowWalley_armourdamage_temperedarrows_name { get; private set; }
    static public string skill_upgrade_arrowWalley_armourdamage_temperedarrows_description { get; private set; }
    static public string skill_upgrade_arrowWalley_arrowscount_name { get; private set; }
    static public string skill_upgrade_arrowWalley_arrowscount_description { get; private set; }
    static public string skill_upgrade_arrowWalley_radius_name { get; private set; }
    static public string skill_upgrade_arrowWalley_radius_description { get; private set; }
    static public string skill_upgrade_arrowWalley_radius_minus_name { get; private set; }
    static public string skill_upgrade_arrowWalley_radius_minus_description { get; private set; }
    static public string skill_upgrade_arrowWalley_cooldown_name { get; private set; }
    static public string skill_upgrade_arrowWalley_cooldown_description { get; private set; }

    static public string skill_upgrade_charge_startunlock_name { get; private set; }
    static public string skill_upgrade_charge_startunlock_description { get; private set; }
    static public string skill_upgrade_charge_speed_moretrain_name { get; private set; }
    static public string skill_upgrade_charge_speed_moretrain_description { get; private set; }
    static public string skill_upgrade_charge_speed_ligthfootwear_name { get; private set; }
    static public string skill_upgrade_charge_speed_ligthfootwear_description { get; private set; }
    static public string skill_upgrade_charge_speed_shortranderun_name { get; private set; }
    static public string skill_upgrade_charge_speed_shortranderun_description { get; private set; }
    static public string skill_upgrade_charge_duration_longrangerun_name { get; private set; }
    static public string skill_upgrade_charge_duration_longrangerun_description { get; private set; }
    static public string skill_upgrade_charge_acceleration_reinforcefootbase_name { get; private set; }
    static public string skill_upgrade_charge_acceleration_reinforcefootbase_description { get; private set; }
    static public string skill_upgrade_charge_acceleration_lowerstart_name { get; private set; }
    static public string skill_upgrade_charge_acceleration_lowerstart_description { get; private set; }
    static public string skill_upgrade_charge_intensivtrain_name { get; private set; }
    static public string skill_upgrade_charge_intensivtrain_description { get; private set; }
    static public string skill_upgrade_charge_damage_runningpunch_name { get; private set; }
    static public string skill_upgrade_charge_damage_runningpunch_description { get; private set; }


    static public string toMainMenu { get; private set; }
    static public string goToMainMenu_header { get; private set; }
    static public string goToMainMenu_info { get; private set; }
    static public string resetSettings_header { get; private set; }
    static public string resetSettings_info { get; private set; }

    static public string currentValue { get; private set; }
    static public string baseValue { get; private set; }
    static public string currentValue_baseValue { get; private set; }

    static public string durability_new { get; private set; }
    static public string durability_worn { get; private set; }
    static public string durability_damaged { get; private set; }

    static public string toast_cant_use_with_shield { get; private set; }
    static public string toast_cant_use_with_current_weapon { get; private set; }
    static public string toast_not_enough_gold { get; private set; }
    static public string toast_not_enough_silver { get; private set; }
    static public string toast_not_enough_expirience { get; private set; }
    static public string toast_not_enough_equipment_count { get; private set; }
    static public string toast_stat_max_upgrade { get; private set; }
    static public string toast_cant_drop_weapon { get; private set; }
    static public string toast_cant_drop { get; private set; }

    static public string you_died { get; private set; }
    static public string tip_death_try_bay_equip_and_train_squad { get; private set; }

    #endregion;

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
        var property = typeof(Localization).GetProperty(name, BindingFlags.Static | BindingFlags.Public);

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
        var property = typeof(Localization).GetProperty(name, BindingFlags.Static | BindingFlags.Public);

        if (property == null)
            throw new Exception(string.Format("Поля строкового ресурса с именем \"{0}\" не существует.", name));

        return property.GetValue(null, null) as string;
    }
}
