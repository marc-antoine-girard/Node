using System.Collections.Generic;
using System.Linq;
using Nodes;
using UnityEditor.Experimental.GraphView;

public static class ExtensionMethods
{
    public static IEnumerable<Port> GetOuputPorts(this ModuleGraphView graphView, BaseNode node)
    {
        return graphView.ports.ToList().Where(x => x.direction == Direction.Output && x.node == node);
    }
    public static IEnumerable<Port> GetInputPorts(this ModuleGraphView graphView, BaseNode node)
    {
        return graphView.ports.ToList().Where(x => x.direction == Direction.Input && x.node == node);
    }
}
