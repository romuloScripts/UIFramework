using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UIFramework {
	public class MenuManager : MonoBehaviour {

		public bool onAwake= true;
		public Menu firstMenuPrefab;
		public List<Menu> menus= new List<Menu>();

		private Menu menuEditor;

		private int getLast{
			get{return menus.Count-1;}
		}

		public void DisableAll(){
			for (int i = 0; i < menus.Count; i++){
				menus[i]?.gameObject.SetActive(false);
			}
		}

		void Awake()
        {
			if (onAwake)
            	Initiate();
        }

        public void Initiate()
        {
            EventSystem.current.SetSelectedGameObject(null);
            Openfirst();
        }

        void Openfirst(){
			DisableAll();
			if(menus.Count<=0 && firstMenuPrefab){
				menus.Add(Instantiate<Menu>(firstMenuPrefab,transform));
			}
			menus[0].Open(this,null);	
		}

		public void AddMenu(Menu menu){
			if(!menus.Contains(menu))
				menus.Add(menu);
		}

		public void RemoveMenu(Menu menu){
			menus.Remove(menu);
		}

		public void Active(){
			gameObject.SetActive(true);
			Awake();
		}

		public void Deactive(){
			gameObject.SetActive(false);
		}

		public void ActiveMenuSolo(Menu menu){
			foreach (var item in menus){
				if(item.gameObject.activeSelf){
					item.Disable(true);
				}
			}
			menu.Enable();
		}

		#if UNITY_EDITOR
		public void OnDrawGizmos(){
			if(Application.isPlaying || menus.Count <=0) return;
			Menu m = null;
			if(Selection.activeGameObject)
				m = Selection.activeGameObject.GetComponent<Menu>();
			if(m) menuEditor = m;
			if(menuEditor == null && menus[0] != null)
				menuEditor = menus[0];
			foreach (Menu item in menus) {
				if(item == null) continue;
				if(item.Equals(menuEditor)){
					if(!item.gameObject.activeSelf)
						item.gameObject.SetActive(true);
				}else if(item.gameObject.activeSelf){
					item.gameObject.SetActive(false);
					EditorUtility.SetDirty(item.gameObject);
				}
			}
		}
		#endif
	}	
}