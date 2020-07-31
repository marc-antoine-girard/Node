using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Nodes
{
    public class BaseNode : Node
    {
        // public BaseModule Script;
        private BaseModule Script = ScriptableObject.CreateInstance<BaseModule>();
        public virtual Type ScriptType => throw new NotImplementedException();


        public string GUID;
        public Type NodeType;
        public List<string> OutputPortIDs = new List<string>();
       
        public BaseNode() {}
        public BaseNode(string nodeName, Rect position, string guid, List<string> outputPortIDs)
        {
            title = nodeName;
            GUID = guid;
            OutputPortIDs = outputPortIDs;
            NodeType = GetType();
            SetPosition(position);
        }
        public static BaseNode Create(string nodeName, Rect position, string guid, List<string> outputPortIDs)
        {
            return new BaseNode(nodeName, position, guid, outputPortIDs);
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
