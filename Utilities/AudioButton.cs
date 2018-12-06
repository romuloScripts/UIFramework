using UnityEngine;
using System.Collections;

public class AudioButton : MonoBehaviour {

	public AudioSource audioSource;
	public AudioClip selectAudio, confirmAudio;
	public bool firstTime;
	
	private static AudioButton singleton;

	void Awake(){
		if(singleton){
			return;
		}
		transform.SetParent(null,false);
		AudioListener.pause = false;
		singleton = this;
		audioSource.ignoreListenerPause=true;
	}

	public void Confirm(){
		audioSource.PlayOneShot(confirmAudio);
	}

	public void Select(){
		if(!firstTime){
			firstTime = true;
			return;
		}
		audioSource.PlayOneShot(selectAudio);
	}

	public static void sConfirm(){
		if(singleton)
			singleton.Confirm();
	}

	public static void sSelect(){
		if(singleton)
			singleton.Select();
	}
}
