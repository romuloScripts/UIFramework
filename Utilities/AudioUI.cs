using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class AudioUI : MonoBehaviour, ISelectHandler, ISubmitHandler, IMoveHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler{

	public bool submit=true,move,useSelectAudio;
	public bool verticalMove;
	public bool UseMousePointer;
	public UnityEvent onSubmit;

	public void OnSelect (BaseEventData eventData){
		if(EventSystem.current && EventSystem.current.currentSelectedGameObject != gameObject)
			AudioButton.sSelect();
	}

	public void OnSubmit(BaseEventData eventData){
		Submit();
	}

	public void Submit(){
		if(!submit) return;
		onSubmit.Invoke();
		if(!useSelectAudio)
			AudioButton.sConfirm();
		else
			AudioButton.sSelect();
	}

	public void OnMove(AxisEventData eventData){
		if(!move) return;
		if(verticalMove && eventData.moveVector.x != 0) return;
		if(!verticalMove && eventData.moveVector.y != 0) return;
		if(!useSelectAudio)
			AudioButton.sConfirm();
		else
			AudioButton.sSelect();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if(UseMousePointer)
			ExecuteEvents.Execute(gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.selectHandler);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if(UseMousePointer)
			ExecuteEvents.Execute(gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.deselectHandler);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (UseMousePointer)
		{
			//ExecuteEvents.Execute(gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
			//if (!gameObject.activeSelf)
				Submit();
		}
	}
}
