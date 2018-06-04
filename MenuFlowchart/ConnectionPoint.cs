#if UNITY_EDITOR
using System;
using UnityEngine;

namespace UIFramework {
	public enum ConnectionPointType { In, Out }

	[System.Serializable]
	public class ConnectionPoint{

		public Rect rect = new Rect(0, 0, 20f, 20f);
		public ConnectionPointType type;
		public GUIStyle style, labelStyle;

		public int id;
		public Rect labelRect = new Rect(0,0,100,20);

		public ConnectionPoint(ConnectionPointType type, GUISkin skin){
			this.type = type;
			this.style = skin.button;
			labelStyle = skin.label;
		}

		public void Draw(Node node,int i){
			id = i;
			Rect label = labelRect;
			switch (type){
			case ConnectionPointType.In:
				rect.y = node.rect.y + (node.rect.height * 0.5f) - rect.height * 0.5f;
				rect.x = node.rect.x - rect.width + 8f;
				label.x = rect.x;
				label.y	= rect.y;
				label.x += 20;
				labelStyle.alignment = TextAnchor.MiddleLeft;
				GUI.Label(label,"Enable",labelStyle);
				if (GUI.Button(rect, "", style)){
					NodeEditor.ClickInPoint(this);
				}
				break;

			case ConnectionPointType.Out:
				if(id >= node.menu.transitions.Count) return;
				rect.y = node.rect.y + 25 *(id+1);
				rect.x = node.rect.x + node.rect.width - 8f;
				label.x = rect.x;
				label.y	= rect.y;
				label.x -= label.width;
				labelStyle.alignment = TextAnchor.MiddleRight;
				GUI.Label(label,node.menu.transitions[id].button.name,labelStyle);
				if (GUI.Button(rect, "", style)){
					NodeEditor.ClickOutPoint(this);
				}
				break;
			}
		}
	}
}
#endif