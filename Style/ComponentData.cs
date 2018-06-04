using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class ComponentData : ScriptableObject {

	public Component[] components;

	public void CopyData(GameObject obj){
		foreach (var item in components)
		{
		  Component	comp = obj.GetComponent(item.GetType());
		  if(comp){
			  comp = item;
		  }
		}
	}
}
