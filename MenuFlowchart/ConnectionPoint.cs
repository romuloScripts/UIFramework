using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UIFramework {

	public enum ConnectionPointType { In, Out }

	[System.Serializable]
	public class ConnectionPoint:ScriptableObject{

		public string textName="name";
		public ConnectionPointType type;
		
		[HideInInspector]
		public Rect rect;

#if UNITY_EDITOR
		public void Ini(ConnectionPointType type){
			this.type = type;
		}

		public void Draw(Node node,int i,GUISkin skin){
			rect = new Rect(0, 0, 20f, 20f);
			Rect label = new Rect(0,0,60,20);

			switch (type){
                case ConnectionPointType.In:
                    DrawInPoint(node, skin,ref rect, label);
                    break;

                case ConnectionPointType.Out:
                   	DrawOutPoint(node, skin, i,ref rect, label);
                    break;
            }
		}

        private void DrawOutPoint(Node node, GUISkin skin, int id,ref Rect rect, Rect label){
			if (id >= node.outPoint.Count) return;
            rect.y = node.rect.y + 5 + (25 * (id + 1));
            rect.x = node.rect.x + node.rect.width - 8f;
            label.x = rect.x;
            label.y = rect.y;
            label.x -= label.width;
            skin.textField.alignment = TextAnchor.MiddleRight;

            textName = GUI.TextField(label, textName, skin.textField);
            if (GUI.Button(rect, "", skin.button)){
                NodeEditor.ClickOutPoint(this);
            }
            float btnSize = 27f;
            if (GUI.Button(new Rect(label.x - 16, label.y + 5, btnSize, btnSize), "-", skin.customStyles[2])){
               Remove(node.outPoint);
            }
        }

        private void DrawInPoint(Node node, GUISkin skin, ref Rect rect, Rect label){
            if (node.outPoint.Count > 1)
                rect.y = node.rect.y + (node.rect.height * 0.5f) - rect.height * 0.5f;
            else
                rect.y = node.rect.y + 25;
            rect.x = node.rect.x - rect.width + 8f;
            label.x = rect.x;
            label.y = rect.y;
            label.x += 20;
            skin.label.alignment = TextAnchor.MiddleLeft;
            GUI.Label(label, "Open", skin.label);
            if (GUI.Button(rect, "", skin.button)){
                NodeEditor.ClickInPoint(this);
            }
        }
	
		public void Remove(List<ConnectionPoint> connectionPoints){
			connectionPoints?.Remove(this);
			DestroyImmediate(this,true);
			AssetDatabase.SaveAssets();
		}
#endif
    }
}