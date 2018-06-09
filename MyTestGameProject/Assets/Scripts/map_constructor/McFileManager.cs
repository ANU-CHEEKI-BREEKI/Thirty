using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Serialization;
using System.IO;
using System;

public class McFileManager : MonoBehaviour
{
    public static McFileManager Instance { get; private set; }

    [SerializeField] McHider dialogPanel;
    [SerializeField] InputField dialogPanelText;

    [SerializeField] Transform parentFileList;
    [SerializeField] GameObject originalItemOfList;

    [SerializeField] McGridConstructController gridConstructController;

    static bool dialogShowed = false;
    public static bool DialogShowed
    {
        get
        {
            return dialogShowed;
        }
    }

    string[] fnames;

    public static MapBlock mapBlocks;

    static XmlSerializer blockSerializer = new XmlSerializer(typeof(MapBlock));
    public const string PATH_TO_GRIDS = @"Assets/Resources/TextAssets/MapBlockGrids/Current/";
    public const string DIMEN = ".xml";

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        RefreshFileList();

        gridConstructController = transform.parent.transform.GetComponentInChildren<McGridConstructController>();
    }
    

    void RefreshFileList()
    {
        ClearFileList();

        fnames = Directory.GetFiles(PATH_TO_GRIDS);

        foreach (var file in fnames)
            if (file.IndexOf(".meta") == -1)
                CreateItem(file.Substring(PATH_TO_GRIDS.Length, file.Length - PATH_TO_GRIDS.Length - DIMEN.Length));
    }

    private void CreateItem(string file)
    {
        GameObject newItem = Instantiate(originalItemOfList, parentFileList);
        McUiListItem listItem = newItem.GetComponent<McUiListItem>();
        listItem.Initiate();

        listItem.text.text = file;
    }

    private void ClearFileList()
    {
        int childCount = parentFileList.childCount;
        for (int i = 0; i < childCount; i++)
            Destroy(parentFileList.GetChild(i).gameObject);
    }

    public void GeneradeFileName()
    {
        mapBlocks.Entrance = McBlockEntryChooser.BlockEntrance;

        mapBlocks.HasExit = McExitDirectionChooser.hasExit;
        mapBlocks.Exit = McExitDirectionChooser.exit;

        string name = mapBlocks.ToString();
        string[] names = Directory.GetFiles(PATH_TO_GRIDS);
        int q = 0;
        foreach (var item in names)
            if (item.IndexOf(".meta") == -1)
                if (item.IndexOf(name) != -1)
                    q++;
        name = name + " [" + gridConstructController.GroundTypeName.ToString() + "]";
        name = name + " (" + (q+1).ToString() + ")";
        dialogPanelText.text = name;
    }

    public void ShowDialogPanel()
    {
        dialogPanel.Show();
        McBlockEntryChooser.Show(McToggleTypeOfBlock.LastPressedToggle.GetComponent<McToggleTypeOfBlock>().Type);
        dialogShowed = true;
    }

    public void OnOkButtonClick()
    {
        mapBlocks.Entrance = McBlockEntryChooser.BlockEntrance;

        mapBlocks.HasExit = McExitDirectionChooser.hasExit;
        mapBlocks.Exit = McExitDirectionChooser.exit;

        if(mapBlocks.HasExit)
            foreach (var item in mapBlocks.Entrance)
                if (mapBlocks.Exit == item)
                    throw new Exception("Направление выхода не может совпадать с направлением проходов.");

        string fname = PATH_TO_GRIDS + dialogPanelText.text + DIMEN;
        using (FileStream fstream = new FileStream(fname, FileMode.Create))
        {
            blockSerializer.Serialize(fstream, mapBlocks);
        }

        RefreshFileList();

        HideDialog();
    }

    public void OnCancelButtonClick()
    {
        HideDialog();
    }

    private void HideDialog()
    {
        dialogPanel.Hide();
        dialogShowed = false;
    }

    public static MapBlock Deserialize(string path)
    {        
        using (FileStream fstream = File.Open(path, FileMode.Open))
            return (MapBlock)blockSerializer.Deserialize(fstream);
    }

    public static MapBlock Deserialize(TextReader reader)
    {
        return (MapBlock)blockSerializer.Deserialize(reader);
    }
}
