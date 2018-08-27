using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScorePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gold;
    [SerializeField] bool isTempGoldVal;
    [Space]
    [SerializeField] TextMeshProUGUI silver;
    [SerializeField] bool isTempSilverVal;
    [Space]
    [SerializeField] TextMeshProUGUI expirience;
    [SerializeField] bool isTempExpirienceVal;
    [Space]
    [SerializeField] bool scaledDeltaTimeInPopUpTextController;
    [Space]
    [SerializeField] float summTextTime = 1;

    Coroutine goldCo = null;
    Coroutine silvCo = null;
    Coroutine expCo = null;

    //переменные для суммирования изменений счета.
    //для того чтобы поп ап текст не накладывался друг на друга
    T tempGold;
    T tempSilv;
    T tempExp;

    RectTransform rTransform;

    static public ScorePanel Instance { get; private set; }

    private void Awake()
    {
        rTransform = GetComponent<RectTransform>();
        Instance = this;

        tempGold = new T();
        tempSilv = new T();
        tempExp = new T();
    }

    void Start()
    {
        var score = GameManager.Instance.PlayerProgress.Score;

        if (gold != null)
        {
            if (!isTempGoldVal)
            {
                score.gold.OnValueChanged += RefreshByEventGold;
                RefreshByEventGold(score.gold.Value, score.gold.Value, score.gold);
            }
            else
            {
                score.tempGold.OnValueChanged += RefreshByEventGold;
                RefreshByEventGold(score.tempGold.Value, score.tempGold.Value, score.tempGold);
            }
        }

        if (silver != null)
        {
            if (!isTempSilverVal)
            {
                score.silver.OnValueChanged += RefreshByEventSilver;
                RefreshByEventSilver(score.silver.Value, score.silver.Value, score.silver);
            }
            else
            {
                score.tempSilver.OnValueChanged += RefreshByEventSilver;
                RefreshByEventSilver(score.tempSilver.Value, score.tempSilver.Value, score.tempSilver);
            }
        }

        if (expirience != null)
        {
            if (!isTempSilverVal)
            {
                score.expirience.OnValueChanged += RefreshByEventExpirience;
                RefreshByEventExpirience(score.expirience.Value, score.expirience.Value, score.expirience);
            }
            else
            {
                score.tempExpirience.OnValueChanged += RefreshByEventExpirience;
                RefreshByEventExpirience(score.tempExpirience.Value, score.tempExpirience.Value, score.tempExpirience);
            }
        }

        PopUpTextController.Instance.ScaledDeltaTime = scaledDeltaTimeInPopUpTextController;
    }

    void OnDestroy()
    {
        if (gold != null)
        {
            if (!isTempGoldVal)
                GameManager.Instance.PlayerProgress.Score.gold.OnValueChanged -= RefreshByEventGold;
            else
                GameManager.Instance.PlayerProgress.Score.tempGold.OnValueChanged -= RefreshByEventGold;
        }

        if (silver != null)
        {
            if (!isTempSilverVal)
                GameManager.Instance.PlayerProgress.Score.silver.OnValueChanged -= RefreshByEventSilver;
            else
                GameManager.Instance.PlayerProgress.Score.tempSilver.OnValueChanged -= RefreshByEventSilver;
        }

        if (expirience != null)
        {
            if (!isTempSilverVal)
                GameManager.Instance.PlayerProgress.Score.expirience.OnValueChanged -= RefreshByEventExpirience;
            else
                GameManager.Instance.PlayerProgress.Score.tempExpirience.OnValueChanged -= RefreshByEventExpirience;
        }
    }

    void RefreshByEventGold(float oldValue, float newValue, DSPlayerScore.Score score)
    {
        tempGold.val += newValue - oldValue;

        if (goldCo == null)
            goldCo = StartCoroutine(PopTextCo(tempGold, gold, score));
    }

    void RefreshByEventSilver(float oldValue, float newValue, DSPlayerScore.Score score)
    {
        tempSilv.val += newValue - oldValue;

        if (silvCo == null)
            silvCo = StartCoroutine(PopTextCo(tempSilv, silver, score));
    }

    void RefreshByEventExpirience(float oldValue, float newValue, DSPlayerScore.Score score)
    {
        tempExp.val += newValue - oldValue;

        if(expCo == null)
            expCo = StartCoroutine(PopTextCo(tempExp, expirience, score));
    }

    void PopText(float value, TextMeshProUGUI textUGUI)
    {
        if (value != 0)
        {
            Color color;

            string text = value.ToString(StringFormats.intSeparatorSignNumber, StringFormats.nfi);

            if (value <= 0)
                color = Color.red;
            else
                color = Color.green;

            Vector2 pos = textUGUI.transform.position;
            float fontSize = textUGUI.fontSize;
            pos.y -= 1;
            //pos.x += 1;

            PopUpTextController.Instance.AddTextLabel(
                text,
                pos,
                screenFloatSpeed: new Vector2(.5f, -1),
                startColor: color,
                endColor: color,
                sortingOrder: 1,
                fontSize: fontSize - 10
            );
        }
    }

    IEnumerator PopTextCo(T value, TextMeshProUGUI textUGUI, DSPlayerScore.Score score)
    {
        if (scaledDeltaTimeInPopUpTextController)
            yield return new WaitForSeconds(summTextTime);
        else
            yield return new WaitForSecondsRealtime(summTextTime);

        PopText(value.val, textUGUI);

        string format = StringFormats.intSeparatorNumber;        

        if (textUGUI == gold)
        {
            goldCo = null;
            if (isTempGoldVal)
                format = StringFormats.intSeparatorSignNumber;
        }
        else if (textUGUI == silver)
        {
            silvCo = null;
            if (isTempSilverVal)
                format = StringFormats.intSeparatorSignNumber;
        }
        else if (textUGUI == expirience)
        {
            expCo = null;
            if (isTempExpirienceVal)
                format = StringFormats.intSeparatorSignNumber;
        }

        textUGUI.text = score.Value.ToString(format, StringFormats.nfi);

        value.val = 0;

        LayoutRebuilder.ForceRebuildLayoutImmediate(rTransform);
    }

    public class T
    {
        public float val;
    }

    //void RefreshByEvent(float oldValue, float newValue, DSPlayerScore.Score score)
    //{        
    //    DSPlayerScore playerScore = GameManager.Instance.PlayerProgress.Score;

    //    if (score != null)
    //    {
    //        float deltaScore = score.Value - oldValue;

    //        if (deltaScore != 0)
    //        {
    //            Color color;

    //            string text = deltaScore.ToString(StringFormats.intSeparatorNumber, StringFormats.nfi);

    //            if (deltaScore <= 0)
    //            {
    //                color = Color.red;
    //            }
    //            else
    //            {
    //                color = Color.green;
    //                text = "+" + text;
    //            }

    //            Vector2 pos = Vector2.zero;
    //            float fontSize = 20;

    //            if (score == playerScore.gold)
    //            {
    //                pos = gold.transform.position;
    //                fontSize = gold.fontSize;
    //            }
    //            else if (score == playerScore.silver)
    //            {
    //                pos = silver.transform.position;
    //                fontSize = silver.fontSize;
    //            }
    //            else if (score == playerScore.expirience)
    //            {
    //                pos = expirience.transform.position;
    //                fontSize = expirience.fontSize;
    //            }
    //            pos.y -= 1;

    //            PopUpTextController.Instance.AddTextLabel(
    //                text,
    //                pos,
    //                1,
    //                new Vector2(1, -2),
    //                color,
    //                color,
    //                1,
    //                fontSize - 10
    //            );
    //        }
    //    }

    //    gold.text = playerScore.gold.Value.ToString(StringFormats.intSeparatorNumber, StringFormats.nfi);
    //    silver.text = playerScore.silver.Value.ToString(StringFormats.intSeparatorNumber, StringFormats.nfi);
    //    expirience.text = playerScore.expirience.Value.ToString(StringFormats.intSeparatorNumber, StringFormats.nfi);

    //    LayoutRebuilder.ForceRebuildLayoutImmediate(rTransform);

    //}
}