﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActionNodeData
{
    public Type NodeType;
    public string GUID;
    public Rect Position;
    public List<string> OutputPortIDs;
    public string SerializedScript;
}
