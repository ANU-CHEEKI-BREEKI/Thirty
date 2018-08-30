using System;
using UnityEngine;

[Serializable]
public class DSPlayerScore : ISavable, IResetable, ITempValuesApplyable
{
    public enum Currency { SILVER, GOLD, EXPIRIENCE }

    public Score gold;
    public Score silver;
    public Score expirience;

    public Score tempGold;
    public Score tempSilver;
    public Score tempExpirience;

    public void Save()
    {
        GameManager.Instance.SavingManager.SaveData<DSPlayerScore>(this.GetType().Name, this);
    }

    public void Load()
    {
        System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
        var g = GameManager.Instance.SavingManager.LoadData<DSPlayerScore>(this.GetType().Name);
        var fields = this.GetType().GetFields(flags);
        foreach (var f in fields)
            f.SetValue(this, f.GetValue(g));
    }

    /// <summary>
    /// Применяет
    /// </summary>
    public void ApplyTempValues()
    {
        gold.Value += tempGold.Value;
        silver.Value += tempSilver.Value;
        expirience.Value += tempExpirience.Value;
    }

    public void ResetTempValues()
    {
        tempGold.Value = 0;
        tempSilver.Value = 0;
        tempExpirience.Value = 0;
    }

    public void Reset()
    {
        gold.Value = 0;
        silver.Value = 0;
        expirience.Value = 0;

        ResetTempValues();
    }

    /// <summary>
    /// Проверка, достаточно ли указаной валюты на счету игока
    /// </summary>
    /// <param name="value">не отрицательное значение!</param>
    /// <param name="currency">валюта</param>
    /// <returns>достаточно ли указаной валюты на счету игока</returns>
    public bool EnoughtMoney(float value, Currency currency)
    {
        if (value < 0) throw new Exception("позволяется проверять только на не отрицательные значения");

        bool res = false;
        switch (currency)
        {
            case Currency.GOLD:
                if (gold.Value >= value) res = true;
                break;
            case Currency.SILVER:
                if (silver.Value >= value) res = true;
                break;
            case Currency.EXPIRIENCE:
                if (expirience.Value >= value) res = true;
                break;
            default:
                throw new Exception("нет такой валюты");
                break;
        }
        return res;
    }

    /// <summary>
    /// Тратится указанная валюта, если её достаточно на счету игрока
    /// </summary>
    /// <param name="value">не отрицательное значение!</param>
    /// <param name="currency">валюта</param>
    /// <returns>потрптилась ли валюта</returns>
    public bool SpendMoney(float value, Currency currency, bool saveAdterOperation = true)
    {
        if (value < 0) throw new Exception("позволяется только не отрицательные значения");

        bool res = EnoughtMoney(value, currency);
        if (res)
        {
            switch (currency)
            {
                case Currency.GOLD:
                        gold.Value -= value;
                    break;
                case Currency.SILVER:
                        silver.Value -= value;
                    break;
                case Currency.EXPIRIENCE:
                        expirience.Value -= value;
                    break;
            }

            if (saveAdterOperation)
                Save();
        }

        return res;
    }

    /// <summary>
    /// Зачисление валюты на счет игрока
    /// </summary>
    /// <param name="value">не отрицательное значение!</param>
    /// <param name="currency">валюта</param>
    /// <returns>зачислена ли валюта</returns>
    public void EarnMoney(float value, Currency currency, bool saveAdterOperation = true)
    {
        if (value < 0) throw new Exception("позволяется только не отрицательные значения");

        switch (currency)
        {
            case Currency.GOLD:
                gold.Value += value;
                break;
            case Currency.SILVER:
                silver.Value += value;
                break;
            case Currency.EXPIRIENCE:
                expirience.Value += value;
                break;
        }

        if (saveAdterOperation)
            Save();
    }

    /// <summary>
    /// Получить локилизованную строку предупреждения пользователя о недостатке валюты
    /// </summary>
    /// <param name="currency">валюта</param>
    /// <returns>искомая строка</returns>
    public string NotEoughtMoveyWarningString(Currency currency)
    {
        string res = string.Empty;
        switch (currency)
        {
            case Currency.SILVER:
                res = LocalizedStrings.toast_not_enough_silver;
                break;
            case Currency.GOLD:
                res = LocalizedStrings.toast_not_enough_gold;
                break;
            case Currency.EXPIRIENCE:
                res = LocalizedStrings.toast_not_enough_expirience;
                break;
        }
        return res;
    }

    [Serializable]
    public class Score
    {
        /// <summary>
        /// float - old value.
        /// даже если новое значение такое же как и старое.
        /// Score - sender
        /// </summary>
        public event OnValChanged OnValueChanged;
        public delegate void OnValChanged(float oldValue, float newValue, Score sender);

        [SerializeField] float value;
        public float Value
        {
            get { return value; }
            set { float old = this.value;  this.value = value; if (OnValueChanged != null) OnValueChanged(old, value, this); }
        }
    }
}
