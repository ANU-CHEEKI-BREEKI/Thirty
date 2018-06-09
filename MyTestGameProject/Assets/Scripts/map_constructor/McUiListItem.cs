using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class McUiListItem : MonoBehaviour
{
    public Text text;
    public Button button;

    public void Initiate()
    {
        text = GetComponentInChildren<Text>();
        button = GetComponentInChildren<Button>();

        button.onClick.AddListener(() => { LoadMapBlock(text.text); });
    }

    void LoadMapBlock(string name)
    {
        McGrid.hasInputedBlock = true;
        McGrid.InputedMapBlock = McFileManager.Deserialize(McFileManager.PATH_TO_GRIDS + name + McFileManager.DIMEN);
    }
}
