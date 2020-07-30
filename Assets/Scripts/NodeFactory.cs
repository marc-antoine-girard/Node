using System;
using Nodes;

public class NodeFactory
{
    public static BaseNode CreateNode(ActionNodeData actionNodeData)
    {
        BaseNode node;
        if (actionNodeData.Type == typeof(ChangeSceneNode))
        {
            node = ChangeSceneNode.Create(actionNodeData.title, actionNodeData.Position, actionNodeData.GUID,
                actionNodeData.OutputPortIDs, actionNodeData.NodeType);
        }
        else if (actionNodeData.Type == typeof(StartNode))
        {
            node = StartNode.Create(actionNodeData.title, actionNodeData.Position, actionNodeData.GUID,
                actionNodeData.OutputPortIDs, actionNodeData.NodeType);
        }
        else if (actionNodeData.Type == typeof(ConditionalNode))
        {
            node = ConditionalNode.Create(actionNodeData.title, actionNodeData.Position, actionNodeData.GUID,
                actionNodeData.OutputPortIDs, actionNodeData.NodeType);
        }
        else if (actionNodeData.Type == typeof(MultiNode))
        {
            node = MultiNode.Create(actionNodeData.title, actionNodeData.Position, actionNodeData.GUID,
                actionNodeData.OutputPortIDs, actionNodeData.NodeType);
        }
        else
        {
            throw new NotImplementedException("Node type not implemented in NodeFactory");
        }
        
        return node;
    }
}
