using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSaveUtility
{
    private ModuleGraphView targetGraphView;
    private ActionContainer containerCache;
    private List<Edge> edges => targetGraphView.edges.ToList();
    private List<BaseNode> nodes => targetGraphView.nodes.ToList().Cast<BaseNode>().ToList();
    public static GraphSaveUtility GetInstance(ModuleGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            targetGraphView = targetGraphView
        };
    }

    public bool SaveGraph(string fileName)
    {
        if (!edges.Any()) return false;

        var actionContainer = ScriptableObject.CreateInstance<ActionContainer>();

        //Cycle through every edges in GraphView
        //Add them to Nodelinks in ActionContainer
        foreach (var edge in edges)
        {
            var outputNode = edge.output.node as BaseNode;
            var inputNode = edge.input.node as BaseNode;
            
            actionContainer.NodeLinks.Add(new NodeLinkData
            {
                BaseNodeGuid = outputNode.GUID,
                PortName = edge.output.name,
                TargetNodeGuid = inputNode.GUID
            });
        }

        foreach (var baseNode in nodes)
        {
            List<string> copy = new List<string>();
            foreach (var outputPortID in baseNode.OutputPortIDs)
            {
                copy.Add(outputPortID);
            }
            actionContainer.ActionNodeDatas.Add(new ActionNodeData
            {
                GUID = baseNode.GUID,
                Position = baseNode.GetPosition(),
                OutputPortIDs = copy,
                NodeType = baseNode.NodeType,
                SerializedScript = baseNode.GetSerializedScript()
            });
        }

        //If Resources folder doesn't exist, create it.
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        
        AssetDatabase.CreateAsset(actionContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();

        return true;
    }
    public bool LoadGraph(string fileName)
    {
        containerCache = Resources.Load<ActionContainer>(fileName);

        if (containerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found",
                $"Target dialogue graph file does not exists. \nFilename: {fileName}", "OK");
            return false;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
        return true;
    }

    private void ConnectNodes()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            //Get all connections associated with the node[i]
            var connections = containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodes[i].GUID).ToList();
            var outputPorts = nodes[i].outputContainer.Children().ToList().Where(x => x.Q<Port>() != null).Cast<Port>().ToList();

            for (int j = 0; j < connections.Count; j++)
            {
                var targetNodeGuid = connections[j].TargetNodeGuid;
                var outputPort = outputPorts.First(x => x.name == connections[j].PortName);
                var targetNode = nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes((Port) targetNode.inputContainer[0], outputPort);
            }
        }
    }

    private void LinkNodes(Port input, Port output)
    {
        var tempEdge = new Edge
        {
            input = input,
            output = output
        };
        
        tempEdge?.input?.Connect(tempEdge);
        tempEdge?.output?.Connect(tempEdge);
        targetGraphView.Add(tempEdge);
    }

    private void CreateNodes()
    {
        foreach (var nodeData in containerCache.ActionNodeDatas)
        {
            var node = NodeFactory.CreateNode(nodeData);
            node?.Draw(targetGraphView);
            // var node = BaseNode.Create(nodeData.title, nodeData.Position, nodeData.GUID, nodeData.OutputPortIDs, nodeData.NodeType);

            // var nodePorts = containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.GUID).ToList();
            // nodePorts.ForEach(x => targetGraphView.AddChoicePort(tempNode, x.PortName));

        }
    }

    private void ClearGraph()
    {
        foreach (var node in nodes)
        {
            edges.Where(edge => edge.input.node == node).ToList().ForEach(edge => targetGraphView.RemoveElement(edge));
            
            targetGraphView.RemoveElement(node);
        }
        
    }
}
