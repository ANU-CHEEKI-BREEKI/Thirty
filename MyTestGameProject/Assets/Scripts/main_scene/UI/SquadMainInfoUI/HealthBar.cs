using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image hpBar;

    [SerializeField] TextMeshProUGUI textHp;
    [SerializeField] TextMeshProUGUI textCount;

    [SerializeField] Squad squad;
    public Squad Squad { get { return squad; } set { squad = value; } }

    bool active = true;
    public bool Active
    {
        get { return active; }
        set
        {
            active = value;
            if(active)
            {
                if (squad != null)
                {
                    DrawHpBar(squad.SquadHealth);
                    DrawCountUnits(squad.UnitCount);
                }
            }
        }
    }

    void Start()
    {
        if(squad == null)
            squad = Squad.playerSquadInstance;

        squad.OnSumHealthChanged += DrawHpBar;
        squad.OnUitCountChanged += DrawCountUnits;

        DrawHpBar(squad.SquadHealth);
        DrawCountUnits(squad.UnitCount);

        hpBar.type = Image.Type.Filled;
        hpBar.fillMethod = Image.FillMethod.Horizontal;

        DrawHpBar(squad.SquadHealth);
    }

    void OnEnable()
    {
        if (squad != null)
        {
            DrawHpBar(squad.SquadHealth);
            DrawCountUnits(squad.UnitCount);
        }
    }

    void OnDestroy()
    {
        if (squad != null)
        {
            squad.OnSumHealthChanged -= DrawHpBar;
            squad.OnUitCountChanged -= DrawCountUnits;
        }
    }

    void DrawCountUnits(int newCount)
    {
        if (squad != null && gameObject.activeInHierarchy)
        {
            if(textCount != null)
                textCount.text = newCount + "/" + squad.FULL_SQUAD_UNIT_COUNT;
        }
    }

    void DrawHpBar(float squadHealth)
    {
        if (squad != null && gameObject.activeInHierarchy && Active)
        {
            float maxHp = squad.DefaultUnitStats.Health * squad.FULL_SQUAD_UNIT_COUNT;

            float t = squadHealth / maxHp;
            Color color;
            if (t >= 0.5f)
                color = Color.Lerp(Color.yellow, Color.green, t * 2 - 1 - 0.3f);
            else
                color = Color.Lerp(Color.red, Color.yellow, t * 2 - 0.3f);

            hpBar.color = color;
            hpBar.fillAmount = t;

            if(textHp != null)
                textHp.text = squadHealth.ToString("0") + "/" + maxHp;
        }
    }
}

