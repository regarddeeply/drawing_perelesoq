using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorMovement : MonoBehaviour
{
    public Texture2D cursorTexture, closeTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    void Start()
    {
        //Cursor.visible = true;
        //Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width / 2, cursorTexture.height / 2), cursorMode);
    }

    public void SetCursor()
    {
        Cursor.visible = true;
        Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width / 2, cursorTexture.height / 2), cursorMode);
    }

    public void ResetCursor()
    {
        Cursor.visible = true;
        //Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width / 2, cursorTexture.height / 2), cursorMode);
    }

    public void SetNoCursor()
    {
        //Cursor.SetCursor(closeTexture, new Vector2(cursorTexture.width / 2, cursorTexture.height / 2), cursorMode);
        Cursor.visible = false;
    }
}
