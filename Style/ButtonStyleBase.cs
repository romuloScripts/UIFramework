using UnityEngine;
namespace UIFramework {
	public class ButtonStyleBase : MonoBehaviour {

		public ButtonStyleData data;
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