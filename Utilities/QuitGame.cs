using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuitGame : MonoBehaviour {

	public float delay;	
	public Button button;
	public UnityEvent onQuitGame;

	void Start(){
		if(button)
			button.onClick.AddListener(Quit);
	}

	void Reset(){
		button = GetComponent<Button>();
	}

	public void Quit(){
		onQuitGame.Invoke();
		if(delay >0)
			StartCoroutine(QuitDelay(name));
		else
			Application.Quit();
	}

	IEnumerator QuitDelay(string name){
		yield return new WaitForSecondsRealtime(delay);
		Application.Quit();
	}
}
