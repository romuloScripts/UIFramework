using System.Collections.Generic;
using UnityEngine;

namespace UIFramework {
	[CreateAssetMenu]
	public class MenuDesign : ScriptableObject {

		[HideInInspector,SerializeField]
		public List<Menu> menuPrefabs;
		[HideInInspector,SerializeField]
		public List<Node> nodes=new List<Node>();
		[HideInInspector,SerializeField]
		public List<Connection> connections=new List<Connection>();

		#if UNITY_EDITOR
		public void RemoveNode(Node node){
			if (connections != null){
				List<Connection> connectionsToRemove = new List<Connection>();
				for (int i = 0; i < nodes.Count; i++) {
					if(nodes[i] == node){
						for (int j = 0; j <connections.Count; j++){
							if (connections[j].nodeIn == i || connections[j].nodeOut == i){
								connectionsToRemove.Add(connections[j]);
							}
						}
						for (int k = 0; k < connections.Count; k++){
							if(i < connections[k].nodeIn)
								connections[k].nodeIn -=1;
							if(i < connections[k].nodeOut)
								connections[k].nodeOut -=1;
						}
						break;
					}
				}     
				for (int i = 0; i < connectionsToRemove.Count; i++){
					connections.Remove(connectionsToRemove[i]);
				}
				connectionsToRemove = null;
			}
			nodes.Remove(node);
		}	
		#endif
	}
}