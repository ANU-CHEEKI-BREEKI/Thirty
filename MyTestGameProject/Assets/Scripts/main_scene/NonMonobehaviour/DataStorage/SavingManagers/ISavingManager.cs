public interface ISavingManager
{
    void SaveData<T>(string name, object data);
    T LoadData<T>(string name);
}
