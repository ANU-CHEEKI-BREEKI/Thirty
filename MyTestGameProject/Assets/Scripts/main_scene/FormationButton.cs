﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FormationButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] Image currentFormationButton;
    [Space]
    [SerializeField] Image imageRanks;
    [SerializeField] GameObject ranksCell;
    [Space]
    [SerializeField] Image imagePhalanx;
    [SerializeField] GameObject phalanxCell;
    [Space]
    [SerializeField] Image imageShields;
    [SerializeField] GameObject shieldsCell;
    CanvasGroup thisCg;

    float angle;
    [SerializeField] Vector2 startP;
    [SerializeField] Vector2 endP;

    Transform thisTransform;

    bool mouseDown = false;
    int touchId;

    bool canUse = true;

    Squad playerSquad;

    public static FormationButton Instance { get; private set; }

    public Sprite GetIcon(FormationStats.Formations formation)
    {
        Sprite res = null;
        switch (formation)
        {
            case FormationStats.Formations.RANKS:
                res = imageRanks.sprite;
                break;
            case FormationStats.Formations.PHALANX:
                res = imagePhalanx.sprite;
                break;
            case FormationStats.Formations.RISEDSHIELDS:
                res = imageShields.sprite;
                break;
            default:
                break;
        }
        return res;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerSquad = Squad.playerSquadInstance;

        thisCg = GetComponent<CanvasGroup>();

        SetButtonsEnabled(false);

        thisTransform = transform;

        SetImage(playerSquad.CurrentFormation);


        playerSquad.OnFormationChanged += PlayerSquadInstance_OnFormationChanged;
        playerSquad.OnBeginCharge += PlayerSquad_OnBeginCharge;
        playerSquad.OnEndCharge += PlayerSquad_OnEndCharge;
    }

    private void OnDestroy()
    {
        if (playerSquad != null)
        {
            playerSquad.OnFormationChanged -= PlayerSquadInstance_OnFormationChanged;
            playerSquad.OnBeginCharge -= PlayerSquad_OnBeginCharge;
            playerSquad.OnEndCharge -= PlayerSquad_OnEndCharge;
        }
    }

    private void PlayerSquad_OnBeginCharge(UnitStatsModifyer m)
    {
        thisCg.alpha = 0.7f;
        thisCg.interactable = false;
        canUse = false;
    }

    private void PlayerSquad_OnEndCharge(UnitStatsModifyer m)
    {
        thisCg.alpha = 1;
        thisCg.interactable = true;
        canUse = true;
    }    

    private void PlayerSquadInstance_OnFormationChanged(FormationStats.Formations formation)
    {
        SetImage(playerSquad.CurrentFormation);
    }

    void SetImage(FormationStats.Formations formation)
    {
        switch (formation)
        {
            case FormationStats.Formations.RANKS:
                currentFormationButton.sprite = imageRanks.sprite;
                break;
            case FormationStats.Formations.PHALANX:
                currentFormationButton.sprite = imagePhalanx.sprite;
                break;
            case FormationStats.Formations.RISEDSHIELDS:
                currentFormationButton.sprite = imageShields.sprite;
                break;
        }
    }

    private void SetButtonsEnabled(bool val)
    {
        ranksCell.SetActive(val);
        phalanxCell.SetActive(val);
        shieldsCell.SetActive(val);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canUse)
        {
            SetButtonsEnabled(true);

            startP = Camera.main.WorldToScreenPoint(thisTransform.position);

            if (!mouseDown)
            {
                mouseDown = true;

                int count = Input.touchCount;
                Touch touch;
                if (count > 0)
                {
                    touch = Input.GetTouch(Input.touchCount - 1);
                    touchId = touch.fingerId;
                }
                else
                {
                    touch = new Touch();
                    touch.position = Input.mousePosition;
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (canUse)
        {
            int count = Input.touchCount;
            Touch touch;
            if (count > 0)
            {
                touch = Input.GetTouch(Input.touchCount - 1);
            }
            else
            {
                touch = new Touch();
                touch.position = Input.mousePosition;
            }

            if (touchId == touch.fingerId || count == 0)
            {
                mouseDown = false;

                endP = touch.position;
                angle = GetAngle(startP, endP);
                playerSquad.CurrentFormation = GetFormation(angle);
                SetImage(playerSquad.CurrentFormation);

                SetButtonsEnabled(false);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mouseDown && canUse)
        {
            int count = Input.touchCount;
            Touch touch = new Touch();
            if (count > 0)
            {
                foreach (var item in Input.touches)
                {
                    if (item.fingerId == touchId)
                    {
                        touch = item;
                        break;
                    }
                }
            }
            else
            {
                touch.position = Input.mousePosition;
            }

            endP = touch.position;
            angle = GetAngle(startP, endP);
            FormationStats.Formations f = GetFormation(angle);
            SetImage(f);
        }
    }

    FormationStats.Formations GetFormation(float angle)
    {
        FormationStats.Formations res;

        if (angle > 30)
        {
            res = FormationStats.Formations.RANKS;
        }
        else if(angle <= 30 && angle > -30)
        {
            res = FormationStats.Formations.PHALANX;
        }
        else // if(angle <= -30)
        {
            res = FormationStats.Formations.RISEDSHIELDS;
        }

        return res;
    }

    float GetAngle(Vector3 spart, Vector3 end)
    {
        Vector2 center = Camera.main.WorldToScreenPoint(imagePhalanx.transform.position);
        center = center - startP;
        center.Normalize();

        Vector2 direction = endP - startP;
        direction.Normalize();

        float res = Vector2.Angle(center, direction);
        if (direction.y < center.x - direction.x)
            res *= -1;

        return res;
    }


}
