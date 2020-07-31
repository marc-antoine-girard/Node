using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ActionContainer : ScriptableObject
{
    public string ContainerName;
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public List<ActionNodeData> ActionNodeDatas = new List<ActionNodeData>();
}
