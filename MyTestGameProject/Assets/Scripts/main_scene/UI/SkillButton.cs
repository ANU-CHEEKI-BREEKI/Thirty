using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup), typeof(Image), typeof(LineRenderer))]
public class SkillButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public enum SkillNum { FIRS, SECOND }
    public enum Type { SKILL, CONSUMABLE }

    [SerializeField] Type typeOfSkill;
    [Space]

    Executable.ExecatableUseType useType;
    [SerializeField] SkillNum skillNum;
    [SerializeField] TextMeshProUGUI cooldownText;
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] GameObject cell;

    Image img;
    CanvasGroup cg;
    Inventory playerInventory;
    Transform playerSquadTransform;

    SkillStack currentSkill;
    ConsumableStack currentConsumable;
    bool canCast;
    
    LineRenderer lineRenderer;
    float rHitCirclreAngle = 20;
    int rHitCirclreSegmentCount;
    Vector3[] rHitCirclreSegments;
    float[] sinAngles;
    float[] cosAngles;


    bool drag;
    int touchId;
    bool finger;

    bool consumableCallbackExpected = false;

    private void Start()
    {
        playerInventory = Squad.playerSquadInstance.Inventory;
        playerSquadTransform = Squad.playerSquadInstance.PositionsTransform;

        img = GetComponent<Image>();
        cg = GetComponent<CanvasGroup>();

        lineRenderer = gameObject.GetComponent<LineRenderer>();

        rHitCirclreSegmentCount = (int)(360 / rHitCirclreAngle) + 1;
        rHitCirclreSegments = new Vector3[rHitCirclreSegmentCount];
        sinAngles = new float[rHitCirclreSegmentCount];
        cosAngles = new float[rHitCirclreSegmentCount];

        CreateSinCosAngles();

        canCast = true;

        cooldownText.text = string.Empty;

        Initiate();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();

        if (typeOfSkill == Type.CONSUMABLE && currentConsumable != null && currentConsumable.Consumable != null)
            currentConsumable.Consumable.CallbackUsedCount -= DecrementUsedConsumableCount;
    }

    private void Initiate()
    {
        if (typeOfSkill == Type.SKILL)
        {
            switch (skillNum)
            {
                case SkillNum.FIRS:
                    currentSkill = playerInventory.FirstSkill;
                    break;
                case SkillNum.SECOND:
                    currentSkill = playerInventory.SecondSkill;
                    break;
            }

            if (currentSkill.Skill != null)
            {
                img.sprite = currentSkill.Skill.MainPropertie.Icon;
                useType = currentSkill.Skill.UseType;
            }
        }
        else
        {
            switch (skillNum)
            {
                case SkillNum.FIRS:
                    currentConsumable = playerInventory.FirstConsumable;
                    break;
                case SkillNum.SECOND:
                    currentConsumable = playerInventory.SecondConsumable;
                    break;
            }
            if (currentConsumable.Consumable != null)
            {
                useType = currentConsumable.Consumable.UseType;
                img.sprite = currentConsumable.Consumable.MainPropertie.Icon;
                currentConsumable.Consumable.CallbackUsedCount += DecrementUsedConsumableCount;
            }
        }

        if ((currentSkill != null && currentSkill.Skill == null) || (currentConsumable != null && currentConsumable.Consumable == null))
        {
            Destroy(cell);
            return;
        }
        else
        {
            if (typeOfSkill == Type.CONSUMABLE)
            {
                if (currentConsumable.Count < 0)
                    countText.enabled = false;
                else
                    countText.text = currentConsumable.Count.ToString(StringFormats.intNumber);
            }
            else
                countText.enabled = false;
        }
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        if (typeOfSkill == Type.SKILL)
            OnPointerClickSkill(eventData);
        else
            OnPointerClickConsumable(eventData);            
    }

    void OnPointerClickSkill(PointerEventData eventData)
    {
        if (useType == Executable.ExecatableUseType.CLICK && Squad.playerSquadInstance != null && canCast)
        {
            currentSkill.Skill.Init(Squad.playerSquadInstance);
            DoSkill();
        }
    }

    void OnPointerClickConsumable(PointerEventData eventData)
    {
        if (useType == Executable.ExecatableUseType.CLICK && Squad.playerSquadInstance != null && canCast)
        {
            currentConsumable.Consumable.Init(Squad.playerSquadInstance);
            DoConsumable();
        }
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        if (drag == false)
        {
            drag = true;

            if (Input.touchCount > 0)
            {
                touchId = Input.GetTouch(Input.touchCount - 1).fingerId;
                finger = true;
            }
            else
            {
                finger = false;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Touch touch = new Touch() { position = Input.mousePosition };
        foreach (var item in Input.touches)
        {
            if (item.fingerId == touchId)
            {
                touch = item;
                break;
            }
        }

        if (!finger || touch.phase == TouchPhase.Ended)
        {
            drag = false;

            if (typeOfSkill == Type.SKILL)
                OnEndDragSkill(touch.position);
            else
                OnEndDragConsumable(touch.position);
        }
    }

    void OnEndDragSkill(Vector2 position)
    {
        if (useType == Executable.ExecatableUseType.DRAG_DROP_PLASE && Squad.playerSquadInstance != null && canCast)
        {
            currentSkill.Skill.Init(
                Squad.playerSquadInstance,
                (Vector2)Camera.main.ScreenToWorldPoint(position),
                playerSquadTransform.rotation
            );
            DoSkill();
            lineRenderer.positionCount = 0;
        }
        else if (useType == Executable.ExecatableUseType.DRAG_DIRECTION && Squad.playerSquadInstance != null && canCast)
        {
            if (currentSkill.Skill is ISkillDirectionable)
            {
                ISkillDirectionable t = currentSkill.Skill as ISkillDirectionable;

                currentSkill.Skill.Init(
                    Squad.playerSquadInstance,
                    ((Vector2)Camera.main.ScreenToWorldPoint(position) - Squad.playerSquadInstance.CenterSquad).normalized * t.Distance + Squad.playerSquadInstance.CenterSquad
                );
                DoSkill();
                lineRenderer.positionCount = 0;
            }
        }
    }

    void OnEndDragConsumable(Vector2 position)
    {
        if (useType == Executable.ExecatableUseType.DRAG_DROP_PLASE && Squad.playerSquadInstance != null && canCast)
        {
            currentConsumable.Consumable.Init(
                Squad.playerSquadInstance,
                (Vector2)Camera.main.ScreenToWorldPoint(position),
                playerSquadTransform.rotation,
                currentConsumable.Count
            );
            DoConsumable();
            lineRenderer.positionCount = 0;
        }
        else if (useType == Executable.ExecatableUseType.DRAG_DIRECTION && Squad.playerSquadInstance != null && canCast)
        {
            if (currentConsumable.ConsumableStats is ISkillDirectionable)
            {
                ISkillDirectionable t = currentConsumable.ConsumableStats as ISkillDirectionable;

                currentConsumable.Consumable.Init(
                    Squad.playerSquadInstance,
                    ((Vector2)Camera.main.ScreenToWorldPoint(position) - Squad.playerSquadInstance.CenterSquad).normalized * t.Distance + Squad.playerSquadInstance.CenterSquad,
                    currentConsumable.Count
                );
                DoConsumable();
                lineRenderer.positionCount = 0;
            }
        }
    }



    private void Update()
    {
        if (drag)
        {
            Touch touch = new Touch() { position = Input.mousePosition };
            foreach (var item in Input.touches)
            {
                if (item.fingerId == touchId)
                {
                    touch = item;
                    break;
                }
            }

            if (typeOfSkill == Type.SKILL)
                OnDragSkill(touch.position);
            else
                OnDragConsumable(touch.position);
        }
    }

    void OnDragSkill(Vector2 position)
    {
        if (useType == Executable.ExecatableUseType.DRAG_DROP_PLASE && Squad.playerSquadInstance != null && canCast)
        {
            if (currentSkill.SkillStats is ISkillRadiusable)
            {
                ISkillRadiusable t = currentSkill.SkillStats as ISkillRadiusable;

                CreatePoints(Camera.main.ScreenToWorldPoint(position), t.Radius);
                lineRenderer.positionCount = rHitCirclreSegmentCount;
                lineRenderer.SetPositions(rHitCirclreSegments);
            }
        }
        else if (useType == Executable.ExecatableUseType.DRAG_DIRECTION && Squad.playerSquadInstance != null && canCast)
        {
            if (currentSkill.Skill is ISkillDirectionable)
            {
                ISkillDirectionable t = currentSkill.Skill as ISkillDirectionable;

                lineRenderer.positionCount = 2;
                var pos = new Vector3[] {
                    Squad.playerSquadInstance.CenterSquad,
                    ((Vector2)Camera.main.ScreenToWorldPoint(position) - Squad.playerSquadInstance.CenterSquad).normalized * t.Distance + Squad.playerSquadInstance.CenterSquad
                };
                for (int i = 0; i < pos.Length; i++)
                    pos[i].z = -9;

                lineRenderer.SetPositions(pos);
            }
        }
    }

    void OnDragConsumable(Vector2 position)
    {
        if (useType == Executable.ExecatableUseType.DRAG_DROP_PLASE && Squad.playerSquadInstance != null && canCast)
        {
            if (currentConsumable.ConsumableStats is ISkillRadiusable)
            {
                ISkillRadiusable t = currentConsumable.ConsumableStats as ISkillRadiusable;

                CreatePoints(Camera.main.ScreenToWorldPoint(position), t.Radius);
                lineRenderer.positionCount = rHitCirclreSegmentCount;
                lineRenderer.SetPositions(rHitCirclreSegments);
            }
        }
        else if (useType == Executable.ExecatableUseType.DRAG_DIRECTION && Squad.playerSquadInstance != null && canCast)
        {
            if (currentConsumable.ConsumableStats is ISkillDirectionable)
            {
                ISkillDirectionable t = currentConsumable.ConsumableStats as ISkillDirectionable;

                lineRenderer.positionCount = 2;
                var pos = new Vector3[] {
                    Squad.playerSquadInstance.CenterSquad,
                    ((Vector2)Camera.main.ScreenToWorldPoint(position) - Squad.playerSquadInstance.CenterSquad).normalized * t.Distance + Squad.playerSquadInstance.CenterSquad
                };
                for (int i = 0; i < pos.Length; i++)
                    pos[i].z = -9;

                lineRenderer.SetPositions(pos);
            }
        }
    }
           
    

    void DoSkill()
    {
        if (currentSkill.Skill.Execute(currentSkill.SkillStats))
        {
            ISkillCooldownable c = currentSkill.SkillStats as ISkillCooldownable;
            if (c != null)
                StartCoroutine(WaitForCooldown(c.Cooldown));
        }
    }

    void DoConsumable()
    {
        consumableCallbackExpected = true;
        if (currentConsumable.Consumable.Execute(currentConsumable.ConsumableStats))
        {
            
            ISkillCooldownable c = currentConsumable.ConsumableStats as ISkillCooldownable;
            if (c != null)
                StartCoroutine(WaitForCooldown(c.Cooldown));
        }
        else
        {
            consumableCallbackExpected = false;
        }
    }

    void DecrementUsedConsumableCount(int count, Squad owner)
    {
        if (owner == Squad.playerSquadInstance && consumableCallbackExpected)
        {
            currentConsumable.Count -= count;
            consumableCallbackExpected = false;

            if (currentConsumable.Count <= 0)
                Destroy(cell);
            else
                countText.text = currentConsumable.Count.ToString(StringFormats.intNumber);
        }
    }

    IEnumerator WaitForCooldown(float time)
    {
        float t = time;

        canCast = false;
        cg.interactable = false;

        while (t > 0)
        {
            cooldownText.text = t.ToString(StringFormats.intNumber);
            yield return new WaitForSeconds(0.1f);
            t -= 0.1f;
        }

        cooldownText.text = string.Empty;
        cg.interactable = true;
        canCast = true;
    }
    
    void CreateSinCosAngles()
    {
        float angle = rHitCirclreAngle;
        for (int i = 0; i < (rHitCirclreSegmentCount); i++)
        {
            sinAngles[i] = Mathf.Sin(Mathf.Deg2Rad * angle);
            cosAngles[i] = Mathf.Cos(Mathf.Deg2Rad * angle);
            angle += rHitCirclreAngle;
        }
    }

    void CreatePoints(Vector2 center, float radius)
    {
        float x;
        float y;

        for (int i = 0; i < (rHitCirclreSegmentCount); i++)
        {
            x = sinAngles[i] * radius + center.x;
            y = cosAngles[i] * radius + center.y;

            rHitCirclreSegments[i] = new Vector3(x, y, -9);
        }
    }
}
