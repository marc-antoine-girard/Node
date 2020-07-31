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

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private ModuleGraphView graphView;
    private EditorWindow window;

    public void Init(EditorWindow window, ModuleGraphView graphView)
    {
        this.graphView = graphView;
        this.window = window;
    }
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
            new SearchTreeGroupEntry(new GUIContent("Actions"), 1),
            new SearchTreeEntry(new GUIContent("Change Scene"))
            {
                userData = new ChangeSceneNode {NodeType = typeof(ChangeSceneNode)}, level = 2
            },
            new SearchTreeEntry(new GUIContent("Take Object"))
            {
                userData = new TakeObjectNode {NodeType = typeof(TakeObjectNode)}, level = 2
            },
            new SearchTreeGroupEntry(new GUIContent("Outputs"), 1),
            new SearchTreeEntry(new GUIContent("Conditional"))
            {
                userData = new ConditionalNode{NodeType = typeof(ConditionalNode)}, level = 2
            },
            new SearchTreeEntry(new GUIContent("Multi"))
            {
                userData = new MultiNode{NodeType = typeof(MultiNode)}, level = 2
            },
            new SearchTreeEntry(new GUIContent("Random"))
            {
                userData = new RandomNode{NodeType = typeof(RandomNode)}, level = 2
            }
            
        };
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        var worldMousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent,
            context.screenMousePosition - window.position.position);
        var localMousePosition = graphView.contentViewContainer.WorldToLocal(worldMousePosition);

        BaseNode node = (BaseNode)SearchTreeEntry.userData;
        ActionNodeData and = new ActionNodeData
        {
            Position = new Rect(localMousePosition, graphView.defaultNodeSize),
            GUID = Guid.NewGuid().ToString(),
            OutputPortIDs = new List<string>(),
            NodeType = node.NodeType.AssemblyQualifiedName
        };
        BaseNode temp = NodeFactory.CreateNode(and);
        temp?.Draw(graphView);
        
        //if tempEdge is not null, this means that the search window 
        if (graphView.tempEdge != null && temp != null)
        {
            //get the inpût port of the new node
            var tempInput = graphView.GetInputPorts(temp).ToList();

            if (tempInput.Count == 0)
            {
                graphView.tempPort = null;
                graphView.tempEdge = null;
            
                return true;
            }
            var inputPort = tempInput.First();

            //if the output port is single and already connected. Must disconnect it.
            if (graphView.tempPort.capacity == Port.Capacity.Single && graphView.tempPort.connected)
            {
                var edge = graphView.edges.ToList().Where(x => x.output == graphView.tempPort);
                if (edge.Any())
                {
                    var e = edge.First();
                    e.input.Disconnect(e);
                    e.output.Disconnect(e);
                    graphView.RemoveElement(e);
                }
            }
            var tempEdge = new Edge
            {
                input = inputPort,
                output = graphView.tempPort
            };
            inputPort.Connect(tempEdge);
            graphView.tempPort.Connect(tempEdge);
            graphView.Add(tempEdge);
            
            graphView.tempPort = null;
            graphView.tempEdge = null;
            
            return true;
        }

        //Return false doesn't close window
        return true;
    }
}
