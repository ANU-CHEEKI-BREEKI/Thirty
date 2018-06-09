using System;

[Serializable]
public class Settings : ISavable, IResetable
{
    public GraphixSettings graphixSettings;
    public AudioSettings audioSettings;
    public CommonSettings commonSettings;

    public Settings()
    {
        graphixSettings = new GraphixSettings();
        audioSettings = new AudioSettings();
        commonSettings = new CommonSettings();
    }

    public void Save()
    {
        graphixSettings.Save();
        audioSettings.Save();
        commonSettings.Save();
    }

    public void Load()
    {
        graphixSettings.Load();
        audioSettings.Load();
        commonSettings.Load();
    }

    void IResetable.Reset()
    {
        graphixSettings.Reset();
        audioSettings.Reset();
        commonSettings.Reset();
    }
}
