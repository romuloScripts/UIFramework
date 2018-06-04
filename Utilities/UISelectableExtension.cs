using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions{

    [AddComponentMenu("UI/Extensions/UI Selectable Extension")]
    [RequireComponent(typeof(Selectable))]
    public class UISelectableExtension : MonoBehaviour, IPointerDownHandler, IPointerUpHandler{
        #region Sub-Classes
        [System.Serializable]
        public class UIButtonEvent : UnityEvent<PointerEventData.InputButton> { }
        #endregion

        #region Events
		[Tooltip("Event that fires when a button is initially pressed down")]
        public UIButtonEvent OnButtonPress;
		[Tooltip("Event that fires when a button is released")]
        public UIButtonEvent OnButtonRelease;
		[Tooltip("Event that continually fires while a button is held down")]
        public UIButtonEvent OnButtonHeld;
        #endregion
		
		private bool _pressed;
        private PointerEventData _heldEventData;

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData){

            if (OnButtonPress != null)
            {
                OnButtonPress.Invoke(eventData.button);
            }
			_pressed = true;
            _heldEventData = eventData;
        }


        void IPointerUpHandler.OnPointerUp(PointerEventData eventData){
            if (OnButtonRelease != null)
            {
                OnButtonRelease.Invoke(eventData.button);
            }
 			_pressed = false;
            _heldEventData = null;
       }
	   
	    void Update(){
			if (!_pressed)
				return;
			
			if (OnButtonHeld != null){
                OnButtonHeld.Invoke(_heldEventData.button);
            }
		}

		public void TestClicked(){
			#if DEBUG || UNITY_EDITOR
				Debug.Log("Control Clicked");
			#endif
		}

		public void TestPressed(){
			#if DEBUG || UNITY_EDITOR
				Debug.Log("Control Pressed");
			#endif
		}

		public void TestReleased(){
			#if DEBUG || UNITY_EDITOR
				Debug.Log("Control Released");
			#endif
		}

		public void TestHold(){
			#if DEBUG || UNITY_EDITOR
				Debug.Log("Control Held");
			#endif
		}

        void OnDisable(){
            _pressed = false;
        }
    }
}