using UnityEngine;
using UnityEngine.UI;

namespace UIFramework {
	public class ButtonStyleGroup : ButtonStyleBase {

		private Button[] button;

		[ContextMenu("SetStyleGroup")]
		public override void SetNewStyle(){
			base.SetNewStyle();
			button = gameObject.GetComponentsInChildren<Button>(true);
			foreach (var item in button) {
				if(item.GetComponent<ButtonStyle>()) continue;
				item.colors = data.colors;
				RectTransform rectTransform = item.GetComponent<RectTransform>();
				if(rectTransform){
					rectTransform.sizeDelta = data.rect;
				}
				if(item.targetGraphic){
					Shadow s =item.targetGraphic.GetComponent<Shadow>();
					if(data.shadow != Vector2.zero && !s){
						s = item.targetGraphic.gameObject.AddComponent<Shadow>();
					}else if(data.shadow == Vector2.zero && s){
						if(Application.isPlaying){
							Destroy(s);
						}else if(Application.isEditor){
							DestroyImmediate(s);
						}
					}
					if(s){
						s.effectDistance = data.shadow;
						s.effectColor = data.shadowColor;
					}	
				}
				Button2 b2 = item as Button2;
				if(b2){
					b2.scaleMultiplier = data.scaleMultiplier;
				}
			}
		}
	}	
}