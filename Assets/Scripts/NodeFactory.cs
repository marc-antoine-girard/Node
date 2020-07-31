using System;
using System.Collections.Generic;
using Nodes;
using UnityEngine;

public class NodeFactory
{
    //TODO use NodeType to categorize Types.
    public static BaseNode CreateNode(ActionNodeData aData)
    {
        if (aData.NodeType == null)
        {
            Debug.LogError("Type not set");
            return null;
        }

        aData.GUID = string.IsNullOrEmpty(aData.GUID) ? Guid.NewGuid().ToString() : aData.GUID;
        aData.OutputPortIDs = aData.OutputPortIDs ?? new List<string>();
        
        //The copy prevent the node to modify the list in the ActionNodeData
        List<string> copy = new List<string>();
        foreach (var outputPort in aData.OutputPortIDs) copy.Add(outputPort);
        
        BaseNode node;
        Type type = Type.GetType(aData.NodeType);

        if (type == typeof(ChangeSceneNode))
        {
            node = ChangeSceneNode.Create("Change Scene", aData.Position, aData.GUID, copy);
        }
        else if (type == typeof(StartNode))
        {
            node = StartNode.Create("Start Node", aData.Position, aData.GUID, copy);
        }
        else if (type == typeof(ConditionalNode))
        {
            node = ConditionalNode.Create("Conditional", aData.Position, aData.GUID, copy);
        }
        else if (type == typeof(MultiNode))
        {
            node = MultiNode.Create("Multi Output", aData.Position, aData.GUID, copy);
        }
        else if (type == typeof(ExitNode))
        {
            node = ExitNode.Create("Exit Node", aData.Position, aData.GUID, copy);
        }
        else if (type == typeof(RandomNode))
        {
            node = RandomNode.Create("Random Node", aData.Position, aData.GUID, copy);
        }
        else if (type == typeof(TakeObjectNode))
        {
            node = TakeObjectNode.Create("Take Object", aData.Position, aData.GUID, copy);
        }
        else
        {
            throw new NotImplementedException($"Node type {type} not handled in NodeFactory");
        }

        node?.SetSerializedScript(aData.SerializedScript);
        return node;
    }
}
