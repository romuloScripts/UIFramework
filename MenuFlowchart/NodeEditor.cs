#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
 using UnityEditorInternal;

namespace UIFramework {
	public class NodeEditor : EditorWindow{

		public MenuDesign menudesign;
		public SerializedObject serializedObject;

		public GUISkin skin;
		private ConnectionPoint selectedInPoint;
		private ConnectionPoint selectedOutPoint;

		private Vector2 offset;
		private Vector2 drag;
		private float zoomScale=1;   

		public static Color lineColor = new Color(0.5f,0,0.5f);
		public static float lineWidht = 4;
		public static NodeEditor singleton; 

		private ReorderableList list;
		private Vector2 scrollOffset;

		public static void OpenWindow(MenuDesign m){
			NodeEditor window = GetWindow<NodeEditor>();
			window.menudesign = m;
			window.titleContent = new GUIContent(m.name);
			window.OnEnable();
			if(singleton){
				singleton.OnDrag(-singleton.offset);
				singleton.offset = singleton.drag  = Vector2.zero;
			}
		}

		private void OnEnable(){
			singleton = this;
			serializedObject = new SerializedObject(menudesign);
		}

		private void OnGUI(){
			
			DrawGrid(20, 0.2f, Color.gray);
			DrawGrid(100, 0.4f, Color.gray);
			if(!menudesign) return;
			DrawNodes();
			DrawConnections();
			DrawConnectionLine(Event.current);
			DrawMenus();
			ProcessNodeEvents(Event.current);
			ProcessEvents(Event.current);
			// if (GUI.changed) 
			Repaint();

		}

		private void DrawMenus(){


			SerializedProperty menu = serializedObject.FindProperty("menuTemplate");
			SerializedProperty button = serializedObject.FindProperty("buttonTemplate");
			
			float width = 150;
			float height = 150;
			Rect rect = new Rect(position.width-width,0,width,height);
			GUI.BeginGroup(rect,"");
				rect = new Rect(0,0,width,height);
				GUI.Box(rect,"",skin.customStyles[1]);
				rect = new Rect(5,0,width-5,15);
				EditorGUI.LabelField(rect,"Menu Template");
				rect.y += 20;
				EditorGUI.PropertyField(rect,menu, GUIContent.none);
				rect.y += 20;
				EditorGUI.LabelField(rect,"Button Template");
				rect.y += 20;
				EditorGUI.PropertyField(rect, button,GUIContent.none);
				serializedObject.ApplyModifiedProperties();
				CreateButton(rect.y+30);
			GUI.EndGroup();
		}

		private void CreateButton(float y){
			Rect rect = new Rect(5,y,140,20);
			if(GUI.Button(rect,"Create Menus")){
				menudesign?.CreatePrefabs(false);
			}
			rect.y+=30;
			if(GUI.Button(rect,"Create Menus Prefabs")){
				menudesign?.CreatePrefabs(true);
			}
		}

		private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor){
			int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
			int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

			Handles.BeginGUI();

			Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
			offset += drag * 0.5f;
			Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

			for (int i = 0; i < widthDivs; i++){
				Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
			}

			for (int j = 0; j < heightDivs; j++){
				Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
			}

			Handles.color = Color.white;
			Handles.EndGUI();
		}

		private void DrawNodes(){
			if (menudesign.nodes != null){
				for (int i = 0; i < menudesign.nodes.Count; i++){
					menudesign.nodes[i].Draw(skin);
				}
			}
		}

		private void DrawConnections(){
			if (menudesign.connections != null){
				for (int i = 0; i < menudesign.connections.Count; i++){
					menudesign.connections[i].Draw(menudesign,skin);
				} 
			}
		}

		private void ProcessEvents(Event e){
			drag = Vector2.zero;

			switch (e.type){
			case EventType.MouseDown:
				if (e.button == 0){
					ClearConnectionSelection();
				}
				if (e.button == 1){
					ProcessContextMenu(e.mousePosition);
				}
				break;

			case EventType.MouseDrag:
				if (e.button == 0){
					OnDrag(e.delta);
				}
				break;

			case EventType.ScrollWheel:
				OnZoom(e.delta);
				e.Use();
				break;
			}
		}

		private void ProcessNodeEvents(Event e){
			if (menudesign.nodes != null){
				for (int i = menudesign.nodes.Count - 1; i >= 0; i--){
					bool guiChanged = menudesign.nodes[i].ProcessEvents(e);
					if (guiChanged){
						GUI.changed = true;
					}
				}
			}
		}

		private void DrawConnectionLine(Event e){
			if (selectedInPoint != null && selectedOutPoint == null){
				Handles.DrawBezier(
					selectedInPoint.rect.center,
					e.mousePosition,
					selectedInPoint.rect.center + Vector2.left * 50f,
					e.mousePosition - Vector2.left * 50f,
					lineColor,
					null,
					lineWidht
				);
				GUI.changed = true;
			}

			if (selectedOutPoint != null && selectedInPoint == null){
				Handles.DrawBezier(
					selectedOutPoint.rect.center,
					e.mousePosition,
					selectedOutPoint.rect.center - Vector2.left * 50f,
					e.mousePosition + Vector2.left * 50f,
					lineColor,
					null,
					lineWidht
				);

				GUI.changed = true;
			}
		}

		private void ProcessContextMenu(Vector2 mousePosition){
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent("Add Menu"), false, () => OnClickAddNode(mousePosition,null));
			genericMenu.AddItem(new GUIContent("Remove All Nodes"), false, () => RemoveAllNodes()); 	
			genericMenu.ShowAsContext();
		}

		private void OnDrag(Vector2 delta){
			drag = delta;

			if (menudesign.nodes != null){
				for (int i = 0; i < menudesign.nodes.Count; i++){
					menudesign.nodes[i].Drag(delta);
				}
			}
			GUI.changed = true;
		}

		private void OnZoom(Vector2 z){
			zoomScale -= z.y*0.1f; 
			zoomScale = Mathf.Clamp(zoomScale,0.2f,1f);
			GUI.changed = true;
		}

		private void OnClickAddNode(Vector2 mousePosition,Menu menu){
			if (menudesign.nodes == null){
				menudesign.nodes = new List<Node>();
			}
			Node n = ScriptableObject.CreateInstance<Node>();
			n.name = "Menu";
			AssetDatabase.AddObjectToAsset(n,menudesign);
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(n));
			n.Ini(mousePosition,skin);
			menudesign.nodes.Add(n);
			EditorUtility.SetDirty (menudesign);
		}

		private void OnClickInPoint(ConnectionPoint inPoint){
			selectedInPoint = inPoint;
			if (selectedOutPoint != null){
				if (GetNode(selectedOutPoint) != GetNode(selectedInPoint)){
					CreateConnection();
					ClearConnectionSelection(); 
				}else{
					ClearConnectionSelection();
				}
			}
		}

		private Node GetNode(ConnectionPoint point){
			if(!menudesign) return null;
			foreach (var item in menudesign.nodes) {
				if(item.inPoint == point) return item;
				foreach (var item2 in item.outPoint) {
					if(item2 == point){
						return item;
					}
				}
			}
			return null;
		}

		public static void ClickInPoint(ConnectionPoint inPoint){
			if(singleton)
				singleton.OnClickInPoint(inPoint);
		}

		public static void ClickOutPoint(ConnectionPoint inPoint){
			if(singleton)
				singleton.OnClickOutPoint(inPoint);
		}

		private void OnClickOutPoint(ConnectionPoint outPoint){
			selectedOutPoint = outPoint;
			if (selectedInPoint != null){
				if (GetNode(selectedOutPoint) != GetNode(selectedInPoint)){
					CreateConnection();
					ClearConnectionSelection();
				}
				else{
					ClearConnectionSelection();
				}
			}
		}

		private void CreateConnection(){
			if (menudesign.connections == null){
				menudesign.connections = new List<Connection>();
			}
			int menuOut=0, menuIn=0;
			for (int i = 0; i < menudesign.nodes.Count; i++) {
				for (int j = 0; j < menudesign.nodes[i].outPoint.Count; j++) {
					if(menudesign.nodes[i].outPoint[j] == selectedOutPoint){
						menuOut = i;
						break;
					}
				}	
			}
			for (int i = 0; i < menudesign.nodes.Count; i++) {
				if(menudesign.nodes[i].inPoint == selectedInPoint){
					menuIn = i;
					break;
				}
			}
			Connection c = ScriptableObject.CreateInstance<Connection>();
			c.name = "Connection";
			AssetDatabase.AddObjectToAsset(c,menudesign);
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(c));
			c.Ini(selectedOutPoint,menudesign.nodes[menuOut],menudesign.nodes[menuIn]);
			menudesign.connections.Add(c);
			EditorUtility.SetDirty (menudesign);
		}

		private void ClearConnectionSelection(){
			selectedInPoint = null;
			selectedOutPoint = null;
		}

		public static void RemoveConnection(Connection connection){
			if (singleton && singleton.menudesign) {
				connection.Remove(singleton.menudesign.connections);
				EditorUtility.SetDirty (singleton.menudesign);
			}
		}

		public static void RemoveNode(Node node){
			if (singleton && singleton.menudesign){
				node.Remove(singleton.menudesign.nodes,singleton.menudesign.connections);
				EditorUtility.SetDirty (singleton.menudesign);
			}
		}

		private void RemoveAllNodes(){
			if(menudesign){
				for (int j = menudesign.nodes.Count-1; j >=0; j--){
					menudesign.nodes[j].Remove(menudesign.nodes,menudesign.connections);
				}
				Object[] objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(menudesign));
				for (int i = objs.Length-1; i >=0; i--){
					DestroyImmediate(objs[i],true);
				}
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(menudesign));
				EditorUtility.SetDirty(menudesign);
			}
		}

		public static void RemoveDuplicateConnectionOut(Connection connection){
			if(!(singleton && singleton.menudesign)) return;
			for (int i = 0; i < singleton.menudesign.connections.Count; i++) {
				if(connection.menuTarget== singleton.menudesign.connections[i].menuTarget 
				&& connection.buttonOut == singleton.menudesign.connections[i].buttonOut){
					RemoveConnection(singleton.menudesign.connections[i]);
					return;
				}
			}
			EditorUtility.SetDirty (singleton.menudesign);
		}
	}
}
#endif