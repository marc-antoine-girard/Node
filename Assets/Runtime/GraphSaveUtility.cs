using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public void SaveGraph(string fileName)
    {
        if (!edges.Any()) return;

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
                PortName = edge.output.portName,
                TargetNodeGuid = inputNode.GUID
            });
        }

        foreach (var dialogueNode in nodes)
        {
            actionContainer.ActionNodeDatas.Add(new ActionNodeData
            {
                GUID = dialogueNode.GUID,
                DialogueText = dialogueNode.DialogueText,
                Position = dialogueNode.GetPosition(),
                NodeType = dialogueNode.NodeType
            });
        }

        //If Resources folder doesn't exist, create it.
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        
        AssetDatabase.CreateAsset(actionContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }
    public void LoadGraph(string fileName)
    {
        containerCache = Resources.Load<ActionContainer>(fileName);

        if (containerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found",
                $"Target dialogue graph file does not exists. \nFilename: {fileName}", "OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();

    }

    private void ConnectNodes()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            var connections = containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodes[i].GUID).ToList();
            for (int j = 0; j < connections.Count; j++)
            {
                var targetNodeGuid = connections[j].TargetNodeGuid;
                var targetNode = nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes((Port) targetNode.inputContainer[0], nodes[i].outputContainer[j].Q<Port>());

                targetNode.SetPosition(containerCache.ActionNodeDatas.First(x => x.GUID == targetNodeGuid).Position);
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
        
        tempEdge?.input.Connect(tempEdge);
        tempEdge?.output.Connect(tempEdge);
        targetGraphView.Add(tempEdge);
    }

    private void CreateNodes()
    {
        foreach (var nodeData in containerCache.ActionNodeDatas)
        {
            var tempNode = targetGraphView.CreateNode(nodeData.DialogueText, nodeData.NodeType);
            tempNode.GUID = nodeData.GUID;

            var nodePorts = containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.GUID).ToList();
            nodePorts.ForEach(x => targetGraphView.AddChoicePort(tempNode, x.PortName));
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
