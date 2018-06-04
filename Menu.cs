using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

namespace UIFramework {
	public class Menu : MonoBehaviour {

		public GameObject firstSelected;
		public bool remenberLastSelection;
		public bool notDeselectMouse=true;

		public List<Transition> transitions= new List<Transition>();
		public UnityEvent onEnter,onEntered,onLeave,onLeft;

		private GameObject lastButton;

		private void Start() {
			foreach (var item in transitions){
				item.button.onClick.AddListener(Disable);
				item.button.onClick.AddListener(item.EnableMenu);
			}
			lastButton = firstSelected;
		}

		private void Update() {
			if(notDeselectMouse){
				NotSelected();
			}
		}

		private void NotSelected(){
			if(!EventSystem.current) return;
			if(!EventSystem.current.currentSelectedGameObject && lastButton){
				EventSystem.current.SetSelectedGameObject(lastButton);
			}else{
				lastButton = EventSystem.current.currentSelectedGameObject;
			}
		}

		public void Disable(){
			if(remenberLastSelection && EventSystem.current){
				lastButton = EventSystem.current.currentSelectedGameObject;
			}
			onLeave.Invoke ();
			gameObject.SetActive(false);
			onLeft.Invoke ();
		}

		public void Enable(){
			onEnter.Invoke ();
			gameObject.SetActive(true);
			if(EventSystem.current){
				if(lastButton && remenberLastSelection){
					EventSystem.current.SetSelectedGameObject(lastButton);
				}else
					EventSystem.current.SetSelectedGameObject(firstSelected);
			}
			onEntered.Invoke ();
		}

		public void LinkTransitions(){
			//transitions.Clear();
			/* foreach (var item in template.connections) {
				Transition t = new Transition();
				t.LinkTransition(menus,item);
				//transitions.Add(t);
			} */
		}

		/* public void SetFirstButton(){
			if(!lastButton){
				EventSystem.current.SetSelectedGameObject(firstSelected);
			}
		} */

		[System.Serializable]
		public class Transition{

			public Button button;
			public Menu toMenu;

			public void EnableMenu(){
				toMenu.Enable();
			}

			public void LinkTransition(List<Menu> menus,Connection data){
				/* for (int i = 0; i < menus.Count; i++) {
					if(menus[i].name == data.menuOrigin.name){
						button = menus[i].transitionButtons[data.buttonOut];
					}else if(menus[i].name == data.menuTarget.name){
						toMenu = menus[i];
					}
				} */
			}
		}
	}	
}