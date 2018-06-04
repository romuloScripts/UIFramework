using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework {
	public class FontStyleGroup : FontStyleBase {
		
		private Text[] texts;

		[ContextMenu("SetStyleGroup")]
		public override void SetNewStyle(){
			if(!data) return;
			base.SetNewStyle();
			texts = gameObject.GetComponentsInChildren<Text>(true);
			foreach (var item in texts) {
				//if(item.GetComponent<ButtonStyle>()) continue;
				item.font = data.font;
				item.color = data.color;
			}
		}
	}
}