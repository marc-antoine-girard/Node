using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Nodes
{
    public class ExitNode : BaseNode
    {
        private ExitModule Script = ScriptableObject.CreateInstance<ExitModule>();
        public override Type ScriptType => typeof(ExitModule);
        public ExitNode() { }
        public ExitNode(string nodeName, Rect  position, string guid, List<string> outputPortIDs) : base(nodeName, position, guid, outputPortIDs) { }
    
        public new static ExitNode Create(string nodeName, Rect position, string guid, List<string> outputPortIDs)
        {
            return new ExitNode(nodeName, position, guid, outputPortIDs);
        }

        protected override void DrawNode(ModuleGraphView graphView)
        {
            AddToClassList("exit");
            var generatedPort = graphView.GeneratePort<float>(this, Direction.Input, Port.Capacity.Multi);
            generatedPort.portName = "Exit";
        
            inputContainer.Add(generatedPort);

            capabilities &= ~Capabilities.Deletable;
            capabilities &= ~Capabilities.Copiable;
            capabilities &= ~Capabilities.Renamable;
            capabilities &= ~Capabilities.Resizable;
        
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