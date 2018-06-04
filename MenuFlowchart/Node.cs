#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
#endif
namespace UIFramework {
	[System.Serializable]
	public class Node{

		#if UNITY_EDITOR
		public Menu menu;

		public Rect rect,rect2;
		public bool isDragged;
		public bool isSelected;
		public ConnectionPoint inPoint;
		public List<ConnectionPoint> outPoint = new List<ConnectionPoint>();
		public GUIStyle style, defaultNodeStyle, selectedNodeStyle,styleShadow;
		public GUISkin skin;

		private float width = 200;
		private float height;

		public Node(Menu menu, Vector2 position, GUISkin skin){
			this.menu = menu;

			height = 50+(menu.transitions.Count-1)*25;
			rect = new Rect(position.x, position.y, width, height);
			inPoint = new ConnectionPoint(ConnectionPointType.In,  skin);
			for (int i = 0; i < menu.transitions.Count; i++) {
				ConnectionPoint outP = new ConnectionPoint( ConnectionPointType.Out, skin);
				outPoint.Add(outP);
			}
			style = defaultNodeStyle = skin.box;
			selectedNodeStyle = skin.customStyles[0];
			this.skin = skin;
			styleShadow = skin.customStyles[1];

		}

		public bool ConnectionOutExists(ConnectionPoint con){
			foreach (var item in outPoint) {
				if(con == item){
					return true;
				}
			}
			return false;
		}

		public void Drag(Vector2 delta){
			rect.position += delta;
		}

		public void Draw(){
			if(!menu){
				OnClickRemoveNode();
				return;
			}
			height = 50+(menu.transitions.Count-1)*25;
			rect.height = height;
			//Color shadowCol = new Color(0, 0, 0, 0.06f);
			//for (int i = 0; i < 3; i++){ // Draw a shadow
			Rect rectshadow = rect;
			rectshadow.x+=4;
			rectshadow.y+=4;
			GUI.Box(rectshadow, "", styleShadow);
			//}
			GUI.Box(rect, menu.name, style);
			inPoint.Draw(this,0);
			if(menu.transitions.Count > outPoint.Count){
				for (int i = outPoint.Count-1; i < menu.transitions.Count; i++) {
					ConnectionPoint outP = new ConnectionPoint(ConnectionPointType.Out, skin);
					outPoint.Add(outP);
				}
			}else if(menu.transitions.Count < outPoint.Count){
				for (int i = outPoint.Count-1; i > menu.transitions.Count-1; i--) {
					//				ConnectionPoint outP = new ConnectionPoint( ConnectionPointType.Out, skin);
					outPoint.RemoveAt(i);
				}
			}
			for (int i = 0; i < outPoint.Count; i++) {
				outPoint[i].Draw(this,i);
			}
		}

		public bool ProcessEvents(Event e){
			switch (e.type){
			case EventType.MouseDown:
				if (e.button == 0){
					if (rect.Contains(e.mousePosition)){
						isDragged = true;
						GUI.changed = true;
						isSelected = true;
						style = selectedNodeStyle;
					}else{
						GUI.changed = true;
						isSelected = false;
						style = defaultNodeStyle;
					}
				}

				if (e.button == 1 && isSelected && rect.Contains(e.mousePosition)){
					ProcessContextMenu();
					e.Use();
				}
				break;

			case EventType.MouseUp:
				isDragged = false;
				break;

			case EventType.MouseDrag:
				if (e.button == 0 && isDragged){
					Drag(e.delta);
					e.Use();
					return true;
				}
				break;
			case EventType.KeyDown:
				if (e.keyCode == KeyCode.Delete && isSelected){
					OnClickRemoveNode();
					e.Use();
				}
				break;
			}
			return false;
		}

		private void ProcessContextMenu(){
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
			genericMenu.ShowAsContext();
		}

		private void OnClickRemoveNode(){
			NodeEditor.RemoveNode(this);
		}
		#endif
	}
}