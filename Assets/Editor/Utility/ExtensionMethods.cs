using System;
using System.Collections.Generic;
using System.Linq;
using Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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

/// <summary>
/// Use it like this: UnityObjectHelper.FindObjectFromInstanceID(objectID);
/// Utility class to get Object with ID
/// </summary>
public static class UnityObjectHelper
{
    private static Func<int, UnityEngine.Object> m_FindObjectFromInstanceID;
    static UnityObjectHelper()
    {
        var methodInfo = typeof(UnityEngine.Object).GetMethod("FindObjectFromInstanceID",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        if (methodInfo == null)
            Debug.LogError("FindObjectFromInstanceID was not found in UnityEngine.Object");
        else
            m_FindObjectFromInstanceID = (Func<int, UnityEngine.Object>)Delegate.CreateDelegate(typeof(Func<int, UnityEngine.Object>), methodInfo);
    }
    public static UnityEngine.Object FindObjectFromInstanceID(int aObjectID)
    {
        if (m_FindObjectFromInstanceID == null)
            return null;
        return m_FindObjectFromInstanceID(aObjectID);
    }
}
