using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nodes
{
    public class MultiNode : BaseNode
    {
        public MultiNode()
        {
        }
        public MultiNode(string nodeName, Rect position, string guid, List<string> outputPortIDs, NodeType nodeType) : base(nodeName, position, guid, outputPortIDs, nodeType)
        {
        }
    
        public new static MultiNode Create(string nodeName, Rect position, string guid, List<string> OutputPortIDs, NodeType nodeType)
        {
            var startNode = new MultiNode(nodeName, position, guid, OutputPortIDs, nodeType);
            return startNode;
        }
    
        public override void DrawNode(ModuleGraphView graphView)
        {
            var inputPort = graphView.GeneratePort<float>(this, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            inputContainer.Add(inputPort);
            titleContainer.Insert(1, new Button(() =>
            {
                OutputPortIDs.Add(graphView.AddMultiRow(this).name);
            }){ text = "Add", style = { flexGrow = 0}});
        
            foreach (var guid in OutputPortIDs)
            {
                var outputPort = graphView.GeneratePort<float>(this, Direction.Output);
                outputPort.portName = "Output";
                outputPort.name = guid;
                var deleteButton = new Button(() =>
                {
                    OutputPortIDs.Remove(outputPort.name);
                    graphView.RemovePort(this, outputPort);
                    graphView.RefreshNode(this);
                }){ text = "-", style = { width = 10}};
                outputPort.contentContainer.Add(deleteButton);
                outputContainer.Add(outputPort);
            }
        
            graphView.RefreshNode(this);
            graphView.AddElement(this);
        }
    }
}