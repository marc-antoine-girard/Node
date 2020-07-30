using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Nodes
{
    public class BaseNode : Node
    {
        public BaseModule Script = ScriptableObject.CreateInstance<BaseModule>();

        public string GUID;
        public NodeType NodeType;
        public List<string> OutputPortIDs = new List<string>();
        public Type Type;
       
        public BaseNode() {}
        public BaseNode(string nodeName, Rect position, string guid, List<string> outputPortIDs, NodeType nodeType)
        {
            title = nodeName;
            GUID = guid;
            NodeType = nodeType;
            OutputPortIDs = outputPortIDs;
            Type = GetType();
            SetPosition(position);
        }
        public static BaseNode Create(string nodeName, Rect position, string guid, List<string> OutputPortIDs, NodeType nodeType)
        {
            return new BaseNode(nodeName, position, guid, OutputPortIDs, nodeType);
        }

        [Conditional("UNITY_EDITOR")]
        public void Draw(ModuleGraphView graphView)
        {
            DrawNode(graphView);
        }

        protected virtual void DrawNode(ModuleGraphView graphView)
        {
            throw new NotImplementedException();
        }

        public virtual string GetSerializedScript()
        {
            return JsonUtility.ToJson(Script);
        }
        public virtual void SetSerializedScript(string json)
        {
            JsonUtility.FromJsonOverwrite(json, Script);
        }
    }
}
