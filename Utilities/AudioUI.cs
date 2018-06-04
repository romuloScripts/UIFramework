using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class AudioUI : MonoBehaviour, ISelectHandler, ISubmitHandler, IMoveHandler {

	public bool submit=true,move,useSelectAudio;
	public bool verticalMove;
	public UnityEvent onSubmit;

	public void OnSelect (BaseEventData eventData){
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
}
