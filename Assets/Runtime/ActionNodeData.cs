using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActionNodeData
{
    public string GUID;
    public string title;
    public Rect Position;
    public NodeType NodeType;
    public List<string> OutputPortIDs;
    public Type Type;
}
