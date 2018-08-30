using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScorePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gold;
    [SerializeField] TextMeshProUGUI silver;
    [SerializeField] TextMeshProUGUI expirience;
    [Space]
    [SerializeField] public bool scaledDeltaTimeInPopUpTextController;
    [Space]
    [SerializeField] bool isTempValues;
    [Space]
    [SerializeField] public float summTextTime = 1;
    [SerializeField] public Vector2 popUpTextSpeed = new Vector2(.5f, -1);
    [SerializeField] public float  popUpTextLifetime = 0.75f;

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

        var score = GameManager.Instance.PlayerProgress.Score;

        Reset(isTempValues, scaledDeltaTimeInPopUpTextController);

        PopUpTextController.Instance.ScaledDeltaTime = scaledDeltaTimeInPopUpTextController;
    }

    void OnDestroy()
    {
        if (gold != null)
        {
            if (!isTempValues)
                GameManager.Instance.PlayerProgress.Score.gold.OnValueChanged -= RefreshByEventGold;
            else
                GameManager.Instance.PlayerProgress.Score.tempGold.OnValueChanged -= RefreshByEventGold;
        }

        if (silver != null)
        {
            if (!isTempValues)
                GameManager.Instance.PlayerProgress.Score.silver.OnValueChanged -= RefreshByEventSilver;
            else
                GameManager.Instance.PlayerProgress.Score.tempSilver.OnValueChanged -= RefreshByEventSilver;
        }

        if (expirience != null)
        {
            if (!isTempValues)
                GameManager.Instance.PlayerProgress.Score.expirience.OnValueChanged -= RefreshByEventExpirience;
            else
                GameManager.Instance.PlayerProgress.Score.tempExpirience.OnValueChanged -= RefreshByEventExpirience;
        }
    }

    public void Reset(bool asTempValues = false, bool scaledTime = false, bool setStartValuesWithDelay = false, bool subscribeToEvents = true)
    {
        var score = GameManager.Instance.PlayerProgress.Score;

        Unsubscribe(isTempValues);
        if(subscribeToEvents)
            Subscribe(asTempValues);
        isTempValues = asTempValues;

        scaledDeltaTimeInPopUpTextController = scaledTime;
        PopUpTextController.Instance.ScaledDeltaTime = scaledDeltaTimeInPopUpTextController;

        if (!setStartValuesWithDelay)
        {
            if (!asTempValues)
            {
                if (gold != null)
                    gold.text = score.gold.Value.ToString(StringFormats.intSeparatorNumber, StringFormats.nfi);

                if (silver != null)
                    silver.text = score.silver.Value.ToString(StringFormats.intSeparatorNumber, StringFormats.nfi);

                if (expirience != null)
                    expirience.text = score.expirience.Value.ToString(StringFormats.intSeparatorNumber, StringFormats.nfi);
            }
            else
            {
                if (gold != null)
                    gold.text = score.tempGold.Value.ToString(StringFormats.intSeparatorSignNumber, StringFormats.nfi);

                if (silver != null)
                    silver.text = score.tempSilver.Value.ToString(StringFormats.intSeparatorSignNumber, StringFormats.nfi);

                if (expirience != null)
                    expirience.text = score.tempExpirience.Value.ToString(StringFormats.intSeparatorSignNumber, StringFormats.nfi);
            }
        }
        else
        {
            if (!asTempValues)
            {
                if (gold != null)
                {
                    gold.text = 0.ToString(StringFormats.intSeparatorNumber, StringFormats.nfi);
                    RefreshByEventGold(0, score.gold.Value, new DSPlayerScore.Score() { Value = score.gold.Value });
                }

                if (silver != null)
                {
                    silver.text = 0.ToString(StringFormats.intSeparatorNumber, StringFormats.nfi);
                    RefreshByEventSilver(0, score.silver.Value, new DSPlayerScore.Score() { Value = score.silver.Value });
                }

                if (expirience != null)
                {
                    expirience.text = 0.ToString(StringFormats.intSeparatorNumber, StringFormats.nfi);
                    RefreshByEventExpirience(0, score.expirience.Value, new DSPlayerScore.Score() { Value = score.expirience.Value });
                }
            }
            else
            {
                if (gold != null)
                {
                    gold.text = 0.ToString(StringFormats.intSeparatorSignNumber, StringFormats.nfi);
                    RefreshByEventGold(0, score.tempGold.Value, new DSPlayerScore.Score() { Value = score.tempGold.Value });
                }

                if (silver != null)
                {
                    silver.text = 0.ToString(StringFormats.intSeparatorSignNumber, StringFormats.nfi);
                    RefreshByEventSilver(0, score.tempSilver.Value, new DSPlayerScore.Score() { Value = score.tempSilver.Value });
                }

                if (expirience != null)
                {
                    expirience.text = 0.ToString(StringFormats.intSeparatorSignNumber, StringFormats.nfi);
                    RefreshByEventExpirience(0, score.tempExpirience.Value, new DSPlayerScore.Score() { Value = score.tempExpirience.Value });
                }
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rTransform);

    }

    void Subscribe(bool isTempValues)
    {
        var score = GameManager.Instance.PlayerProgress.Score;

        if (!isTempValues)
        {
            if (gold != null)
                score.gold.OnValueChanged += RefreshByEventGold;

            if (silver != null)
                score.silver.OnValueChanged += RefreshByEventSilver;

            if (expirience != null)
                score.expirience.OnValueChanged += RefreshByEventExpirience;
        }
        else
        {
            if (gold != null)
                score.tempGold.OnValueChanged += RefreshByEventGold;

            if (silver != null)
                score.tempSilver.OnValueChanged += RefreshByEventSilver;

            if (expirience != null)
                score.tempExpirience.OnValueChanged += RefreshByEventExpirience;
        }
    }

    void Unsubscribe(bool isTempValues)
    {
        var score = GameManager.Instance.PlayerProgress.Score;

        if (!isTempValues)
        {
            if (gold != null)
                score.gold.OnValueChanged -= RefreshByEventGold;

            if (silver != null)
                score.silver.OnValueChanged -= RefreshByEventSilver;

            if (expirience != null)
                score.expirience.OnValueChanged -= RefreshByEventExpirience;
        }
        else
        {
            if (gold != null)
                score.tempGold.OnValueChanged -= RefreshByEventGold;

            if (silver != null)
                score.tempSilver.OnValueChanged -= RefreshByEventSilver;

            if (expirience != null)
                score.tempExpirience.OnValueChanged -= RefreshByEventExpirience;
        }
    }

    void RefreshByEventGold(float oldValue, float newValue, DSPlayerScore.Score score)
    {
        if (!gameObject.activeInHierarchy)
            return;

        tempGold.val += newValue - oldValue;

        if (goldCo == null)
            goldCo = StartCoroutine(PopTextCo(tempGold, gold, score));
    }

    void RefreshByEventSilver(float oldValue, float newValue, DSPlayerScore.Score score)
    {
        if (!gameObject.activeInHierarchy)
            return;

        tempSilv.val += newValue - oldValue;

        if (silvCo == null)
            silvCo = StartCoroutine(PopTextCo(tempSilv, silver, score));
    }

    void RefreshByEventExpirience(float oldValue, float newValue, DSPlayerScore.Score score)
    {
        if (!gameObject.activeInHierarchy)
            return;

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
                screenFloatSpeed: popUpTextSpeed,
                startColor: color,
                endColor: color,
                sortingOrder: 1000,
                fontSize: fontSize - 10,
                lifetime: popUpTextLifetime
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
            if (isTempValues)
                format = StringFormats.intSeparatorSignNumber;
        }
        else if (textUGUI == silver)
        {
            silvCo = null;
            if (isTempValues)
                format = StringFormats.intSeparatorSignNumber;
        }
        else if (textUGUI == expirience)
        {
            expCo = null;
            if (isTempValues)
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
}