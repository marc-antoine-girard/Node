using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nodes
{
    public class RandomNode : BaseNode
    {
        public RandomNode()
        {
        }
        public RandomNode(string nodeName, Rect  position, string guid, List<string> outputPortIDs, NodeType nodeType) : base(nodeName, position, guid, outputPortIDs, nodeType)
        {
        }
    
        public new static RandomNode Create(string nodeName, Rect position, string guid, List<string> OutputPortIDs, NodeType nodeType)
        {
            return new RandomNode(nodeName, position, guid, OutputPortIDs, nodeType);
        }

        protected override void DrawNode(ModuleGraphView graphView)
        {
            var inputPort = graphView.GeneratePort<float>(this, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            inputContainer.Add(inputPort);
            
            //Set the Add Button
            titleContainer.Insert(1, new Button(() =>
            {
                OutputPortIDs.Add(AddMultiRow(this, graphView).name);
            }){ text = "Add", style = { flexGrow = 0}});
        
            //Add saved port, none otherwise
            foreach (var guid in OutputPortIDs)
            {
                Port port = AddMultiRow(this, graphView);
                port.name = guid;
            }
        
            graphView.RefreshNode(this);
            graphView.AddElement(this);
        }
        private Port AddMultiRow(RandomNode node, ModuleGraphView graphView)
        {
            var temp = graphView.GeneratePort<float>(node, Direction.Output);
            temp.portName = "Output";
            temp.name = Guid.NewGuid().ToString();
            var deleteButton = new Button(() =>
            {
                node.OutputPortIDs.Remove(temp.name);
                graphView.RemovePort(node, temp);
                graphView.RefreshNode(node);
            }){ text = "-", style = { width = 10}};
            temp.contentContainer.Add(deleteButton);
            node.outputContainer.Add(temp);
            graphView.RefreshNode(node);
            return temp;
        }
    }
}