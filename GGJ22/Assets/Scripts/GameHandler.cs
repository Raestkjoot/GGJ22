using UnityEngine;

public class GameInformation
{
    public int currentLevel = 0;
    public int maxUnlockedLevel = 0;
}

public class GameHandler
{
    public GameInformation gameInformation;

    public void Initialize(Bootstrap bootstrap)
    {
        gameInformation = new GameInformation
        {
            currentLevel = PlayerPrefs.GetInt("CurrentLevel", 0),
            maxUnlockedLevel = PlayerPrefs.GetInt("MaxUnlockedLevel", 0)
        };

        bootstrap.OnDestroy += Bootstrap_OnDestroy;
        bootstrap.OnApplicationQuit += Bootstrap_OnApplicationQuit;
    }

    private void Bootstrap_OnDestroy()
    {
        SaveSettings();
    }

    private void Bootstrap_OnApplicationQuit()
    {
        SaveSettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("CurrentLevel", gameInformation.currentLevel);
        PlayerPrefs.SetInt("MaxUnlockedLevel", gameInformation.maxUnlockedLevel);

        PlayerPrefs.Save();
    }
}
