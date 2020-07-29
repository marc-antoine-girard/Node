using UnityEditor.Experimental.GraphView;

public class BaseNode : Node
{
    public string GUID;

    public string DialogueText;

    public bool EntryPoint = false;

    public bool ExitPoint = false;
    
    public NodeType NodeType;
}
