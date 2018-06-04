using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {

	public string sceneName;
	public float delay;	
	public Button button;
	public UnityEvent onLoadScene;

	void Start(){
		if(button)
			button.onClick.AddListener(Load);
	}

	void Reset(){
		button = GetComponent<Button>();
	}

	public void Load(){
		onLoadScene.Invoke();
		if(delay >0)
			StartCoroutine(LoadDelay(sceneName));
		else
			SceneManager.LoadScene(sceneName);
	}

	public void Load(string name){
		onLoadScene.Invoke();
		if(delay >0)
			StartCoroutine(LoadDelay(name));
		else
			SceneManager.LoadScene(name);
	}

	IEnumerator LoadDelay(string name){
		yield return new WaitForSecondsRealtime(delay);
		SceneManager.LoadScene(name);
	}
}
