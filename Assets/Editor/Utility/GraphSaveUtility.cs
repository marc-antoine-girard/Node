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

    public ActionContainer SaveGraph(string fileName)
    {
        if (!edges.Any()) return null;

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
                BasePortName = edge.output.name,
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
                NodeType = baseNode.NodeType.AssemblyQualifiedName,
                SerializedScript = baseNode.GetSerializedScript(),
                ScriptType = baseNode.ScriptType.AssemblyQualifiedName
            });
        }

        //If Resources folder doesn't exist, create it.
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        
        AssetDatabase.CreateAsset(actionContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();

        return actionContainer;
    }

    public bool LoadGraph(ActionContainer actionContainer)
    {
        containerCache = actionContainer;
        ClearGraph();
        CreateNodes();
        ConnectNodes();
        return true;
    }
    
    public bool LoadGraph(string fileName)
    {
        var container = Resources.Load<ActionContainer>(fileName);

        if (containerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found",
                $"Target dialogue graph file does not exists. \nFilename: {fileName}", "OK");
            return false;
        }
        
        return LoadGraph(container);
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
                var outputPort = outputPorts.First(x => x.name == connections[j].BasePortName);
                var targetNode = nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes(outputPort, (Port) targetNode.inputContainer[0]);
            }
        }
    }

    private void LinkNodes(Port output, Port input)
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
        }
    }

    public void ClearGraph()
    {
        foreach (var node in nodes)
        {
            edges.Where(edge => edge.input.node == node).ToList().ForEach(edge => targetGraphView.RemoveElement(edge));
            targetGraphView.RemoveElement(node);
        }
    }
    public static void ClearGraph(ModuleGraphView graphView)
    {
        foreach (var node in graphView.nodes.ToList())
        {
            graphView.edges.ToList().Where(edge => edge.input.node == node).ToList().ForEach(edge => graphView.RemoveElement(edge));
            graphView.RemoveElement(node);
        }
    }
}
