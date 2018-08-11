using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUpTextController : MonoBehaviour
{
    [SerializeField] GameObject[] textOrigin;

    [Header("Default properties")]
    [SerializeField] Vector2 floatSpeed = new Vector2(0, 2);
    [SerializeField] [Range(0, 5)]float textLifeTime = 1f;
    [SerializeField] Color startColor = Color.green;
    [SerializeField] Color endColor = Color.red;


    static public PopUpTextController Instance { get; private set; }
    Transform thisTransform;
    List<Item> textes = new List<Item>(30);
    Stack<TextMeshPro> pool;

    Camera mainCamera;

    public bool ScaledDeltaTime { get; set; } = true;

    private void Awake()
    {
        thisTransform = transform;
        Instance = this;
    }

    void Start ()
    {
        mainCamera = Camera.main;

        pool = new Stack<TextMeshPro>();

        {
            Stack<TextMeshPro> t = new Stack<TextMeshPro>();

            for (int i = 0; i < 30; i++)
                t.Push(GetText());

            for (int i = 0; i < 30; i++)
                GiveBackText(t.Pop());
        }        
    }

    private void Update()
    {
        float deltatime = 0;
        if (ScaledDeltaTime)
            deltatime = Time.deltaTime;
        else
            deltatime = Time.unscaledDeltaTime;

        for (int i = 0; i < textes.Count; i++)
        {
            if (textes[i].properties.RemainingLifetime <= 0)
            {
                GiveBackText(textes[i].textUI);
                textes.Remove(textes[i]);
                i--;
                continue;
            }

            var prop = textes[i].properties;
            prop.RemainingLifetime -= deltatime;
            prop.Position += prop.Speed * deltatime;
            textes[i].properties = prop;
            textes[i].textUI.color = prop.Color;
            textes[i].textUI.transform.position = prop.Position;
            if (prop.FontSize == null)
                textes[i].textUI.fontSize = mainCamera.orthographicSize / 1.7f;
            else
                textes[i].textUI.fontSize = prop.FontSize.Value;
        }
    }
    
    public void AddTextLabel(string text, Vector2 worldPosition, float? lifetime = null, Vector2? screenFloatSpeed = null, 
        Color? startColor = null, Color? endColor = null, int sortingOrder = 0, float? fontSize = null)
    {
        if (lifetime == null)
            lifetime = textLifeTime;
        if (screenFloatSpeed == null)
            screenFloatSpeed = this.floatSpeed;
        if (startColor == null)
            startColor = this.startColor;
        if (endColor == null)
            endColor = this.endColor;

        PopUpTextProperties textProp = new PopUpTextProperties(
            text: text, 
            screenPosition: worldPosition, 
            lifetime: lifetime.Value, 
            floatSpeed: screenFloatSpeed.Value, 
            startColor:startColor.Value, 
            endColor: endColor.Value,
            fontSize: fontSize
        );

        TextMeshPro txt = GetText();
        txt.transform.position = textProp.Position;
        txt.sortingOrder = sortingOrder;
        txt.text = textProp.Text;
        txt.color = textProp.Color;

        //аккуратно тут
        if(fontSize == null)
            txt.fontSize = mainCamera.orthographicSize / 1.7f;
        else
            txt.fontSize = fontSize.Value;

        textes.Add(new Item() { properties = textProp, textUI = txt });
    }
    
    public TextMeshPro GetText(int index = 0)
    {
        TextMeshPro res;
        if (pool.Count > 0)
            res = pool.Pop();
        else
        {
            var obj = Instantiate(textOrigin[index], thisTransform);
            var renderer = obj.GetComponent<Renderer>();
            res = obj.GetComponent<TextMeshPro>();
        }
        res.gameObject.SetActive(true);

        return res;
    }

    public void GiveBackText(TextMeshPro text)
    {
        text.gameObject.SetActive(false);
        pool.Push(text);
    }
    
    struct PopUpTextProperties
    {
        public string Text;
        public Vector2 Position;
        public Color Color
        {
            get
            {
                return Color.Lerp(endColor, startColor, RemainingLifetime / lifetime);
            }
        }
        public float RemainingLifetime;
        public Vector2 Speed;

        Color startColor;
        Color endColor;
        float lifetime;
        public float? FontSize;

        public PopUpTextProperties(string text, Vector2 screenPosition, float lifetime, Vector2 floatSpeed, Color startColor, Color endColor, float? fontSize)
        {
            Text = text;
            Position = screenPosition;
            RemainingLifetime = lifetime;
            Speed = floatSpeed;
            this.startColor = startColor;
            this.endColor = endColor;
            this.lifetime = lifetime;
            FontSize = fontSize;
        }

    }

    class Item
    {
        public PopUpTextProperties properties { get; set; }
        public TextMeshPro textUI { get; set; }
    }
}
