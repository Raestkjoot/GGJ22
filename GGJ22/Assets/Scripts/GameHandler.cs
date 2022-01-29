using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInformation
{
    public int currentLevel = 0;
    public int maxUnlockedLevel = 0;
}

public struct GameLevelScene
{
    public int buildIndex;
    public string name;
}

public class GameHandler
{
    public void Initialize(Bootstrap bootstrap)
    {
        gameInformation = new GameInformation
        {
            currentLevel = PlayerPrefs.GetInt("CurrentLevel", 0),
            maxUnlockedLevel = PlayerPrefs.GetInt("MaxUnlockedLevel", 0)
        };

        int sceneCount = SceneManager.sceneCountInBuildSettings;
        _gameLevelScenes = new List<GameLevelScene>(sceneCount);

        for (int i = 0; i < sceneCount; i++)
        {
            string sceneName = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            if (!sceneName.StartsWith("Level"))
                continue;

            GameLevelScene levelScene = new GameLevelScene();
            levelScene.buildIndex = i;
            levelScene.name = sceneName;

            _gameLevelScenes.Add(levelScene);
        }

        // Sort Levels based on name in alphabetical order
        _gameLevelScenes.Sort((x, y) => x.name.CompareTo(y.name));

        // Ensure CurrentLevel && MaxUnlockedLevel is between 0 -> sceneCount
        gameInformation.currentLevel = Math.Min(gameInformation.currentLevel, sceneCount);
        gameInformation.currentLevel = Math.Max(gameInformation.currentLevel, 0);
        gameInformation.maxUnlockedLevel = Math.Min(gameInformation.maxUnlockedLevel, sceneCount);
        gameInformation.maxUnlockedLevel = Math.Max(gameInformation.maxUnlockedLevel, 0);

        int currentLevel = gameInformation.currentLevel;
        LoadLevel(currentLevel);

        bootstrap.OnDestroy += Bootstrap_OnDestroy;
        bootstrap.OnApplicationQuit += Bootstrap_OnApplicationQuit;
    }

    public bool IsLevelValid(int level)
    {
        return level >= 0 && level < _gameLevelScenes.Count;
    }
    public bool LoadLevel(int level)
    {
        if (!IsLevelValid(level))
            return false;

        GameLevelScene scene = _gameLevelScenes[level];
        SceneManager.LoadSceneAsync(scene.buildIndex, LoadSceneMode.Additive);
        return true;
    }
    public bool UnloadLevel(int level)
    {
        if (!IsLevelValid(level))
            return false;

        GameLevelScene scene = _gameLevelScenes[level];
        SceneManager.UnloadSceneAsync(scene.buildIndex);
        return true;
    }
    public bool LoadNextMap()
    {
        int currentLevel = gameInformation.currentLevel;
        int nextLevel = gameInformation.currentLevel + 1;

        bool isValidMaps = IsLevelValid(currentLevel) && IsLevelValid(nextLevel);
        if (isValidMaps)
        {
            UnloadLevel(currentLevel);
            LoadLevel(nextLevel);

            gameInformation.currentLevel += 1;
            gameInformation.maxUnlockedLevel = Math.Max(gameInformation.currentLevel, gameInformation.maxUnlockedLevel);
            SaveSettings();
        }

        return isValidMaps;
    }
    public bool LoadPrevMap()
    {
        int currentLevel = gameInformation.currentLevel;
        int prevLevel = gameInformation.currentLevel - 1;

        bool isValidMaps = IsLevelValid(currentLevel) && IsLevelValid(prevLevel);
        if (isValidMaps)
        {
            UnloadLevel(currentLevel);
            LoadLevel(prevLevel);

            gameInformation.currentLevel -= 1;
            SaveSettings();
        }

        return isValidMaps;
    }
    public void ResetMap()
    {
        int currentLevel = gameInformation.currentLevel;

        if (IsLevelValid(currentLevel))
        {
            UnloadLevel(currentLevel);
            LoadLevel(currentLevel);
        }
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("CurrentLevel", gameInformation.currentLevel);
        PlayerPrefs.SetInt("MaxUnlockedLevel", gameInformation.maxUnlockedLevel);

        PlayerPrefs.Save();
    }

    private void Bootstrap_OnDestroy()
    {
        SaveSettings();
    }
    private void Bootstrap_OnApplicationQuit()
    {
        SaveSettings();
    }

    public GameInformation gameInformation;
    private List<GameLevelScene> _gameLevelScenes;
}
