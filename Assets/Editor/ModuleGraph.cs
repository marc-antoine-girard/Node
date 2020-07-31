using System;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class ModuleGraph : EditorWindow
{
    public static ModuleGraphView graphView;
    public readonly static string defaultCacheName = "cache";
    public string LoadedFileName = "";
    public static string filename = "New Narrative";
    public static readonly string DefaultName = "Chapter Graph";
    [MenuItem("Graph/Module Graph")]
    public static void OpenDialogueGraphWindow()
    {
        GetWindow<ModuleGraph>();
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
        
        var cached = Resources.Load<ActionContainer>(defaultCacheName);
        if (cached != null)
        {
            LoadGraph(defaultCacheName);
            AssetDatabase.DeleteAsset($"Assets/Resources/{defaultCacheName}.asset");
            titleContent = new GUIContent($"{DefaultName} *");
            graphView.SetDirty();
        }
    }

    private void OnDisable()
    {
        if (graphView.IsDirty)
        {
            SaveGraph("cache");
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
        
        TextField fileNameTextField = new TextField("");
        ObjectField objectField = new ObjectField();
        ToolbarMenu groupMenu = new ToolbarMenu {text = "Groups"};
        Toggle gridCheckbox = new Toggle();
        Button saveButton = new Button {text = "Save"};
        Button newButton = new Button {text = "New"};
        ToolbarMenu fileMenu = new ToolbarMenu {text = "File"};
        
        #region Groups
        //Should iterate through ActionModuleGroup
        fileMenu.menu.AppendAction("new", action =>
        {
            objectField.value = null;
            graphView.CreateEmptyNewGraph();
        });
        fileMenu.menu.AppendSeparator();

        fileMenu.menu.AppendAction("Save", action => { groupMenu.text = action.name; }, 
            a => DropdownMenuAction.Status.Normal, default);

        #endregion
        
        
        #region New Button
        newButton.clicked += () =>
        {
            objectField.value = null;
            graphView.CreateEmptyNewGraph();
        };
        #endregion
        
        #region Object Loader
        objectField.objectType = typeof(ActionContainer);
        objectField.RegisterCallback<ChangeEvent<Object>>(evt =>
        {
            var saveUtility = GraphSaveUtility.GetInstance(graphView);

            //if dirty
            if (!graphView.IsDirty)
            {
                //and has a value
                if (evt.newValue != null)
                {
                    //clears and loadGraph
                    saveUtility.LoadGraph((ActionContainer)evt.newValue);
                    string loadedName = ((ActionContainer) evt.newValue).ContainerName;
                    LoadedFileName = loadedName;
                    fileNameTextField.SetValueWithoutNotify(loadedName);
                    return;
                }
                graphView.CreateEmptyNewGraph();
                return;
            }
            //Graph is dirty
            int option = EditorUtility.DisplayDialogComplex("Unsaved Changes",
                "Do you want to save the changes you made before?","Save","Cancel","Don't Save");
            
            switch (option)
            {
                case 1: //Cancel
                    objectField.SetValueWithoutNotify(evt.previousValue);
                    break;
                case 0: // Save
                    if (evt.newValue == null)
                        graphView.CreateEmptyNewGraph();
                    else
                        graphView.SetDirty (!(saveUtility.SaveGraph(filename) != null && 
                                            saveUtility.LoadGraph((ActionContainer)evt.newValue)));
                    break;
                case 2: // Don't Save.
                    graphView.SetDirty (!saveUtility.LoadGraph((ActionContainer)evt.newValue));
                    break;
                default:
                    Debug.LogError("Unrecognized option.", this);
                    break;
            }
            
        });
        #endregion
        
        #region Filename
        fileNameTextField.style.backgroundColor = new StyleColor(Color.black);
        fileNameTextField.style.minWidth = 50;
        fileNameTextField.SetValueWithoutNotify(filename);
        fileNameTextField.RegisterCallback<ChangeEvent<string>>(evt => { filename = evt.newValue;});
        #endregion
        
        #region Save Buttons
        saveButton.clicked += () =>
        {
            string path = EditorUtility.SaveFilePanelInProject("Save", "Assets", "asset", "aaaaa");
            var actionContainer = SaveGraph(path, true);
            if (actionContainer) objectField.SetValueWithoutNotify(actionContainer);
        };
        #endregion
        
        #region Grid Checkbox
        gridCheckbox.RegisterCallback((ChangeEvent<bool> e) => { graphView.ToggleGrid(e.newValue);});
        // activateGrid.RegisterCallback<ChangeEvent<bool>>(e => { graphView.ToggleGrid(e.newValue);});
        
        gridCheckbox.value = true;
        graphView.ToggleGrid(true);
        #endregion

        #region Groups
        //Should iterate through ActionModuleGroup
        groupMenu.menu.AppendAction("Groups", action => { groupMenu.text = action.name; }, 
            a => DropdownMenuAction.Status.Disabled, default);
        groupMenu.menu.AppendAction("test", action => { groupMenu.text = action.name; }, 
            a => DropdownMenuAction.Status.Checked, default);
        #endregion
        
        toolbar.Add(fileMenu);
        // toolbar.Add(newButton);
        toolbar.Add(objectField);
        // toolbar.Add(fileNameTextField);
        toolbar.Add(saveButton);
        toolbar.Add(gridCheckbox);
        toolbar.Add(groupMenu);
        
        toolbar.MarkDirtyRepaint();
        //Adds toolbar to window
        rootVisualElement.Add(toolbar);
    }

    private bool LoadGraph(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name", "Please enter a valid file name.", "OK");
            return false;
        }

        var saveUtility = GraphSaveUtility.GetInstance(graphView);
        
        // if loads cache file, shouldn't set the graphview to not dirty.
        if(saveUtility.LoadGraph(fileName) && fileName != defaultCacheName)
            graphView.SetDirty(false);

        return true;
    }

    public ActionContainer SaveGraph(string fileName, bool usePath = false)
    {
        var saveUtility = GraphSaveUtility.GetInstance(graphView);
        ActionContainer saved = null;
        if (graphView.IsDirty || fileName != LoadedFileName)
        {
            saved = saveUtility.SaveGraph(fileName, usePath);
            if (saved) graphView.SetDirty(false);
        }
        return saved;
    }
    
    
    

}
