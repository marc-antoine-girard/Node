using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Nodes
{
    public class ExitNode : BaseNode
    {
        public ExitNode()
        {
        }
        public ExitNode(string nodeName, Rect  position, string guid, List<string> outputPortIDs, NodeType nodeType) : base(nodeName, position, guid, outputPortIDs, nodeType)
        {
        }
    
        public new static ExitNode Create(string nodeName, Rect position, string guid, List<string> OutputPortIDs, NodeType nodeType)
        {
            return new ExitNode(nodeName, position, guid, OutputPortIDs, nodeType);
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
    }
}