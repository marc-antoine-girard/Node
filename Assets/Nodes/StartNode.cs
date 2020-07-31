using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Nodes
{
    public class StartNode : BaseNode
    {
        private StartModule Script = ScriptableObject.CreateInstance<StartModule>();
        public override Type ScriptType => typeof(StartModule);

        public StartNode(string nodeName, Rect position, string guid, List<string> outputPortIDs) : base(nodeName, position, guid, outputPortIDs) { }
    
        public new static StartNode Create(string nodeName, Rect position, string guid, List<string> outputPortIDs)
        {
            var node = new StartNode(nodeName, position, guid, outputPortIDs);
            return node;
        }

        protected override void DrawNode(ModuleGraphView graphView)
        {
            AddToClassList("start");
            var generatedPort = graphView.GeneratePort<float>(this, Direction.Output, Port.Capacity.Multi);
            generatedPort.portName = "Next";
            outputContainer.Add(generatedPort);
        
            capabilities &= ~Capabilities.Deletable;
            capabilities &= ~Capabilities.Copiable;
            capabilities &= ~Capabilities.Renamable;
        
            graphView.RefreshNode(this);
            graphView.AddElement(this);
        }
        
        public override string GetSerializedScript()
        {
            return JsonUtility.ToJson(Script);
        }
        public override void SetSerializedScript(string json)
        {
            JsonUtility.FromJsonOverwrite(json, Script);
        }
    }
}
