using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace UnityEngine.UI {
 
    public class Button2: Button, IPointerEnterHandler, ISelectHandler{//, ISelectHandler {
 
		public float scaleMultiplier = 1.1f;
		
		private Vector3 scala;
		
		protected override void Awake (){
			scala = transform.localScale;
			base.Awake ();
			onClick.AddListener(AudioButton.sConfirm);
		}

        public override void OnPointerEnter(PointerEventData eventData){
            base.OnPointerEnter(eventData);
			EventSystem.current.SetSelectedGameObject(gameObject);
        }

		public override void OnSelect (BaseEventData eventData){
			base.OnSelect (eventData);
			//Debug.Log("a");
			transform.localScale = scala*scaleMultiplier;
			AudioButton.sSelect();
		}

		public override void OnDeselect (BaseEventData eventData){
			base.OnDeselect (eventData);
			transform.localScale = scala;
		}

		public void Clicar(){
			onClick.Invoke();
		} 

		public void ProximoSelecionado(GameObject obj){
			EventSystem.current.SetSelectedGameObject(obj);
		}
    }
}