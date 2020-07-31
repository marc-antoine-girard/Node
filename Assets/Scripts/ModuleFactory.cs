using System;
using Nodes;
using UnityEngine;

public class ModuleFactory
{
    /// <summary>
    /// Creates a module  
    /// </summary>
    /// <param name="type">The type of the module to create</param>
    /// <param name="json">The serialized Script in JSON Format</param>
    /// <returns>BaseModule</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static BaseModule CreateModule(Type type, string json)
    {
        if (type == null) return null;
        
        BaseModule module = null;
        
        if (type == typeof(ChangeSceneModule))
        {
            module = ScriptableObject.CreateInstance<ChangeSceneModule>();
        }
        else if (type == typeof(ConditionalModule))
        {
            module = ScriptableObject.CreateInstance<ConditionalModule>();
        }
        else if (type == typeof(ExitModule))
        {
            module = ScriptableObject.CreateInstance<ExitModule>();
        }
        else if (type == typeof(MultiModule))
        {
            module = ScriptableObject.CreateInstance<MultiModule>();
        }
        else if (type == typeof(RandomModule))
        {
            module = ScriptableObject.CreateInstance<RandomModule>();
        }
        else if (type == typeof(StartModule))
        {
            module = ScriptableObject.CreateInstance<StartModule>();
        }
        else if (type == typeof(TakeObjectModule))
        {
            module = ScriptableObject.CreateInstance<TakeObjectModule>();
        }
        else
        {
            throw new NotImplementedException($"Module Type {type} not handled in ModuleFactory");
        }
        
        if(!string.IsNullOrEmpty(json))
            JsonUtility.FromJsonOverwrite(json, module);

        return module;
    }
}
