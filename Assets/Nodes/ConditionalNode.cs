using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nodes
{
    public class ConditionalNode : BaseNode
    {
        public ConditionalNode() { }
        public ConditionalNode(string nodeName, Rect position, string guid, List<string> outputPortIDs, NodeType nodeType) : base(nodeName, position, guid, outputPortIDs, nodeType) { }
    
        public new static ConditionalNode Create(string nodeName, Rect position, string guid, List<string> OutputPortIDs, NodeType nodeType)
        {
            return new ConditionalNode(nodeName, position, guid, OutputPortIDs, nodeType);
        }

        protected override void DrawNode(ModuleGraphView graphView)
        {
            var inputPort = graphView.GeneratePort<float>(this, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            inputContainer.Add(inputPort);

            var Success = graphView.GeneratePort<float>(this, Direction.Output);
            Success.portName = "Success";
            outputContainer.Add(Success);

            var Failure = graphView.GeneratePort<float>(this, Direction.Output);
            Failure.portName = "Failure";
            outputContainer.Add(Failure);

            Debug.Log(Failure.name);
            
            graphView.RefreshNode(this);
            graphView.AddElement(this);
        }
    }
}