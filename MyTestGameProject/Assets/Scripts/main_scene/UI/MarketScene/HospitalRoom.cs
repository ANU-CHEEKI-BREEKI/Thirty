using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Tools;

public class HospitalRoom : MonoBehaviour
{
    public enum HealTarget { WEAKEST = 33, STRONGEST = 66}

    [SerializeField] NumberConverter converter;
    [Space]
    [SerializeField] Transform containerUnitHealthIdicators;
    [SerializeField] Transform LineOriginalUnitHealthIndicator;
    [Space]
    [SerializeField] TMP_Dropdown healTarget;
    [Space]
    [SerializeField] CanvasGroup healControl;

	void Awake ()
    {
        LineOriginalUnitHealthIndicator.gameObject.SetActive(false);

        HealTarget[] t = Enum.GetValues(typeof(HealTarget)) as HealTarget[];
        int cnt = t.Length;
        healTarget.options.Clear();
        for (int i = 0; i < cnt; i++)
            healTarget.options.Add(new TMP_Dropdown.OptionData(t[i].GetNameLocalize()));
        healTarget.value = 1;
        healTarget.value = 0;

        converter.OnCliclOk += Converter_OnCliclOk;
    }

    private void OnEnable()
    {
        if (Squad.playerSquadInstance != null)
        {
            var canHeal = ResetConverter();
            if (canHeal)
            {
                healControl.blocksRaycasts = true;
                healControl.alpha = 1;
            }
            else
            {
                healControl.blocksRaycasts = false;
                healControl.alpha = 0.7f;
            }
            AddNewLines();
        }
    }

    private void OnDisable()
    {
        RemoveOldLines();
    }

    private void Converter_OnCliclOk()
    {
        //sub silver and save progress
        GameManager.Instance.SavablePlayerData.PlayerProgress.Score.silver.Value -= converter.OutputValue;

        //heal the units
        List<Unit> weak = new List<Unit>();
        List<Unit> midle = new List<Unit>();
        List<Unit> strong = new List<Unit>();
        var pos = Squad.playerSquadInstance.UnitPositions;
        int cnt = pos.Count;
        float unitHealth = Squad.playerSquadInstance.DefaultUnitStats.Health;
        float health;
        switch (healTarget.value)
        {
            case 0:
                for (int i = 0; i < cnt; i++)
                {
                    health = pos[i].Unit.Stats.Health;
                    if (health / unitHealth * 100 <= (float)HealTarget.WEAKEST)
                        weak.Add(pos[i].Unit);
                    else if (health / unitHealth * 100 <= (float)HealTarget.STRONGEST)
                        midle.Add(pos[i].Unit);
                    else
                        strong.Add(pos[i].Unit);
                }
                break;
            case 1:
                for (int i = 0; i < cnt; i++)
                {
                    health = pos[i].Unit.Stats.Health;
                    if (health / unitHealth * 100 > (float)HealTarget.STRONGEST)
                        strong.Add(pos[i].Unit);
                    else if (health / unitHealth * 100 > (float)HealTarget.WEAKEST)
                        midle.Add(pos[i].Unit);
                    else
                        weak.Add(pos[i].Unit);
                }
                break;
            default:
                throw new Exception("Oups...");
                break;
        }

        float hlth = HealUnits(weak, converter.OutputValue);
        if(hlth > 0)
            hlth = HealUnits(midle, hlth);
        if (hlth > 0)
            HealUnits(strong, hlth);

        //reset converter values
        var canHeal = ResetConverter();
        if (!canHeal)
        {
            healControl.blocksRaycasts = false;
            healControl.alpha = 0.7f;
        }

        //refresh unit health indicators
        RereshLines();
    }

    bool ResetConverter()
    {
        var pos = Squad.playerSquadInstance.UnitPositions;
        int cnt = pos.Count;
        float unitHealth = Squad.playerSquadInstance.DefaultUnitStats.Health;

        float neededHealth = 0;
        for (int i = 0; i < cnt; i++)
            neededHealth += unitHealth - pos[i].Unit.Stats.Health;

        converter.OutputValue = neededHealth;
        float cost = converter.InputValue;

        float silver = GameManager.Instance.SavablePlayerData.PlayerProgress.Score.silver.Value;
        if (cost > silver)
            cost = silver;

        converter.SetMaxValues(0, cost);

        return cost > 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="units"></param>
    /// <param name="health"></param>
    /// <returns>возвращает лишнее здоровье (которое никому больше не надо)</returns>
    float HealUnits(List<Unit> units, float health)
    {
        int cnt = units.Count;
        float oldHealth;
        do
        {
            oldHealth = health;// для предотвращения бесконечного цикла (на вский случай!)
            float avg = health / cnt;
            for (int i = 0; i < cnt; i++)
            {
                float need = units[i].squad.DefaultUnitStats.Health - units[i].Stats.Health;
                if (avg > need)
                {
                    units[i].Heal(need);
                    health -= need;
                }
                else
                {
                    units[i].Heal(avg);
                    health -= avg;
                }
            }
        }
        while (health > 0 && oldHealth != health);
        return health;
    }

    void AddNewLines()
    {
        int rowLength = Squad.playerSquadInstance.SQUAD_LENGTH;

        int c = Squad.playerSquadInstance.UnitCount;
        int lines = c / rowLength;
        int cntInLastLine = c % rowLength;
        if (cntInLastLine > 0)
            lines++;

        Transform go = null;
        for (int i = 0; i < lines; i++)
        {
            go = Instantiate(LineOriginalUnitHealthIndicator, containerUnitHealthIdicators);
            go.gameObject.SetActive(true);
        }
        int countDoRemove = 0;
        if (cntInLastLine > 0)
        {
            countDoRemove = rowLength - cntInLastLine;
            for (int i = 0; i < countDoRemove; i++)
            {
                Destroy(go.GetChild(i).gameObject);
            }
        }

        RereshLines();
    }

    void RemoveOldLines()
    {
        int cnt = containerUnitHealthIdicators.childCount;
        if (cnt > 1)
            for (int i = 1; i < cnt; i++)
                Destroy(containerUnitHealthIdicators.GetChild(i).gameObject);
    }

    void RereshLines()
    {
        int rowLength = Squad.playerSquadInstance.SQUAD_LENGTH;
        int c = Squad.playerSquadInstance.UnitCount;
        int lines = c / rowLength;
        int cntInLastLine = c % rowLength;
        if (cntInLastLine > 0)
            lines++;

        int countToRemove = 0;
        if (cntInLastLine > 0)
            countToRemove = rowLength - cntInLastLine;

        int index = 0;
        var pos = Squad.playerSquadInstance.UnitPositions;
        int posCnt = pos.Count;

        float sumHealth = 0;

        Image r = null;
        for (int i = 0; i < lines; i++)
        {
            var l = containerUnitHealthIdicators.GetChild(i + 1);
            for (int j = 0; j < rowLength; j++)
            {
                index = i * rowLength + j;
                if (index < posCnt)
                {
                    if (i == lines - 1 && l.childCount + countToRemove > rowLength)
                        r = l.GetChild(j + countToRemove).GetComponent<Image>();
                    else
                        r = l.GetChild(j).GetComponent<Image>();

                    r.color = pos[index].Unit.GetColorByHealth();
                }
                else
                    return;
            }
        }
    }
}
