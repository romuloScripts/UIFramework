using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UIFramework {
	[System.Serializable]
	public class Connection:ScriptableObject {
		
		public Node menuOrigin;
		public Node menuTarget;
		public ConnectionPoint buttonOut;

#if UNITY_EDITOR
		public void Ini(ConnectionPoint buttonOut, Node menuOrigin, Node menuTarget) {
			this.menuOrigin = menuOrigin;
			this.menuTarget = menuTarget;
			this.buttonOut = buttonOut;
			
			NodeEditor.RemoveDuplicateConnectionOut (this);
		}

		public void Draw (MenuDesign menu, GUISkin skin) {
		
			if (buttonOut == null || menuOrigin == null || menuTarget == null) {
				NodeEditor.RemoveConnection (this);
				return;
			}
			Rect rectIn = menuTarget.inPoint.rect;
			Rect rectOut = buttonOut.rect;

			DrawShadow (rectIn, rectOut);
			Handles.DrawBezier (
				rectIn.center,
				rectOut.center,
				rectIn.center + Vector2.left * 50f,
				rectOut.center - Vector2.left * 50f,
				NodeEditor.lineColor,
				null,
				NodeEditor.lineWidht
			);
		
			Vector3 btnPos = (rectIn.center + rectOut.center) * 0.5f;
			float btnSize = 24f;
			if (GUI.Button (new Rect (btnPos.x - (btnSize * 0.5f), btnPos.y - (btnSize * 0.5f), btnSize, btnSize), "╳", skin.button)) {
				NodeEditor.RemoveConnection (this);
			}
		}

		private void DrawShadow(Rect shadowrect, Rect shadowrectOut){

			Color shadowCol = new Color(0, 0, 0, 0.3f);
			shadowrect.x+=3;
			shadowrect.y+=3;
			shadowrectOut.x+=3;
			shadowrectOut.y+=3;

			Handles.DrawBezier(
				shadowrect.center,
				shadowrectOut.center,
				shadowrect.center + Vector2.left * 50f,
				shadowrectOut.center - Vector2.left * 50f,			
				shadowCol, null, NodeEditor.lineWidht*1.5f);
		}

		public void Remove(List<Connection> connections){
			connections.Remove(this);
			DestroyImmediate(this,true);
			AssetDatabase.SaveAssets();
		}
#endif
	}
}