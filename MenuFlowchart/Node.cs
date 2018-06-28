using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace UIFramework {
	[System.Serializable]
	public class Node:ScriptableObject{

		public string menuName="Menu";
		public ConnectionPoint inPoint;
		public List<ConnectionPoint> outPoint = new List<ConnectionPoint>();

		[HideInInspector,SerializeField]
		public Rect rect;

#if UNITY_EDITOR
		[HideInInspector]
		public bool isDragged;
		[HideInInspector]
		public bool isSelected;

		private GUIStyle style, defaultNodeStyle, selectedNodeStyle;
		private const float width = 150;

		public void Ini(Vector2 position, GUISkin skin){
			rect = new Rect(position.x, position.y, width, 0);

			inPoint = ScriptableObject.CreateInstance<ConnectionPoint>();
			inPoint.name = "Open";
			inPoint.textName = "Back";
			AssetDatabase.AddObjectToAsset(inPoint,NodeEditor.singleton.menudesign);
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(inPoint));
			inPoint.Ini(ConnectionPointType.In);
			EditorUtility.SetDirty (NodeEditor.singleton.menudesign);

			style = skin.box;
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

		public void Draw(GUISkin skin){
            UpdateSkinStyles(skin);
            DrawRect(skin);
            inPoint.Draw(this, 0, skin);
            DrawConnectionPoints(skin);
            AddButton(skin);
        }

        private void UpdateSkinStyles(GUISkin skin){
            selectedNodeStyle = skin.customStyles[0];
            defaultNodeStyle = skin.box;
        }

        private void DrawRect(GUISkin skin){
            float height = 70 + (outPoint.Count - 1) * 25;
            rect.height = height;

            Rect rectshadow = rect;
            rectshadow.x += 4;
            rectshadow.y += 4;
            GUI.Box(rectshadow, "", skin.customStyles[1]);

			if(style == null){
				style = skin.box;
			}
            GUI.Box(rect, "", style);
            skin.textField.alignment = TextAnchor.MiddleCenter;
            menuName = GUI.TextField(new Rect(rect.x + rect.width / 2f - width / 2f, rect.y, width, 20f), menuName, skin.textField);
        }

        private void DrawConnectionPoints(GUISkin skin){
            for (int i = 0; i < outPoint.Count; i++){
                outPoint[i].Draw(this, i, skin);
            }
        }

		private void AddButton(GUISkin skin){
			float btnSize = 30f;
			if (GUI.Button (new Rect (rect.x+rect.width-20,rect.y+rect.height-20,btnSize,btnSize), "+", skin.customStyles[2])) {
				ConnectionPoint outP = ScriptableObject.CreateInstance<ConnectionPoint>();
				outP.name = "Button";
				AssetDatabase.AddObjectToAsset(outP,NodeEditor.singleton.menudesign);
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(outP));
				outP.Ini(ConnectionPointType.Out);
				outPoint.Add(outP);
				EditorUtility.SetDirty (NodeEditor.singleton.menudesign);
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

		public void Remove(List<Node> nodes,List<Connection> connections){
			if (connections != null){
				for (int j = connections.Count-1; j >=0; j--){
					Connection c = connections[j];
					if (c.menuOrigin == this || c.menuTarget == this){
						c.Remove(connections);
					}
				}
			}
			inPoint.Remove(null);
			for (int j = outPoint.Count-1; j >=0; j--){
				 outPoint[j].Remove(outPoint);
			}
			nodes.Remove(this);
			DestroyImmediate(this,true);
			AssetDatabase.SaveAssets();
		}

#endif
	}
}