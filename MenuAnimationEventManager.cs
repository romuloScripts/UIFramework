using System.Collections;
using System.Collections.Generic;
using GameArchiteture.Events;
using UIFramework;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Menu))]
public class MenuAnimationEventManager : GameEventListenerBase, IMenuComponent
{

	[EditScriptable]
	public GameEvent OnEnter, OnEntered;
	[EditScriptable]
	public GameEvent OnLeave, OnLeft;

	public bool DisableOnEnter;

	private Menu _menu;

	private void Awake()
	{
		Ini();
	}

	public bool Initialize { get; set; }

	public void Ini()
	{
		if(Initialize) return;
		Initialize = true;
		_menu = GetComponent<Menu>();
		
		if(DisableOnEnter)
			_menu.onEnter.AddListener(() => _menu.SetActiveGO(false));
		
		if (OnEntered)
		{
			_menu.waitEnterAnimationEnd = true;
			OnEntered.RegisterListener(this);
			
		}
		if(OnEnter)
			_menu.onEnter.AddListener(OnEnter.Raise);
		
		if (OnLeft)
		{
			OnLeft.RegisterListener(this);
			_menu.waitLeaveAnimationEnd = true;
			
		}
		if(OnLeave)
			_menu.onLeave.AddListener(OnLeave.Raise);	
	}

	public override void OnEventRaised(GameEvent gameEvent = null)
	{
		if (gameEvent == OnEntered)
		{
			_menu.SetActiveGO(true);
			_menu.OnEntered();
			
		}
		else if(gameEvent == OnLeft)
		{
			_menu.OnLeft();
		}
	}

	private void OnDestroy()
	{
		if(OnEntered)
			OnEntered.UnregisterListener(this);
		if(OnLeft)
			OnLeft.UnregisterListener(this);
	}
}
