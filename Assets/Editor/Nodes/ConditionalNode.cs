﻿using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nodes
{
    public class ConditionalNode : BaseNode
    {
        public new ConditionalModule Script = ScriptableObject.CreateInstance<ConditionalModule>();
        public override Type ScriptType => typeof(ConditionalModule);
        public ConditionalNode() { }
        public ConditionalNode(string nodeName, Rect position, string guid, List<string> outputPortIDs) : base(nodeName, position, guid, outputPortIDs) { }
    
        public new static ConditionalNode Create(string nodeName, Rect position, string guid, List<string> OutputPortIDs)
        {
            return new ConditionalNode(nodeName, position, guid, OutputPortIDs);
        }

        protected override void DrawNode(ModuleGraphView graphView)
        {
            var inputPort = graphView.GeneratePort<float>(this, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            inputContainer.Add(inputPort);

            var Success = graphView.GeneratePort<float>(this, Direction.Output);
            Success.portName = "Success";
            Success.name = "Success";
            outputContainer.Add(Success);

            var Failure = graphView.GeneratePort<float>(this, Direction.Output);
            Failure.portName = "Failure";
            Failure.name = "Failure";
            outputContainer.Add(Failure);

            graphView.RefreshNode(this);
            graphView.AddElement(this);
        }
        
        public override string GetSerializedScript()
        {
            return JsonUtility.ToJson(Script);
        }
        public override void SetSerializedScript(string json)
        {
            JsonUtility.FromJsonOverwrite(json, Script);
        }
    }
}