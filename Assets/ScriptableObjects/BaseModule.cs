using UnityEngine;

public class BaseModule : ScriptableObject, IModule
{
    public string GUID;
    private ModuleInfo previousInfo;
    public int Delay;

    public void StartAction(ModuleInfo info)
    {
        previousInfo = info;
        Run();
    }

    public virtual void Run()
    {
    }
}
