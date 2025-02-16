using System;
using System.Collections.Generic;
using UnityEngine;

namespace mikealpha
{
    [Serializable]
    public class BehaviourTreeNode
    {
        public string name;

        [HideInInspector]
        public Rect rect;

        [HideInInspector]
        public Rect InNode, OutNode;
        public Node node;

        [HideInInspector]
        public bool InNodeBool;
        [HideInInspector]
        public bool OutNodeBool;


        public BehaviourTreeNode(string name, Vector2 pos/*, Node node*/, bool inNodeBool, bool outNodeBool)
        {
            this.name = name;
            this.rect = new Rect(pos.x, pos.y, 150, 50);
            //this.node = node;
            this.InNodeBool = inNodeBool;
            this.OutNodeBool = outNodeBool;

            if (OutNodeBool)
            {
                Debug.Log("Here---1");
                this.OutNode = new Rect(this.rect.center.x, this.rect.yMax, 10, 10);
            }

            if (InNodeBool)
            {
                Debug.Log("Here---2");
                this.InNode = new Rect(this.rect.center.x, this.rect.yMin - 10, 10, 10);
            }
            else if (InNodeBool && OutNodeBool)
            {
                Debug.Log("Here---3");
                this.InNode = new Rect(this.rect.center.x, this.rect.yMin - 10, 10, 10);
                this.OutNode = new Rect(this.rect.center.x, this.rect.yMax, 10, 10);
            }
        }

        public void Draw(GUIStyle InNodeStyle, GUIStyle OutNodeStyle)
        {
            GUI.Box(rect, name);

            if (InNodeStyle == null && OutNodeStyle != null)
            {
                if(OutNodeBool)
                    GUI.Box(OutNode, string.Empty, OutNodeStyle);
                
                if(InNodeBool)
                    GUI.Box(InNode, string.Empty);
            }

            else if (OutNodeStyle == null && InNodeStyle != null)
            {
                if(InNodeBool)
                    GUI.Box(InNode, string.Empty, InNodeStyle);
                
                if(OutNodeBool)
                    GUI.Box(OutNode, string.Empty);
            }
            else
            {
                if(OutNodeBool)
                    GUI.Box(OutNode, string.Empty);
                
                if(InNodeBool)
                    GUI.Box(InNode, string.Empty);
            }
        }

        public void UpdatePosition(Vector2 delta)
        {
            this.rect.position += delta;

            if (OutNodeBool)
            {
                Debug.Log("Here---1");
                this.OutNode = new Rect(this.rect.center.x, this.rect.yMax, 10, 10);
            }

            if (InNodeBool)
            {
                Debug.Log("Here---2");
                this.InNode = new Rect(this.rect.center.x, this.rect.yMin - 10, 10, 10);
            }
            else if (InNodeBool && OutNodeBool)
            {
                Debug.Log("Here---3");
                this.InNode = new Rect(this.rect.center.x, this.rect.yMin - 10, 10, 10);
                this.OutNode = new Rect(this.rect.center.x, this.rect.yMax, 10, 10);
            }
        }
    }
}
