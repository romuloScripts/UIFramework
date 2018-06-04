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

		public  static void OpenWindow(MenuDesign m){
			NodeEditor window = GetWindow<NodeEditor>();
			window.menudesign = m;
			window.titleContent = new GUIContent("Node Editor");
			window.OnEnable();
			if(singleton){
				singleton.OnDrag(-singleton.offset);
				singleton.offset = singleton.drag  = Vector2.zero;
			}
		}

		private void OnEnable(){
			singleton = this;
			SetList();
		}

		private void OnGUI(){
			
			DrawGrid(20, 0.2f, Color.gray);
			DrawGrid(100, 0.4f, Color.gray);
			if(!menudesign) return;
			DrawNodes();
			DrawConnections();
			DrawConnectionLine(Event.current);
			DrawList();

			ProcessNodeEvents(Event.current);
			ProcessEvents(Event.current);
			// if (GUI.changed) 
			Repaint();

		}

		private void SetList(){
			if(!menudesign) return;
			serializedObject = new SerializedObject(menudesign);
			list = new ReorderableList(serializedObject, 
                serializedObject.FindProperty("menuPrefabs"), 
               true, true, true, true); 
			list.drawHeaderCallback = (Rect rect) => {  
    			EditorGUI.LabelField(rect, "Menu Prefabs");
			};
			list.drawElementCallback =  (Rect rect, int index, bool isActive, bool isFocused) => {
    			var element = list.serializedProperty.GetArrayElementAtIndex(index);
    			rect.y += 2;
    			EditorGUI.PropertyField(
        			new Rect(rect.x, rect.y, 160, EditorGUIUtility.singleLineHeight),
        			element, GUIContent.none);
  			};
		}

		private void DrawList(){
			serializedObject.Update();
			
			Rect listRect = position;
			listRect.y = 0;
			float width = 200;
			listRect.x = position.width-width;
			listRect.width = width;

			GUILayout.BeginArea(listRect);
			scrollOffset = EditorGUILayout.BeginScrollView(scrollOffset,false,false);
        	list.DoLayoutList();
			EditorGUILayout.EndScrollView();
			GUILayout.EndArea();
        	serializedObject.ApplyModifiedProperties();
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
					menudesign.nodes[i].Draw();
				}
			}
		}

		private void DrawConnections(){
			if (menudesign.connections != null){
				for (int i = 0; i < menudesign.connections.Count; i++){
					menudesign.connections[i].Draw(menudesign);
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
			foreach (var item in menudesign.menuPrefabs) {
				if(item != null)
					genericMenu.AddItem(new GUIContent(item.name), false, () => OnClickAddNode(mousePosition,item)); 	
			}
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
			menudesign.nodes.Add(new Node(menu, mousePosition, skin));
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
			int menuOut=0, menuIn=0,buttonOut = 0;
			for (int i = 0; i < menudesign.nodes.Count; i++) {
				for (int j = 0; j < menudesign.nodes[i].outPoint.Count; j++) {
					if(menudesign.nodes[i].outPoint[j] == selectedOutPoint){
						menuOut = i;
						buttonOut = j;
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
			menudesign.connections.Add(new Connection(menuOut,buttonOut,menuIn,menudesign.nodes[menuOut].menu,menudesign.nodes[menuIn].menu, skin));
			EditorUtility.SetDirty (menudesign);
		}

		private void ClearConnectionSelection(){
			selectedInPoint = null;
			selectedOutPoint = null;
		}

		public static void RemoveConnection(Connection connection){
			if (singleton && singleton.menudesign) {
				singleton.menudesign.connections.Remove (connection);
				EditorUtility.SetDirty (singleton.menudesign);
			}
		}

		public static void RemoveNode(Node node){
			if (singleton && singleton.menudesign) {
				singleton.menudesign.RemoveNode (node);
				EditorUtility.SetDirty (singleton.menudesign);
			}
		}

		private void RemoveAllNodes(){
			if(menudesign){
				menudesign.nodes.Clear();
				menudesign.connections.Clear();
				EditorUtility.SetDirty(menudesign);
			}
		}

		public static void RemoveDuplicateConnectionOut(Connection connection){
			if(!(singleton && singleton.menudesign)) return;
			for (int i = 0; i < singleton.menudesign.connections.Count; i++) {
				if(singleton.menudesign.nodes[connection.nodeOut].menu == singleton.menudesign.nodes[singleton.menudesign.connections[i].nodeOut].menu && connection.buttonOut == singleton.menudesign.connections[i].buttonOut){
					RemoveConnection(singleton.menudesign.connections[i]);
					return;
				}
			}
			EditorUtility.SetDirty (singleton.menudesign);
		}
	}
}
#endif