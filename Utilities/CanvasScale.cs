using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScale : MonoBehaviour {

	public AnimationCurve curve = AnimationCurve.Linear(0,0,1,1);
	public float time = 1f;
	public float timeDisable = 1f;

	private RectTransform rect;
	private float t;

	public void Awake ()
	{
		SetRect();
	}

	public void SetRect()
	{
		rect = GetComponent<RectTransform>();
	}

	public void play () {
		t = 0f;
		enabled = true;
		apply();
	}

	void Update () {
		t += Time.unscaledDeltaTime;
		apply();
	}

	public void apply () {
		rect.localScale = Vector3.one * curve.Evaluate(t / time);
		if (timeDisable > 0f && t >= timeDisable)
			enabled = false;
	}

}
