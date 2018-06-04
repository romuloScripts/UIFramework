using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework {
	public class FontStyleBase : MonoBehaviour {
		
		public FontStyleData data;
		public bool setInAwake;

		void Reset(){
			SetNewStyle();
		}

		void OnValidate(){
			SetNewStyle();
		}

		void Awake(){
			if(setInAwake)
				SetNewStyle();
		}

		[ContextMenu("SetStyle")]
		public virtual void SetNewStyle(){
			if(!data) return;
		}
	}
}
