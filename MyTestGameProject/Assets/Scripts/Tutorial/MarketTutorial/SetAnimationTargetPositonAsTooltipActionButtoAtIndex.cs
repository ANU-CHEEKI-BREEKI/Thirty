using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Класс-костыль для туториала
/// </summary>
[RequireComponent(typeof(RelativeAnimation))]
public class SetAnimationTargetPositonAsTooltipActionButtoAtIndex : MonoBehaviour
{
    RelativeAnimation animation;
    [Tooltip("ндекс кнопки TipsPanel")]
    [SerializeField] int index;

    bool inited = false;
    [SerializeField] UnityEvent TipsPanelOnClickOk;

    Button button;

    void Start()
    {
        inited = true;
        StartCoroutine(Set());
    }

    void OnEnable()
    {
        StartCoroutine(Set());
    }

    void OnDisable()
    {
        if (button != null)
            button.onClick.RemoveListener(TipsPanelOnClickOk.Invoke);
    }

    IEnumerator Set()
    {
        if (inited)
        {
            yield return null;

            animation = GetComponent<RelativeAnimation>();
            var target = TipsPanel.Instance.GetActionButtonAt(index)?.transform;
            if (target != null)
                foreach (var point in animation.Points)
                    point.positionTransform = target;

            if(target != null)
            {
                button = target.GetComponent<Button>();
                if(button != null)
                {
                    button.onClick.AddListener(TipsPanelOnClickOk.Invoke);
                }
            }
        }
    }

}
