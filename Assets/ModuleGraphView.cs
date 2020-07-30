using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
public enum NodeType
{
    Start, Exit, Action, Conditional, Multi, Random
}

[Serializable]
public class ModuleGraphView : GraphView, IEdgeConnectorListener
{
    public readonly Vector2 defaultNodeSize = new Vector2(400, 400);
    public bool IsDirty;
    public UnityEvent OnElementChange = new UnityEvent();
    private StyleSheet gridStyle;
    private GridBackground grid;
    private NodeSearchWindow searchWindow;
    private EditorWindow window;
    public Edge tempEdge;
    public Port tempPort;
    public ModuleGraphView(EditorWindow window)
    {
        this.window = window;
        gridStyle = Resources.Load<StyleSheet>("Dialogue");
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        
        grid = new GridBackground();
        styleSheets.Add(gridStyle);
        Insert(0, grid);
        grid.StretchToParentSize();
        AddElement(GenerateEntryPointNode(new BaseNode {NodeType = NodeType.Start}));
        AddElement(GenerateExitPointNode(new BaseNode {NodeType = NodeType.Exit}));

        
        AddSearchWindow();
        LiveChangeActionModule();
        OnElementChange.AddListener(() =>
        {
            IsDirty = true;
            window.titleContent = new GUIContent($"{ModuleGraph.DefaultName} *");
        });
    }

    public void LiveChangeActionModule()
    {
        bool hasChanges = false;
        graphViewChanged += change =>
        {
            if (change.elementsToRemove != null)
            {
                foreach (var element in change.elementsToRemove)
                {
                    if (element is Edge)
                    {
                        hasChanges = true;
                        //Disconnect ActionModules in Live
                    }
                }
            }

            if (change.edgesToCreate != null)
            {
                foreach (var element in change.edgesToCreate)
                {
                    hasChanges = true;
                }
            }

            if (change.movedElements != null)
            {
                foreach (var element in change.movedElements)
                {
                    hasChanges = true;
                }
            }

            if (hasChanges)
                OnElementChange.Invoke();
            
            return change;
        };
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
        port.RegisterCallback<MouseDownEvent>(evt => { tempPort = port; });
        // When the edge is dropped outside a node, OnDropOutsidePort is called
        // This does not work without the interface IEdgeConnectorListener
        port.AddManipulator(new EdgeConnector<Edge>(this));
        return port;
    }
    
    public Port GeneratePort<T>(BaseNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return GeneratePort(node, portDirection, typeof(T), capacity);
    }

    private BaseNode GenerateEntryPointNode(BaseNode node)
    {
        if (string.IsNullOrEmpty(node.GUID))
            node = BaseNode.Create("Start Node", new Rect(100, 200, 100, 150),
                Guid.NewGuid().ToString(), new List<string>(), NodeType.Start);

        node.AddToClassList("start");
        var generatedPort = GeneratePort<float>(node, Direction.Output, Port.Capacity.Multi);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);
        
        node.capabilities &= ~Capabilities.Deletable;
        node.capabilities &= ~Capabilities.Copiable;
        node.capabilities &= ~Capabilities.Renamable;
        
        RefreshNode(node);
        return node;
    }

    private BaseNode GenerateExitPointNode(BaseNode node)
    {
        if (string.IsNullOrEmpty(node.GUID))
            node = BaseNode.Create("Exit Node", new Rect(300, 200, 100, 150),
                Guid.NewGuid().ToString(), new List<string>(), NodeType.Exit);
        
        node.AddToClassList("exit");
        var generatedPort = GeneratePort<float>(node, Direction.Input, Port.Capacity.Multi);
        generatedPort.portName = "Exit";
        
        node.inputContainer.Add(generatedPort);

        node.capabilities &= ~Capabilities.Deletable;
        node.capabilities &= ~Capabilities.Copiable;
        node.capabilities &= ~Capabilities.Renamable;
        node.capabilities &= ~Capabilities.Resizable;
        
        RefreshNode(node);
        return node;
    }

    public BaseNode CreateMultiNode(BaseNode node)
    {
        var inputPort = GeneratePort<float>(node, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        node.inputContainer.Add(inputPort);
        node.titleContainer.Insert(1, new Button(() =>
        {
            node.OutputPortIDs.Add(AddMultiRow(node).name);
        }){ text = "Add", style = { flexGrow = 0}});
        
        foreach (var guid in node.OutputPortIDs)
        {
            var outputPort = GeneratePort<float>(node, Direction.Output);
            outputPort.portName = "Output";
            outputPort.name = guid;
            var deleteButton = new Button(() =>
            {
                node.OutputPortIDs.Remove(outputPort.name);
                RemovePort(node, outputPort);
                RefreshNode(node);
            }){ text = "-", style = { width = 10}};
            outputPort.contentContainer.Add(deleteButton);
            node.outputContainer.Add(outputPort);
        }
        
        RefreshNode(node);
        return node;
    }
    
    public Port AddMultiRow(BaseNode node)
    {
        var temp = GeneratePort<float>(node, Direction.Output);
        temp.portName = "Output";
        temp.name = Guid.NewGuid().ToString();
        var deleteButton = new Button(() =>
        {
            node.OutputPortIDs.Remove(temp.name);
            RemovePort(node, temp);
            RefreshNode(node);
        }){ text = "-", style = { width = 10}};
        temp.contentContainer.Add(deleteButton);
        node.outputContainer.Add(temp);
        RefreshNode(node);
        return temp;
    }

    public void RemovePort(BaseNode node, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(x =>
            x.output.name == generatedPort.name && x.output.node == generatedPort.node);

        if (!targetEdge.Any())
        {
            RemoveElement(generatedPort);
            return;
        }

        var edge = targetEdge.First();
        edge.input.Disconnect(edge);
        RemoveElement(targetEdge.First());
        
        node.outputContainer.Remove(generatedPort);
        RefreshNode(node);
        MarkDirtyRepaint();
    }


    public BaseNode CreateActionNode(BaseNode node)
    {
        var inputPort = GeneratePort<float>(node, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        node.inputContainer.Add(inputPort);
        node.AddToClassList("action");

        var outputPort = GeneratePort<float>(node, Direction.Output);
        outputPort.portName = "Output";
        node.outputContainer.Add(outputPort);

        ObjectField a = new ObjectField();
        a.objectType = typeof(GameObject);
        node.mainContainer.Add(a);
        RefreshNode(node);
        return node;
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
    public void AddChoicePort(BaseNode baseNode, string overridenPortName = "")
    {
        var generatePort = GeneratePort<float>(baseNode, Direction.Output);
        
        //Removes the default port name using a query
        var oldLabel = generatePort.contentContainer.Q<Label>("type");
        oldLabel.visible = false;
        oldLabel.style.width = 0;
        oldLabel.style.marginLeft = 0;
        oldLabel.style.marginRight = 0;
        
        var outputPortCount = baseNode.outputContainer.Query("connector").ToList().Count;
        var choicePortName = string.IsNullOrEmpty(overridenPortName)
            ? $"Output {outputPortCount + 1}"
            : overridenPortName;
        
        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName,
            style = {width = 60}
        };
        textField.RegisterCallback<ChangeEvent<string>>(evt => generatePort.portName = evt.newValue);
        generatePort.contentContainer.Add(new Label(" "));
        generatePort.contentContainer.Add(textField);
        generatePort.portName = choicePortName;
        RefreshNode(baseNode);
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
        if (tempPort.direction == Direction.Input) return;
        
        tempEdge = edge;
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
