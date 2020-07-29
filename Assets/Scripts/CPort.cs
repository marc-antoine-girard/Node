using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CPort : Port
{
    public CPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
    {
    }

    public override void Connect(Edge edge)
    {
        base.Connect(edge);
        Debug.Log(edge);
    }
}
