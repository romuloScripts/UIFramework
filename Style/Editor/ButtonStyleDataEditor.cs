using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditorInternal;

namespace UIFramework {
	[CustomEditor(typeof(ButtonStyleData))]
	public class ButtonStyleDataEditor : Editor {

		private List<GameObject> objs = new List<GameObject>();

		public override void OnInspectorGUI (){
			base.OnInspectorGUI ();

			Scene scene = SceneManager.GetActiveScene();
			scene.GetRootGameObjects(objs);
			foreach (var item in objs) {
				foreach (var b in item.GetComponentsInChildren<ButtonStyleBase>(true)) {
					b.SetNewStyle();
				}
			}
		}
	}	
}