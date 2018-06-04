using UnityEngine;
using UnityEngine.UI;

namespace UIFramework {
	public class ButtonStyle : ButtonStyleBase {

		private Button button;

		[ContextMenu("SetStyle")]
		public override void SetNewStyle(){
			base.SetNewStyle();
			button = gameObject.GetComponentInChildren<Button>(true);
			if(button){
				button.colors = data.colors;
				Button2 b2 = button as Button2;
				if(b2){
					b2.scaleMultiplier = data.scaleMultiplier;
				}
			}
		}
	}	
}