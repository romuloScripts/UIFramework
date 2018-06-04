using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;

namespace UIFramework {
	[CustomEditor(typeof(MenuDesign))]
	public class MenuDesignEditor : Editor {

		private MenuDesign m;

		public override void OnInspectorGUI (){
			m = (MenuDesign)target;
			base.OnInspectorGUI ();
			if(GUILayout.Button("Open Editor")){
				NodeEditor.OpenWindow(m);
			}
		}
	}	
}