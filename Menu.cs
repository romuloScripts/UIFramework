using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

namespace UIFramework {

	public interface IMenuComponent
	{
		bool Initialize { get; }
		void Ini();
	}

	public class Menu : MonoBehaviour {
		
		[Foldout("Behaviour",true)]
		public BaseRaycaster raycaster;
		public GameObject firstSelected;
		public bool waitEnterAnimationEnd, waitLeaveAnimationEnd;
		public bool remenberLastSelection;
		public bool destroyWhenClosed;
		public bool hideUnderneath=true;
		public bool notDeselectMouse=true;
		public bool skipEvents;
		public bool SkipEvents
		{
			//get { return skipEvents; }
			set { skipEvents = value; }
		}

		[Foldout("Transitions",true)]
		public string keyClose="Cancel";
		public Button closeTransition;
		public List<Transition> transitions= new List<Transition>();
		
		[Foldout("Events",true)]
		public UnityEvent onEnter,onEntered,onLeave,onLeft,onSkipEnter;

		private GameObject lastButton;
		private Menu menuUnderneath;
		private MenuManager manager;
		private Action leftActions;
		private bool CreatedTransitions;

		public void Open(MenuManager manager, Menu underneathMenu){
			this.manager = manager;
			manager.AddMenu(this);
			if(!CreatedTransitions)
				SetTransitions();
			menuUnderneath = underneathMenu;
			underneathMenu?.Disable(hideUnderneath);
			Enable();
		}

		public void SetTransitions()
		{
			CreatedTransitions = true;
			closeTransition?.onClick.AddListener(Close);
			foreach (var item in transitions)
			{
				item.button.onClick.AddListener(() => item.OpenMenu(manager, this));
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
		
		public void Enable()
		{
			if (skipEvents)
			{
				gameObject.SetActive(true);
				Interactable(true);
				onSkipEnter.Invoke();
				return;
			}
			leftActions = null;
			OnEnter();
			if(!waitEnterAnimationEnd)
				OnEntered();
		}

		public void Disable(bool hide)
		{
			if (skipEvents)
			{
				Interactable(false);
				SetHide(hide);
				leftActions?.Invoke();
				leftActions = null;
				skipEvents = false;
				return;
			}
			leftActions += () => SetHide(hide);
			OnLeave();
			//onLeft.AddListener(() => SetHide(hide));
			if(!waitLeaveAnimationEnd)
				OnLeft();
		}
		
		private void OnEnter()
		{
			gameObject.SetActive(true);
			onEnter.Invoke();
		}

		public void OnEntered()
		{
			onEntered.Invoke();
			Interactable(true);
		}
		
		private void OnLeave()
		{
			Interactable(false);
			onLeave.Invoke();
		}

		public void OnLeft()
		{
			onLeft.Invoke();
			leftActions?.Invoke();
			leftActions = null;
		}

		private void SetHide(bool hide)
		{
			//onLeft.RemoveListener(() => SetHide(hide));
			if (hide)
				gameObject.SetActive(false);
		}

		public void Close()
		{
			//onLeft.AddListener(DestroyIfClosed);
			leftActions += DestroyIfClosed;
			Disable(true);
			menuUnderneath?.Enable();
		}

		private void DestroyIfClosed()
		{
			//onLeft.RemoveListener(DestroyIfClosed);
			if (destroyWhenClosed)
			{
				manager.RemoveMenu(this);
				Destroy(gameObject);
			}
		}

		public void AddTransition(Button b, Menu m){
			transitions.Add(new Transition(b,m));
		}

		public void InitializeComponents()
		{
			foreach (var component in GetComponents<IMenuComponent>())
			{
				component.Ini();
			}
		}
		
		public void InstantiateAndOpenMenu(Menu menu){
			menu = Instantiate<Menu>(menu,manager.transform);
			menu.InitializeComponents();
			menu.Open(manager,this);
		}

		public void ClickButton(Button b)
		{
			b.onClick.Invoke();
		}
		
		public Menu ClickButton(int i, bool skip)
		{
			transitions[i].OpenMenu(manager,this,skip);
			return transitions[i].toMenu;
		}

		[System.Serializable]
		public class Transition{

			public Button button;
			public Menu toMenu;
			private Menu prefab;

			public Transition(){}
			public Transition(Button b, Menu m){
				button = b;
				toMenu = m;
			}

			public void OpenMenu(MenuManager manager, Menu menu, bool skip=false){
				if(prefab && !toMenu){
					toMenu = prefab;
				}
				if(toMenu){
					if(string.IsNullOrEmpty(toMenu.gameObject.scene.name)){
					//if(toMenu.gameObject.name.Contains("(Clone)")){
						prefab = toMenu;
						toMenu = Instantiate<Menu>(toMenu,manager.transform);
						toMenu.InitializeComponents();
					}
					toMenu.skipEvents = skip;
					toMenu.Open(manager,menu);
				}
			}
		}
	}	
}