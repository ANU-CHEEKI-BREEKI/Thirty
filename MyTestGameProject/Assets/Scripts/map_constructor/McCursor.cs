using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class McCursor : MonoBehaviour
{
    [SerializeField] Texture2D handCursorTexture;
    [SerializeField] Texture2D brushCursorTexture;

    // Use this for initialization
    void Start ()
    {
        SetBrushCursor();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    
    public void SetHandCursor()
    {
        Cursor.SetCursor(
            handCursorTexture,
            new Vector2(handCursorTexture.width / 2, handCursorTexture.height / 2),
            CursorMode.Auto
        );
    }

    public void SetBrushCursor()
    {
        SetDefaultCursor();

        //Cursor.SetCursor(
        //    brushCursorTexture, 
        //    new Vector2(brushCursorTexture.width / 2, brushCursorTexture.height / 2), 
        //    CursorMode.Auto
        //);
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

}
