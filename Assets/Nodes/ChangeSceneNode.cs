using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;

namespace Nodes
{
    public class ChangeSceneNode : BaseNode
    {
        public ChangeSceneNode()
        {
        }

        public ChangeSceneNode(string nodeName, Rect position, string guid, List<string> outputPortIDs, NodeType nodeType) : base(nodeName, position, guid, outputPortIDs, nodeType)
        {
        }

        public new static ChangeSceneNode Create(string nodeName, Rect position, string guid, List<string> OutputPortIDs, NodeType nodeType)
        {
            return new ChangeSceneNode(nodeName, position, guid, OutputPortIDs, nodeType);
        }

        protected override void DrawNode(ModuleGraphView graphView)
        {
            var inputPort = graphView.GeneratePort<float>(this, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            inputContainer.Add(inputPort);
            AddToClassList("action");

            var outputPort = graphView.GeneratePort<float>(this, Direction.Output);
            outputPort.portName = "Output";
            outputContainer.Add(outputPort);

            ObjectField a = new ObjectField();
            a.objectType = typeof(GameObject);
            extensionContainer.Add(a);
            graphView.RefreshNode(this);
            graphView.AddElement(this);

        }
    }
}
