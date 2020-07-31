using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ModuleGraph : EditorWindow
{
    public static ModuleGraphView graphView;
    public readonly static string defaultCacheName = "cache";
    public static string filename = "New Narrative";
    public static readonly string DefaultName = "Chapter Graph";
    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<ModuleGraph>();
        // window.titleContent = new GUIContent(DefaultName);
    }

    private void OnEnable()
    {
        var cached = Resources.Load<ActionContainer>(defaultCacheName);

        if (cached)
        {
            ConstructGraphView();
            RequestDataOperation(false, defaultCacheName);
            AssetDatabase.DeleteAsset($"Assets/Resources/{defaultCacheName}.asset");
            titleContent = new GUIContent($"{DefaultName} *");
            // graphView.IsDirty = true;
            graphView.SetDirty();
        }
        else { ConstructGraphView(); }
        
        GenerateToolbar();
    }

    private void OnDisable()
    {
        if (graphView.IsDirty)
        {
            RequestDataOperation(true, "cache");
        }
        rootVisualElement.Remove(graphView);
    }

    private void ConstructGraphView()
    {
        graphView = new ModuleGraphView(this);
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
        fileNameTextField.RegisterCallback<ChangeEvent<string>>(evt => { filename = evt.newValue;});
        toolbar.Add(fileNameTextField);
        #endregion
        
        #region Save and load Buttons
        toolbar.Add(new Button(() => RequestDataOperation(true, filename)){text = "Save"});
        toolbar.Add(new Button(() => RequestDataOperation(false, filename)){text = "Load"});
        #endregion
        
        #region Grid Checkbox
        var activateGrid = new Toggle();
        activateGrid.RegisterCallback((ChangeEvent<bool> e) => { graphView.ToggleGrid(e.newValue);});
        // activateGrid.RegisterCallback<ChangeEvent<bool>>(e => { graphView.ToggleGrid(e.newValue);});
        
        activateGrid.value = true;
        graphView.ToggleGrid(true);
        toolbar.Add(activateGrid);
        #endregion

        //Should iterate through ActionModuleGroup
        var toolbarMenu = new ToolbarMenu {text = "Groups"};
        toolbarMenu.menu.AppendAction("Groups", action => { toolbarMenu.text = action.name; }, 
            a => DropdownMenuAction.Status.Disabled, default);
        toolbarMenu.menu.AppendAction("test", action => { toolbarMenu.text = action.name; }, 
            a => DropdownMenuAction.Status.Checked, default);
        toolbar.Add(toolbarMenu);
        
        toolbar.MarkDirtyRepaint();
        //Adds toolbar to window
        rootVisualElement.Add(toolbar);
    }

    private void RequestDataOperation(bool save, string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name", "Please enter a valid file name.", "OK");
            return;
        }

        var saveUtility = GraphSaveUtility.GetInstance(graphView);
        if (save && graphView.IsDirty)
        {
            if (saveUtility.SaveGraph(fileName))
                graphView.SetDirty(false);
        }
        else
        {
            if(saveUtility.LoadGraph(fileName) && fileName != defaultCacheName)
                graphView.SetDirty(false);
        }
    }
    
    
    

}
