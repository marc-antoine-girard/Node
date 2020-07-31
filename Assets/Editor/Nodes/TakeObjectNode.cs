using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Nodes
{
    public class TakeObjectNode : BaseNode
    {
        public new TakeObjectModule Script = ScriptableObject.CreateInstance<TakeObjectModule>();
        public override Type ScriptType => typeof(TakeObjectModule);

        public TakeObjectNode() { }

        public TakeObjectNode(string nodeName, Rect position, string guid, List<string> outputPortIDs) : base(nodeName, position, guid, outputPortIDs) {}
    
        public new static TakeObjectNode Create(string nodeName, Rect position, string guid, List<string> outputPortIDs)
        {
            return new TakeObjectNode(nodeName, position, guid, outputPortIDs);
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

            ObjectField objectField = new ObjectField();
            objectField.objectType = typeof(GameObject);
            objectField.value = Script.GameObject;
            objectField.RegisterCallback<ChangeEvent<Object>>(evt => Script.GameObject = evt.newValue);
            extensionContainer.Add(objectField);

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