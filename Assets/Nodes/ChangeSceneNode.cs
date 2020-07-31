using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nodes
{
    public class ChangeSceneNode : BaseNode
    {
        public new ChangeSceneModule Script = ScriptableObject.CreateInstance<ChangeSceneModule>();
        public override Type ScriptType => typeof(ChangeSceneModule);

        public ChangeSceneNode() {}

        public ChangeSceneNode(string nodeName, Rect position, string guid, List<string> outputPortIDs) : base(nodeName, position, guid, outputPortIDs) { }

        public new static ChangeSceneNode Create(string nodeName, Rect position, string guid, List<string> OutputPortIDs)
        {
            return new ChangeSceneNode(nodeName, position, guid, OutputPortIDs);
        }

        protected override void DrawNode(ModuleGraphView graphView)
        {
            var inputPort = graphView.GeneratePort<float>(this, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            inputContainer.Add(inputPort);
            AddToClassList("action");

            var outputPort = graphView.GeneratePort<float>(this, Direction.Output);
            outputPort.portName = "Output";
            outputContainer.Add(outputPort);

            IntegerField intField = new IntegerField("Scene Index", 2);
            intField.value = Script.SceneIndex;
            intField.RegisterValueChangedCallback(evt =>
            {
                var temp = evt.newValue < 0 ? 0 : evt.newValue > 99 ? 99 : evt.newValue;
                intField.SetValueWithoutNotify(temp);
                Script.SceneIndex = temp;
                graphView.SetDirty();
            });
            
            extensionContainer.Add(intField);

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
