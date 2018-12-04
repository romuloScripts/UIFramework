using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonEvent : MonoBehaviour {

	public string nomeBotao="Cancel";
	public UnityEvent onPressionar;

	private Button b;
	
	void Awake(){
		b= GetComponent<Button>();	
	}

	void Update () {
		if(Input.GetButtonUp(nomeBotao))
		{
			OnPress();
		}		
	}

	public void OnPress()
	{
		onPressionar.Invoke();
		if (b)
		{
			//b.onClick.Invoke();
			ExecuteEvents.Execute(b.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
		}
	}
}
