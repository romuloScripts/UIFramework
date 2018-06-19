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

		public MenuDesign menuTemplate;
		[Tooltip("Destroy current menus gameobjects and add prefabs again")]
		public bool createOnAwake = false;

		public List<Menu> menus;


		private Menu menuEditor;

		void Awake(){
			if(menus.Count<=0 || createOnAwake)
				CreateMenu();
			Init();
		}

		void Start() {
			EventSystem.current.SetSelectedGameObject(null);
			menus[0].Enable();
			//if(menus[0].firstSelected)
			//	EventSystem.current.SetSelectedGameObject(menus[0].firstSelected);
		}

		public void Active(){
			gameObject.SetActive(true);
			Awake();
			Start();
			//StartCoroutine(Start());
		}

		public void Deactive(){
			//StartCoroutine(Start2());
			gameObject.SetActive(false);
		}

		public void ActiveMenuSolo(Menu menu){
			foreach (var item in menus){
				if(item.gameObject.activeSelf)
					item.Disable();
			}
			menu.Enable();
		}

/* 		IEnumerator Start2() {
			yield return new WaitForSecondsRealtime(0.01f);
			gameObject.SetActive(false);
		}	 */

		[ContextMenu("Create Menu")]
		public void CreateMenu(){
			foreach (var item in menus) {
				if(item != null) {
					#if UNITY_EDITOR
					if(Application.isPlaying){
						Destroy(item.gameObject);
					}else{
						DestroyImmediate(item.gameObject);
					}
					#else
					Destroy(item.gameObject);
					#endif
				}
			}
			menus.Clear();
			foreach (var item in menuTemplate.menuPrefabs) {
				#if UNITY_EDITOR
				if(Application.isPlaying){
					Menu m = Instantiate<Menu>(item,transform);
					m.name = item.name;
					menus.Add(m);
				}else{
					Menu m = PrefabUtility.InstantiatePrefab(item) as Menu;
					m.transform.SetParent(transform,false);
					menus.Add(m);
				}
				#else
				Menu m = Instantiate<Menu>(item,transform);
				m.name = item.name;
				menus.Add(m);
				#endif
			}
		}

		/* void LinkTransitions(){
			if(menuTemplate){
				foreach (var item in menus)
				{
					item.LinkTransitions(menuTemplate,menus);	
				}	
			}
		} */

		public void Init(){
			//LinkTransitions();
			//Menu firstMenu = menus[0];
			//firstMenu.Enable();
			for (int i = 0; i < menus.Count; i++) {
				//if(menus[i].Equals(firstMenu)) continue;
				menus[i].gameObject.SetActive(false);
			}
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

		public void ApplyPrefabs(){
			for (int i = 0; i < menus.Count; i++) {
				if(!menus[i]) continue;
				PrefabUtility.ReplacePrefab(menus[i].gameObject,menuTemplate.menuPrefabs[i]);
			} 
		}
		#endif
	}	
}