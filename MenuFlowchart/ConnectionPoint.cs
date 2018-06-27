#if UNITY_EDITOR
using System;
using UnityEngine;

namespace UIFramework {
	public enum ConnectionPointType { In, Out }

	[System.Serializable]
	public class ConnectionPoint{

		public Rect rect = new Rect(0, 0, 20f, 20f);
		public ConnectionPointType type;
		public GUISkin skin;
		public GUIStyle labelStyle, textFieldstyle,buttonMinus;
		public string textName="name";

		public int id;
		public Rect labelRect = new Rect(0,0,60,20);

		public ConnectionPoint(ConnectionPointType type){
			this.type = type;
		}

		public void Draw(Node node,int i,GUISkin skin){
			id = i;
			Rect label = labelRect;
			switch (type){
			case ConnectionPointType.In:
				if(node.menu.transitions.Count>1)
					rect.y = node.rect.y + (node.rect.height * 0.5f) - rect.height * 0.5f;
				else
					rect.y = node.rect.y + 25;
				rect.x = node.rect.x - rect.width + 8f;
				label.x = rect.x;
				label.y	= rect.y;
				label.x += 20;
				skin.label.alignment = TextAnchor.MiddleLeft;
				GUI.Label(label,"Open",skin.label);
				if (GUI.Button(rect, "", skin.button)){
					NodeEditor.ClickInPoint(this);
				}
				break;

			case ConnectionPointType.Out:
				if(id >= node.menu.transitions.Count) return;
				rect.y = node.rect.y +5+ (25 *(id+1));
				rect.x = node.rect.x + node.rect.width - 8f;
				label.x = rect.x;
				label.y	= rect.y;
				label.x -= label.width;
				skin.textField.alignment = TextAnchor.MiddleRight;
				//GUI.Label(label,node.menu.transitions[id].button.name,labelStyle);

				textName = GUI.TextField(label,textName, skin.textField);
				if (GUI.Button(rect, "", skin.button)){
					NodeEditor.ClickOutPoint(this);
				}
				float btnSize = 27f;
				if (GUI.Button (new Rect (label.x-16,label.y+5,btnSize,btnSize), "-", skin.customStyles[2])) {
					
				}
				break;
			}
		}
	}
}
#endif