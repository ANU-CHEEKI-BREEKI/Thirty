using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnvirinmantInventoryUI : AInventoryUI
{
    public static EnvirinmantInventoryUI Instance { get; set; }

    [SerializeField] GameObject inventoryItemOriginal;
       
    [SerializeField] Transform envaironmentInventory;
    Transform[] envaironmentCells;

    [SerializeField] [Range(0, 2000)] int maxRhitsCount = 1000;
    [SerializeField] [Range(0, 50)] int radiusToFind = 20;
    public int RadiusToFind { get { return radiusToFind; } }

    [Header("LineRender")]
    [SerializeField]
    [Range(0, 10)]
    float delayBeforeDrawPickUpArea = 1;
    [SerializeField] [Range(0, 30)] float rHitCirclreAngle = 20;
    
    List<EquipmentStack> inventory;
    public bool Fill { get { return inventory.Count == envaironmentCells.Length; } }

    RaycastHit2D[] rhits;
    ContactFilter2D rHitFilter;
    int hitCount;

    LineRenderer lineRenderer;
    int rHitCirclreSegmentCount;
    Vector3[] rHitCirclreSegments;
    float[] sinAngles;
    float[] cosAngles;
    bool toggleDown = false;

    bool draw = false;

    bool canRefresh = true;
    [Space]
    [SerializeField] float timeToRefresh = 0.2f;

    void Awake()
    {
        Instance = this;

        int cnt = envaironmentInventory.childCount;
        envaironmentCells = new Transform[cnt];
        for (int i = 0; i < cnt; i++)
            envaironmentCells[i] = envaironmentInventory.GetChild(i);

        inventory = new List<EquipmentStack>(cnt);
    }

    private void Start()
    {
        rhits = new RaycastHit2D[maxRhitsCount];

        int rHitLayer = 1 << LayerMask.NameToLayer("ITEM");
        rHitFilter = new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = new LayerMask()
            {
                value = rHitLayer
            }
        };

        lineRenderer = gameObject.GetComponent<LineRenderer>();

        rHitCirclreSegmentCount = (int)(360 / rHitCirclreAngle) + 1;
        rHitCirclreSegments = new Vector3[rHitCirclreSegmentCount];
        sinAngles = new float[rHitCirclreSegmentCount];
        cosAngles = new float[rHitCirclreSegmentCount];

        CreateSinCosAngles();

        Unit.OnAnyUnitDeath += RefreshOnUnitDeath;
    }

    private void OnDestroy()
    {
        Unit.OnAnyUnitDeath -= RefreshOnUnitDeath;
    }

    void RefreshOnUnitDeath()
    {
        if (canRefresh)
            StartCoroutine(WaitForRefresh());
    }

    IEnumerator WaitForRefresh()
    {
        canRefresh = false;
        yield return new WaitForSeconds(timeToRefresh);
        canRefresh = true;
        RefreshUI();
    }

    private void Update()
    {
        if (toggleDown && draw)
        {
            CreatePoints();
            lineRenderer.positionCount = rHitCirclreSegmentCount;
            lineRenderer.SetPositions(rHitCirclreSegments);
        }
    }

    public void ContentToggle_OnUp()
    {
        toggleDown = false;
        draw = false;
        lineRenderer.positionCount = 0;
    }

    public void ContentToggle_OnDown()
    {
        toggleDown = true;
        StartCoroutine(DrawPickUpArea());
    }

    IEnumerator DrawPickUpArea()
    {
        float time = 0;

        while (time < delayBeforeDrawPickUpArea)
        {
            if (!toggleDown)
                break;

            time += Time.deltaTime;
            yield return null;
        }

        if (toggleDown)
            draw = true;
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

    void CreatePoints()
    {
        float x;
        float y;

        for (int i = 0; i < (rHitCirclreSegmentCount); i++)
        {
            x = sinAngles[i] * radiusToFind + Squad.playerSquadInstance.CenterSquad.x;
            y = cosAngles[i] * radiusToFind + Squad.playerSquadInstance.CenterSquad.y;

            rHitCirclreSegments[i] = new Vector3(x, y, -9);
        }
    }

    public void FillInventory(EquipmentStack stack)
    {
        bool finded = false;
        foreach (var inventoryStack in inventory)
        {
            if (inventoryStack.EquipmentStats.Type == stack.EquipmentStats.Type 
                && inventoryStack.EquipmentStats.Id == stack.EquipmentStats.Id
                && inventoryStack.EquipmentStats.ItemDurability == stack.EquipmentStats.ItemDurability)
            {
                inventoryStack.PushItems(stack);
                finded = true;
                break;
            }
        }

        if (!finded)
            inventory.Add(new EquipmentStack(stack));
    }

    public void PickUpEquipment(EquipmentStack stack)
    {
        int q = 0;
        for (int i = 0; i < hitCount; i++)
        {
            DroppedItem di = rhits[i].transform.GetComponent<DroppedItem>();

            if (di.Stack.EquipmentStats.Type == stack.EquipmentStats.Type 
                && di.Stack.EquipmentStats.Id == stack.EquipmentStats.Id
                && di.Stack.EquipmentStats.ItemDurability == stack.EquipmentStats.ItemDurability)
            {
                q += di.Stack.Count;

                if (q > stack.Count)
                {
                    di.Stack.PopItems(di.Stack.Count - (q - stack.Count));
                    q = stack.Count;
                }
                else
                    Destroy(di.gameObject);
            }

            if (q == stack.Count)
                break;
        }
    }

    public void FindItems()
    {
        hitCount = Physics2D.CircleCast(Squad.playerSquadInstance.CenterSquad, radiusToFind, Vector2.zero, rHitFilter, rhits);

        int cnt = inventory.Count;
        for (int i = 0; i < cnt; i++)
            inventory[i].PopItems(inventory[i].Count);

        for (int i = 0; i < hitCount; i++)
        {
            DroppedItem di = rhits[i].transform.GetComponent<DroppedItem>();
            FillInventory(di.Stack);
        }

        cnt = inventory.Count;
        for (int i = 0; i < cnt; i++)
        {
            if (inventory[i].Count <= 0)
            {
                inventory.RemoveAt(i);
                i--;
                cnt--;
            }
        }
    }

    private void FillEnvironmantInventoryIcons()
    {
        int cnt = inventory.Count;
        if (cnt > envaironmentCells.Length)
            cnt = envaironmentCells.Length;
        for (int i = 0; i < cnt; i++)
        {
            SetImage(inventoryItemOriginal, envaironmentCells[i], inventory[i], CanDrag);
        }
    }

    public override GameObject SetImage(GameObject origin, Transform cell, AStack stack, bool canDrag)
    {
        var go = base.SetImage(origin, cell, stack, canDrag);
        if (go != null)
        {
            var st = stack as EquipmentStack;
            var drag = go.GetComponent<DragEquipment>();
            var txt = go.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            //drag.EquipStack = st;
            txt.text = drag.EquipStack.Count.ToString();
        }
        return go;
    }
    
    private void ClearEnvironmantInventoryIcons()
    {
        int nc = envaironmentCells.Length;

        for (int col = 0; col < nc; col++)
            if (envaironmentCells[col].childCount > 0)
                Destroy(envaironmentCells[col].GetChild(0).gameObject);
    }

    override public void RefreshUI()
    {
        if (!gameObject.activeInHierarchy)
            return;

        ClearEnvironmantInventoryIcons();
        FindItems();
        FillEnvironmantInventoryIcons();
    }

    private void OnDrawGizmos()
    {
        if (Squad.playerSquadInstance != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(Squad.playerSquadInstance.CenterSquad, radiusToFind);
        }
    }
}
