using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour {

    public bool showCursor;
    public Texture2D texture;
    public Vector2 hotspot = new Vector2(16, 16);

	public void ApplySettings(){
		Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
        Cursor.visible = showCursor;
	}
}
