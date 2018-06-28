using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UIFramework {
	[CreateAssetMenu(fileName ="UI Flowchart", menuName = "UI Framework/UI Flowchart", order = 0)]
	public class MenuDesign : ScriptableObject {

		public Menu menuTemplate;
		public Button buttonTemplate;

		[HideInInspector,SerializeField]
		public List<Node> nodes;
		[HideInInspector,SerializeField]
		public List<Connection> connections;

#if UNITY_EDITOR
		public class NodeToMenu{
			public Menu menu;
			public List<Button> buttons=new List<Button>();

			public NodeToMenu(Menu m){
				menu = m;
			}
		}

		public void CreateMenus(bool createPrefabs){
            if (!menuTemplate || !buttonTemplate) return;
            List<NodeToMenu> menus = InstantiateMenus();
            CreateBackConnections(menus);
            if (createPrefabs){
                TransformInPrefabs(menus);
            }else{
                CreateMenuManager(menus);
            }
            CreateConnections(menus);
            foreach (var nm in menus){
                EditorUtility.SetDirty(nm.menu);
            }
        }

        private void CreateMenuManager(List<NodeToMenu> menus){
            GameObject go = new GameObject(name);
            MenuManager m = go.AddComponent<MenuManager>();
            foreach (var item in menus){
                item.menu.destroyWhenClosed = false;
                m.menus.Add(item.menu);
                item.menu.transform.SetParent(m.transform, false);
            }
        }

        private void TransformInPrefabs(List<NodeToMenu> menus){
            foreach (var nm in menus){
                Menu old = nm.menu;
                nm.menu = PrefabUtility.CreatePrefab(Path.GetDirectoryName(AssetDatabase.GetAssetPath(this)) + "/" + nm.menu.name + ".prefab",
                nm.menu.gameObject, ReplacePrefabOptions.ConnectToPrefab).GetComponent<Menu>();
                Button[] buttons = nm.menu.GetComponentsInChildren<Button>();
                nm.menu.destroyWhenClosed = true;
                for (int i = 0; i < buttons.Length && i < nm.buttons.Count; i++){
                    nm.buttons[i] = buttons[i];
                }
                DestroyImmediate(old.gameObject);
            }
        }

        private List<NodeToMenu> InstantiateMenus(){
            List<NodeToMenu> menus = new List<NodeToMenu>();
            foreach (var item in nodes){
                Menu m = Instantiate<Menu>(menuTemplate);
                m.name = item.menuName;
                NodeToMenu nm = new NodeToMenu(m);
                CreateButtons(nm, item);
                menus.Add(nm);
            }

            return menus;
        }

        private void CreateButtons(NodeToMenu nodeToMenu, Node node){
			foreach (var item in node.outPoint){
                Button b = InstantiateButton(nodeToMenu);
				b.name = item.textName;
				Text t = b.GetComponentInChildren<Text>();
				if(t) t.text = b.name;
                nodeToMenu.buttons.Add(b);
            }
        }

		private void CreateBackConnections(List<NodeToMenu> menus){
			foreach (var item in connections){
				int target = nodes.FindIndex(o =>o == item.menuTarget);
				if(!menus[target].menu.closeTransition){
                   Button b = InstantiateButton(menus[target]);
				   menus[target].menu.closeTransition = b;
				   b.name = item.menuTarget.inPoint.textName;
				   Text t = b.GetComponentInChildren<Text>();
				   if(t) t.text = b.name;
                }
            }
		}

        private void CreateConnections(List<NodeToMenu> menus){
			foreach (var item in connections){
				int target = nodes.FindIndex(o =>o == item.menuTarget);
				int origin = nodes.FindIndex(o =>o == item.menuOrigin);
				int buttonOut = nodes[origin].outPoint.FindIndex(o =>o == item.buttonOut);
				menus[origin].menu.AddTransition(menus[origin].buttons[buttonOut],menus[target].menu);
            }
		}

		private Button InstantiateButton(NodeToMenu nodeToMenu){
            Button b = Instantiate<Button>(buttonTemplate);
            VerticalLayoutGroup vGroup = nodeToMenu.menu.GetComponentInChildren<VerticalLayoutGroup>();
            b.transform.SetParent(vGroup ? vGroup.transform : nodeToMenu.menu.transform, false);
            return b;
        }
#endif
	}
}