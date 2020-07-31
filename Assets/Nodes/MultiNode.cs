using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nodes
{
    public class MultiNode : BaseNode
    {
        public new MultiModule Script = ScriptableObject.CreateInstance<MultiModule>();
        public override Type ScriptType => typeof(MultiModule);
        public MultiNode() { }
        public MultiNode(string nodeName, Rect position, string guid, List<string> outputPortIDs) : base(nodeName, position, guid, outputPortIDs) { }
    
        public new static MultiNode Create(string nodeName, Rect position, string guid, List<string> outputPortIDs)
        {
            return new MultiNode(nodeName, position, guid, outputPortIDs);
        }

        protected override void DrawNode(ModuleGraphView graphView)
        {
            var inputPort = graphView.GeneratePort<float>(this, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            inputContainer.Add(inputPort);
            
            //Set the Add Button
            titleContainer.Insert(1, new Button(() =>
            {
                OutputPortIDs.Add(AddMultiRow(graphView).name);
            }){ text = "Add", style = { flexGrow = 0}});
        
            //Add saved port, none otherwise
            foreach (var guid in OutputPortIDs)
            {
                Port port = AddMultiRow(graphView);
                port.name = guid;
            }
        
            graphView.RefreshNode(this);
            graphView.AddElement(this);
        }
        private Port AddMultiRow(ModuleGraphView graphView)
        {
            var temp = graphView.GeneratePort<float>(this, Direction.Output);
            temp.portName = "Output";
            temp.name = Guid.NewGuid().ToString();
            var deleteButton = new Button(() =>
            {
                OutputPortIDs.Remove(temp.name);
                Debug.Log(OutputPortIDs.Count);
                graphView.RemovePort(this, temp);
                graphView.RefreshNode(this);
            }){ text = "-", style = { width = 10}};
            temp.contentContainer.Add(deleteButton);
            outputContainer.Add(temp);
            graphView.RefreshNode(this);
            return temp;
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