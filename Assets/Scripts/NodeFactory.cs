using System;
using System.Collections.Generic;
using Nodes;
using UnityEngine;

public class NodeFactory
{
    //TODO use NodeType to categorize Types.
    public static BaseNode CreateNode(ActionNodeData aData)
    {
        if (aData.Type == null) return null;
        
        aData.GUID = string.IsNullOrEmpty(aData.GUID) ? Guid.NewGuid().ToString() : aData.GUID;
        aData.OutputPortIDs = aData.OutputPortIDs ?? new List<string>();
        
        BaseNode node;
        if (aData.Type == typeof(ChangeSceneNode))
        {
            node = ChangeSceneNode.Create("Change Scene", aData.Position, aData.GUID,
                aData.OutputPortIDs, aData.NodeType);
        }
        else if (aData.Type == typeof(StartNode))
        {
            node = StartNode.Create("Start Node", aData.Position, aData.GUID,
                aData.OutputPortIDs, aData.NodeType);
        }
        else if (aData.Type == typeof(ConditionalNode))
        {
            node = ConditionalNode.Create("Conditional", aData.Position, aData.GUID,
                aData.OutputPortIDs, aData.NodeType);
        }
        else if (aData.Type == typeof(MultiNode))
        {
            node = MultiNode.Create("Multi Output", aData.Position, aData.GUID,
                aData.OutputPortIDs, aData.NodeType);
        }
        else if (aData.Type == typeof(ExitNode))
        {
            node = ExitNode.Create("Exit Node", aData.Position, aData.GUID,
                aData.OutputPortIDs, aData.NodeType);
        }
        else if (aData.Type == typeof(RandomNode))
        {
            node = RandomNode.Create("Random Node", aData.Position, aData.GUID,
                aData.OutputPortIDs, aData.NodeType);
        }
        else
        {
            throw new NotImplementedException("Node type not implemented in NodeFactory");
        }

        node?.SetSerializedScript(aData.SerializedScript);
        return node;
    }
}
