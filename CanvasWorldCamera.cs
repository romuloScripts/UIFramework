using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasWorldCamera : MonoBehaviour {

	public Canvas canvas;

	private void Reset() {
		canvas = GetComponent<Canvas>();
	}

	private void Start() {
		GetWorldCamera();
	}

	public void GetWorldCamera(){
		canvas.worldCamera = Camera.main;
	}
}
