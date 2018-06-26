using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
namespace UIFramework {
	[CustomEditor(typeof(MenuManager))]
	public class MenuManagerEditor : Editor {

		string[] buttonsNames;

//		private MenuManager m;

		public override void OnInspectorGUI (){
//			m = (MenuManager)target;
			base.OnInspectorGUI ();
			/* if(m.menuTemplate){
				GUILayout.BeginHorizontal();
				if(GUILayout.Button("Create Menu")){
					m.CreateMenu();
				}
				if(GUILayout.Button("ApplyPrefabs") && EditorUtility.DisplayDialog("Overwrite prefabs","This will overwrite your current prefabs!\nApply prefabs?","Apply","Don't apply")){
					m.ApplyPrefabs();
				}
				GUILayout.EndHorizontal();
			} */
		}
	}	
}