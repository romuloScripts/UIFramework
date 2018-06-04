using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasTextColor : MonoBehaviour {

	public Text[] vet;
	public Color normalColor = Color.white;
	public Color highlightedColor = Color.yellow;
	public AnimationCurve curve = AnimationCurve.Linear(0,0,1,1);
	public float time = 1f;
	public float timeDisable = 1f;

	private float t;

	public void play () {
		t = 0f;
		enabled = true;
		apply();
	}

	void Update () {
		t += Time.unscaledDeltaTime;
		apply();
	}

	void apply () {
		foreach(Text text in vet)
			text.color = Color.Lerp(normalColor, highlightedColor, curve.Evaluate(t / time));
		if (timeDisable > 0f && t >= timeDisable)
			enabled = false;
	}

}
