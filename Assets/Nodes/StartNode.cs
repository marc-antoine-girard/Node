using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class StartNode : BaseNode
{
    public StartNode()
    {
    }
    public StartNode(string nodeName, Rect position, string guid, List<string> outputPortIDs, NodeType nodeType) : base(nodeName, position, guid, outputPortIDs, nodeType)
    {
    }
    
    public new static BaseNode Create(string nodeName, Rect position, string guid, List<string> OutputPortIDs, NodeType nodeType)
    {
        var startNode = new StartNode(nodeName, position, guid, OutputPortIDs, nodeType);
        return startNode;
    }
    
    public override void DrawNode(ModuleGraphView graphView)
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

}
