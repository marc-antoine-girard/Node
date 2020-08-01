using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class ModuleGraph : EditorWindow
{
    public static ModuleGraphView graphView;
    public const string DefaultCacheName = "cache";
    public const string DefaultName = "Chapter Graph";
    
    [MenuItem("Graph/Module Graph")]
    public static void OpenDialogueGraphWindow()
    {
        GetWindow<ModuleGraph>();
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
        
        var cached = Resources.Load<ActionContainer>(DefaultCacheName);
        if (cached != null)
        {
            graphView.IsCachedFile = true;
            Load(DefaultCacheName);
            AssetDatabase.DeleteAsset($"Assets/Resources/{DefaultCacheName}.asset");
            graphView.SetDirty();
        }
    }

    private void OnDisable()
    {
        if (graphView.IsDirty)
        {
            graphView.IsCachedFile = true;
            Save();
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
        
        ObjectField objectField = new ObjectField();
        Toggle gridCheckbox = new Toggle();
        ToolbarMenu fileMenu = new ToolbarMenu {text = "File", style = { width = 50}};

        #region File Menu
        //Should iterate through ActionModuleGroup
        fileMenu.menu.AppendAction("New Module", action =>
        {
            if (string.IsNullOrEmpty(graphView.LoadedFileName))
            {
                objectField.value = null;
                graphView.CreateEmptyNewGraph();
            }
        });
        
        fileMenu.menu.AppendSeparator();
        fileMenu.menu.AppendAction("Save", action =>
        {
            ActionContainer actionContainer;
            if (string.IsNullOrEmpty(graphView.LoadedFileName))
            {
                actionContainer = Save();
                if (actionContainer) objectField.SetValueWithoutNotify(actionContainer);
                return;
            }
            actionContainer = Save();
            if (actionContainer) objectField.SetValueWithoutNotify(actionContainer);
        });
        fileMenu.menu.AppendAction("Save As", action =>
        {
            var actionContainer = SaveAs();
            if (actionContainer) objectField.SetValueWithoutNotify(actionContainer);
        });
        bool isGridActive = false;
        fileMenu.menu.AppendSeparator();
        fileMenu.menu.AppendAction("Activate Grid", action =>
        {
            isGridActive = !isGridActive;
            graphView.ToggleGrid(!isGridActive);
        }, action =>
        {
            if (isGridActive)
                return DropdownMenuAction.Status.Normal;
            return DropdownMenuAction.Status.Checked;
        });

        #endregion
        
        #region Object Loader
        objectField.objectType = typeof(ActionContainer);
        objectField.RegisterCallback<ChangeEvent<Object>>(evt =>
        {
            if (evt.newValue == null)
            {
                objectField.SetValueWithoutNotify(evt.previousValue);
                return;
            }
            var saveUtility = GraphSaveUtility.GetInstance(graphView);

            //if dirty
            if (!graphView.IsDirty)
            {
                //clears and loadGraph
                saveUtility.LoadGraph((ActionContainer)evt.newValue);
                string loadedName = ((ActionContainer) evt.newValue).ContainerName;
                graphView.LoadedFileName = loadedName;
                graphView.SetName(loadedName);
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
                    graphView.SetDirty (!(saveUtility.SaveGraph(graphView.LoadedFileName) != null && 
                                        saveUtility.LoadGraph((ActionContainer)evt.newValue)));
                    break;
                case 2: // Don't Save.
                    graphView.SetDirty (!saveUtility.LoadGraph((ActionContainer)evt.newValue));
                    break;
            }
            
        });
        #endregion
        
        toolbar.Add(fileMenu);
        toolbar.Add(objectField);
        
        //Adds toolbar to window
        rootVisualElement.Add(toolbar);
    }
    
    private bool Load(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name", "Please enter a valid file name.", "OK");
            return false;
        }

        var saveUtility = GraphSaveUtility.GetInstance(graphView);
        
        // if loads cache file, shouldn't set the graphview to not dirty.
        if(saveUtility.LoadGraph(fileName) && !graphView.IsCachedFile)
            graphView.SetDirty(false);

        return true;
    }

    public ActionContainer SaveAs()
    {
        GraphSaveUtility saveUtility = GraphSaveUtility.GetInstance(graphView);
        ActionContainer saved;
        
        string path = EditorUtility.SaveFilePanelInProject("Save As", "Module", "asset", "Please save file.");

        if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
        {
            return null;
        }
        saved = saveUtility.SaveGraph(path, true);
        
        if (saved)
        {
            //Was able to save
            graphView.LoadedFileName = saved.ContainerName;
            graphView.SetDirty(false);
            graphView.IsCachedFile = false;
        }

        return saved;
    }

    public ActionContainer Save()
    {
        GraphSaveUtility saveUtility = GraphSaveUtility.GetInstance(graphView);
        ActionContainer saved = null;
        
        if (graphView.IsCachedFile) return saveUtility.SaveGraph(DefaultCacheName);
        
        if (graphView.IsDirty)
        {
            if (string.IsNullOrEmpty(graphView.LoadedFileName) || string.IsNullOrWhiteSpace(graphView.LoadedFileName))
                return SaveAs();
            
            saved = saveUtility.SaveGraph(graphView.LoadedFileName);
            if (saved)
            {
                graphView.LoadedFileName = saved.ContainerName;
                graphView.SetDirty(false);
                graphView.IsCachedFile = false;
            }
        }
        return saved;
    }
    
    
    

}
