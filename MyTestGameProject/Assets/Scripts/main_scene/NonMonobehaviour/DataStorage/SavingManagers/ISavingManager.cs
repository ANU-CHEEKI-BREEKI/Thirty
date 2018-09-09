using System;

public abstract class ISavingManager
{
    public abstract void SaveData<T>(string name, object data);
    public abstract void LoadData<T>(string name);

    public event Action<string, bool> OnDataSaved;
    public event Action<string, object, bool> OnDataLoaded;

    protected void CallOnDataSaved(string name, bool success)
    {
        if (OnDataSaved != null) OnDataSaved(name, success);
    }

    protected void CallOnDataLoaded(string name, object data, bool success)
    {
        if (OnDataLoaded != null) OnDataLoaded(name, data, success);
    }
}
