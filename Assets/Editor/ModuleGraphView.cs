using System;
using System.Collections.Generic;
using System.Linq;
using Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[Serializable]
public class ModuleGraphView : GraphView, IEdgeConnectorListener
{
    public readonly Vector2 DefaultNodeSize = new Vector2(400, 400);
    public bool IsDirty = false;
    public string LoadedFileName = "";
    public bool IsCachedFile;

    private StyleSheet gridStyle;
    private GridBackground grid;
    private NodeSearchWindow searchWindow;
    private ModuleGraph window;
    public Edge TempEdge;
    public Port TempPort;
    public ModuleGraphView(ModuleGraph window)
    {
        this.window = window;
        gridStyle = Resources.Load<StyleSheet>("GraphStyle");
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        
        grid = new GridBackground();
        grid.AddToClassList("grid");
        styleSheets.Add(gridStyle);
        Insert(0, grid);
        grid.StretchToParentSize();

        CreateEmptyNewGraph();
        AddSearchWindow();
        LiveChangeActionModule();
    }

    public void CreateEmptyNewGraph()
    {
        ClearGraph();
        CreateDefaultNodes();
        SetDirty(false);
    }
    public void ClearGraph()
    {
        LoadedFileName = "";
        foreach (var node in nodes.ToList())
        {
            edges.ToList().Where(edge => edge.input.node == node).ToList().ForEach(RemoveElement);
            RemoveElement(node);
        }
    }

    public void CreateDefaultNodes()
    {
        NodeFactory.CreateNode(new ActionNodeData
        {
            Position = new Rect(100, 200, 100, 150),
            NodeType = typeof(StartNode).AssemblyQualifiedName,
            GUID = Guid.NewGuid().ToString()
        }).Draw(this);
        
        NodeFactory.CreateNode(new ActionNodeData
        {
            Position = new Rect(500, 200, 100, 150),
            NodeType = typeof(ExitNode).AssemblyQualifiedName,
            GUID = Guid.NewGuid().ToString()
        }).Draw(this);
    }
    /// <summary>
    /// May be used to make live changes on ActionModule
    /// </summary>
    public void LiveChangeActionModule()
    {
        if (IsDirty) return;
        graphViewChanged += change =>
        {
            if (change.elementsToRemove != null || change.edgesToCreate != null || change.movedElements != null)
                SetDirty();
            
            return change;
        };
    }

    public void SetDirty(bool setDirty = true)
    {
        IsDirty = setDirty;
        string fileValue = string.IsNullOrEmpty(LoadedFileName) ? "new" : LoadedFileName;

        if (setDirty)
            SetName($"* {fileValue}");
        else
            SetName(fileValue);
    }

    public void SetName(string windowName)
    {
        string fileValue = string.IsNullOrEmpty(windowName) ? "new" : windowName;
        window.titleContent = new GUIContent($"{fileValue} : {ModuleGraph.DefaultName}");
    }

    private void AddSearchWindow()
    {
        searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        searchWindow.Init(window,this);
        nodeCreationRequest = context =>
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
    }

    public Port GeneratePort(BaseNode node, Direction portDirection, Type type, Port.Capacity capacity = Port.Capacity.Single)
    {
        var port = node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, type);
        
        // This is needed to handle the nodesearchwindow with edge drag
        port.RegisterCallback<MouseDownEvent>(evt => { TempPort = port; });
        // When the edge is dropped outside a node, OnDropOutsidePort is called
        // This does not work without the interface IEdgeConnectorListener
        port.AddManipulator(new EdgeConnector<Edge>(this));
        return port;
    }
    
    public Port GeneratePort<T>(BaseNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return GeneratePort(node, portDirection, typeof(T), capacity);
    }
    
    public void RemovePort(BaseNode node, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(x =>
            x.output.name == generatedPort.name && x.output.node == generatedPort.node);

        var enumerable = targetEdge as Edge[] ?? targetEdge.ToArray();
        
        if (!enumerable.Any())
        {
            RemoveElement(generatedPort);
            return;
        }

        var edge = enumerable.First();
        edge.input.Disconnect(edge);
        RemoveElement(edge);
        
        node.outputContainer.Remove(generatedPort);
        RefreshNode(node);
        MarkDirtyRepaint();
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach(port =>
        {
            if (startPort.node != port.node)
                if(!(startPort.direction == Direction.Output && port.direction == Direction.Output) 
                && !(startPort.direction == Direction.Input && port.direction == Direction.Input))
                    compatiblePorts.Add(port);
        });
        return compatiblePorts;
    }

    public void ToggleGrid(bool value)
    {
        if(value)
            grid.AddToClassList("grid");
        else
            grid.RemoveFromClassList("grid");
    }
    
    public void RefreshNode(Node node)
    {
        node.RefreshExpandedState();
        node.RefreshPorts();
    }

    public void OnDropOutsidePort(Edge edge, Vector2 position)
    {
        //Remove this line if you want bidirectional trigger
        if (TempPort.direction == Direction.Input) return;
        
        TempEdge = edge;
        //Add NodeSearchView Here
        nodeCreationRequest.Invoke(new NodeCreationContext
        {
            index = 1,
            target = this,
            screenMousePosition = Event.current.mousePosition + window.position.position
        });
    }

    public void OnDrop(GraphView graphView, Edge edge)
    {
    }
    
    
}
