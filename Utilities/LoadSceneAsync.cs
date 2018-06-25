using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSceneAsync : MonoBehaviour {
	
	public string scene;

	IEnumerator LoadAsync(string scene) {
		if(string.IsNullOrEmpty(scene)){
			yield break;
		}

		AsyncOperation async = SceneManager.LoadSceneAsync(scene);
		async.allowSceneActivation = false;
		
		while (!async.isDone){
			if (async.progress >= 0.9f){
				yield return new WaitForSeconds(1);
				async.allowSceneActivation = true;
			}
			yield return 0; 
		}
	}

	public void LoadScene(int scene){
		StartCoroutine(LoadAsync(SceneManager.GetSceneByBuildIndex(scene).name));
	}

	public void LoadScene(){
		StartCoroutine(LoadAsync(scene));
	}

	public void LoadScene(string scene){
		StartCoroutine(LoadAsync(scene));
	}
}
