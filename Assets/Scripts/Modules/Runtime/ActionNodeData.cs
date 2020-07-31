using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActionNodeData
{
    public string NodeType;
    public string GUID;
    public Rect Position;
    public List<string> InputPortIDs;
    public List<string> OutputPortIDs;
    public string SerializedScript;
    public string ScriptType;
}
