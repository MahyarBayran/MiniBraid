using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides access to configuration data
/// </summary>
public static class ConfigurationUtils
{
    static ConfigurationData configurationData;

    static int maxEnemiesPerSpawner = 1;
    static float spawnInterval = 10.0f;

    #region Properties

    /// <summary>
    /// Gets the paddle move units per second
    /// </summary>
    /// <value>paddle move units per second</value>
    public static int MaxEnemiesPerSpawner
    {
        get { return maxEnemiesPerSpawner; }
    }

    public static float SpawnInterval
    {
        get { return spawnInterval; }
    }

    #endregion

    /// <summary>
    /// Initializes the configuration utils
    /// </summary>
    public static void Initialize()
    {
        configurationData = new ConfigurationData();
    }

    public static void SetDifficulty(Difficulty difficulty)
    {
        switch(difficulty)
        {
            case Difficulty.Easy:
                maxEnemiesPerSpawner = configurationData.MaxEnemiesPerSpawnerEasy;
                spawnInterval = configurationData.SpawnIntervalEasy;
                break;

            case Difficulty.Medium:
                maxEnemiesPerSpawner = configurationData.MaxEnemiesPerSpawnerMedium;
                spawnInterval = configurationData.SpawnIntervalMedium;
                break;

            case Difficulty.Hard:
                maxEnemiesPerSpawner = configurationData.MaxEnemiesPerSpawnerHard;
                spawnInterval = configurationData.SpawnIntervalHard;
                break;
        }
    }
}
