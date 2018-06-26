using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

namespace UIFramework {
	public class Menu : MonoBehaviour {

		public BaseRaycaster raycaster;
		public GameObject firstSelected;
		public bool remenberLastSelection;
		public bool destroyWhenClosed;
		public bool hideUnderneath=true;
		public bool notDeselectMouse=true;

		public string keyClose="Cancel";
		public Button closeTransition;
		public List<Transition> transitions= new List<Transition>();

		public UnityEvent onEnter,onEntered,onLeave,onLeft;

		private GameObject lastButton;
		private Menu menuUnderneath;
		private MenuManager manager;

		public void Open(MenuManager manager, Menu underneathMenu){
			this.manager = manager;
			manager.AddMenu(this);
			menuUnderneath = underneathMenu;
			underneathMenu?.Disable(hideUnderneath);
			Enable();
		}

		private void Start() {
			closeTransition?.onClick.AddListener(Close);
			foreach (var item in transitions){
				item.button.onClick.AddListener(()=>item.OpenMenu(manager,this));
			}
			lastButton = firstSelected;
		}

		private void Update() {
			if(notDeselectMouse){
				NotSelected();
			}
			if(menuUnderneath && !string.IsNullOrEmpty(keyClose) && Input.GetButtonUp(keyClose)){
				Close();
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

		public void Interactable(bool active){
			enabled = active;
			if(raycaster)
				raycaster.enabled = active;
			SelectButton(active);
		}

		void SelectButton(bool active){
			if(EventSystem.current){
				if(!active){
					if(remenberLastSelection)
						lastButton = EventSystem.current.currentSelectedGameObject;
				}else{
					if(lastButton && remenberLastSelection){
						EventSystem.current.SetSelectedGameObject(lastButton);
					}else if(firstSelected)
						EventSystem.current.SetSelectedGameObject(firstSelected);
				}
			}
		}

		public void Disable(bool hide){
			Interactable(false);
			onLeave.Invoke();
			if(hide)
				gameObject.SetActive(false);
			onLeft.Invoke();
		}

		public void Enable(){
			onEnter.Invoke ();
			gameObject.SetActive(true);
			Interactable(true);
			onEntered.Invoke ();
		}

		public void Close(){
			Disable(true);
			menuUnderneath?.Enable();
			if(destroyWhenClosed){
				manager.RemoveMenu(this);
				Destroy(gameObject);
			}
		}

		[System.Serializable]
		public class Transition{

			public Button button;
			public Menu toMenu;
			private Menu prefab;

			public void OpenMenu(MenuManager manager, Menu menu){
				if(prefab && !toMenu){
					toMenu = prefab;
				}
				if(toMenu){
					if(string.IsNullOrEmpty(toMenu.gameObject.scene.name)){
						prefab = toMenu;
						toMenu = Instantiate<Menu>(toMenu,manager.transform);
					}
					toMenu.Open(manager,menu);
				}
			}
		}
	}	
}