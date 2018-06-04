using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetComponentData : MonoBehaviour {

	public ComponentData data;

	[ContextMenu("Copy")]
	public void Get(){
		data.CopyData(gameObject);
	}
}
