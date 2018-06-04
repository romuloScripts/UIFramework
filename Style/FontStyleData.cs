using UnityEngine;
using UnityEngine.UI;

namespace UIFramework {
	[CreateAssetMenu]
	public class FontStyleData : ScriptableObject {
		public Font font;
		public Color color;	
		public Text text;
	}
}
