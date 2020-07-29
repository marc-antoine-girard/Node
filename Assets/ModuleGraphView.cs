using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
public enum NodeType
{
    Start, Exit, Action
}
[Serializable]
public class ModuleGraphView : GraphView
{
    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    private StyleSheet gridStyle;
    private GridBackground grid;
    public ModuleGraphView()
    {
        gridStyle = Resources.Load<StyleSheet>("Dialogue");
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        
        grid = new GridBackground();
        styleSheets.Add(gridStyle);
        Insert(0, grid);
        grid.StretchToParentSize();
        AddElement(GenerateEntryPointNode());
        AddElement(GenerateExitPointNode());
    }

    private Port GeneratePort<T>(BaseNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(T));
    }

    private BaseNode GenerateEntryPointNode()
    {
        var node = new BaseNode
        {
            title = "Start Node",
            name = "test",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "ENTRYPOINT",
            EntryPoint = true,
            NodeType = NodeType.Start
        };
        node.AddToClassList("start");
        var generatedPort = GeneratePort<float>(node, Direction.Output, Port.Capacity.Multi);
        
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);
        
        node.capabilities &= ~Capabilities.Deletable;
        node.capabilities &= ~Capabilities.Copiable;
        node.capabilities &= ~Capabilities.Renamable;
        
        RefreshNode(node);
        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }

    private BaseNode GenerateExitPointNode()
    {
        var node = new BaseNode
        {
            title = "Exit Node",
            GUID = Guid.NewGuid().ToString(),
            ExitPoint = true,
            NodeType = NodeType.Exit
        };
        node.AddToClassList("exit");
        var generatedPort = GeneratePort<float>(node, Direction.Input, Port.Capacity.Multi);
        generatedPort.portName = "Exit";
        node.inputContainer.Add(generatedPort);

        node.capabilities &= ~Capabilities.Deletable;
        node.capabilities &= ~Capabilities.Copiable;
        node.capabilities &= ~Capabilities.Renamable;
        node.capabilities &= ~Capabilities.Resizable;

        
        RefreshNode(node);
        node.SetPosition(new Rect(300, 200, 100, 150));
        return node;
    }

    public BaseNode CreateNode(string nodeName, NodeType nodeType = NodeType.Action)
    {
        BaseNode node;
        switch (nodeType)
        {
            case NodeType.Action:
                node = CreateActionNode(nodeName);
                break;
            case NodeType.Start:
                node = GenerateEntryPointNode();
                break;
            case NodeType.Exit:
                node = GenerateExitPointNode();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, null);
        }
        AddElement(node);
        return node;
    }
    public BaseNode CreateActionNode(string nodeName)
    {
        var dialogueNode = new BaseNode
        {
            title = nodeName,
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString(),
            NodeType = NodeType.Action
        };

        var inputPort = GeneratePort<float>(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);
        
        var outputPort = GeneratePort<float>(dialogueNode, Direction.Output);
        outputPort.portName = "Output";
        dialogueNode.outputContainer.Add(outputPort);

        AddElement(dialogueNode);
        
        RefreshNode(dialogueNode);
        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));
        return dialogueNode;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        ports.ForEach(port =>
        {
            if (startPort.node != port.node)
                if(!(startPort.direction == Direction.Output && port.direction == Direction.Output))
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
    
    public static void RefreshNode(Node node)
    {
        node.RefreshExpandedState();
        node.RefreshPorts();
    }
}
