using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ModuleGraph : EditorWindow
{
    public static ModuleGraphView graphView;
    public static string filename = "New Narrative";

    public static bool contruct = false;
    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<ModuleGraph>();
        window.titleContent = new GUIContent("Action");
    }

    private void OnEnable()
    {
        if (!contruct)
        {
            ConstructGraphView();
        }
        else
        {
            rootVisualElement.Add(graphView);
        }
        GenerateToolbar();
    }
    
    private void ConstructGraphView()
    {
        graphView = new ModuleGraphView
        {
            name = "Module Graph"
        };
        contruct = true;
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        #region Filename
        var fileNameTextField = new TextField("");
        fileNameTextField.style.backgroundColor = new StyleColor(Color.black);
        fileNameTextField.SetValueWithoutNotify(filename);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterCallback<ChangeEvent<string>>(evt => { filename = evt.newValue;});
        toolbar.Add(fileNameTextField);
        #endregion
        
        #region Save and load Buttons
        toolbar.Add(new Button(() => RequestDataOperation(true)){text = "Save"});
        toolbar.Add(new Button(() => RequestDataOperation(false)){text = "Load"});
        #endregion
        
        #region CreateNode
        var nodeCreateButton = new Button(() =>
        {
            graphView.CreateNode("Action Node");
        });
        nodeCreateButton.text = "Create Node";
        toolbar.Add(nodeCreateButton);
        #endregion
        
        #region Grid Checkbox
        var activateGrid = new Toggle();
        activateGrid.MarkDirtyRepaint();
        activateGrid.RegisterCallback((ChangeEvent<bool> e) => { graphView.ToggleGrid(e.newValue);});
        // activateGrid.RegisterCallback<ChangeEvent<bool>>(e => { graphView.ToggleGrid(e.newValue);});
        
        activateGrid.value = true;
        graphView.ToggleGrid(true);
        toolbar.Add(activateGrid);
        #endregion
        
        //Adds toolbar to window
        rootVisualElement.Add(toolbar);
    }

    private void RequestDataOperation(bool save)
    {
        if (string.IsNullOrEmpty(filename))
        {
            EditorUtility.DisplayDialog("Invalid file name", "Please enter a valid file name.", "OK");
            return;
        }

        var saveUtility = GraphSaveUtility.GetInstance(graphView);
        if(save)
            saveUtility.SaveGraph(filename);
        else
            saveUtility.LoadGraph(filename);
    }
    

}
