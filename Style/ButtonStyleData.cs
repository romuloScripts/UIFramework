using UnityEngine;
using UnityEngine.UI;

namespace UIFramework {
	[CreateAssetMenu]
	public class ButtonStyleData : ScriptableObject {

		public Vector2 rect = new Vector2(100,100);
		public ColorBlock colors;
		[Range(1,5)]
		public float scaleMultiplier=1.1f;
		public Vector2 shadow;
		public Color shadowColor;
	}	
}