using System;
using System.Collections.Generic;
using UnityEngine;

public class ModuleController : MonoBehaviour
{
    /// <summary>
    /// Contains every Module associated with their unique ID.
    /// </summary>
    /// <remarks>
    /// String = Current Node' GUID
    /// List<IModule> OutputModules
    /// </remarks>
    public Dictionary<string, List<IModule>> Modules;
    public ActionContainer Container;

    private void Awake()
    {
        // Populate Modules using container
    }

    private void Start()
    {
        //Find Start Node / Nodes
        //Pass itself
        //Run
    }
}
