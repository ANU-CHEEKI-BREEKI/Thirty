using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class McExitDirectionChooser : MonoBehaviour
{
    public static MapBlock.Direction exit;
    public static bool hasExit = false;

    [SerializeField] MapBlock.Direction direction;

    private void Start()
    {
        GetComponent<Toggle>().onValueChanged.AddListener(OnValChanged);
    }

    public void OnValChanged(bool newValue)
    {
        if (newValue)
        {
            exit = direction;
            hasExit = newValue;
        }
        else
        {
            hasExit = newValue;
        }
        McFileManager.Instance.GeneradeFileName();
    }

}

