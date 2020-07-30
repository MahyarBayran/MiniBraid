using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// A container for the configuration data
/// </summary>
public class ConfigurationData
{
    #region Fields

    const string ConfigurationDataFileName = "ConfigurationData.csv";

    // configuration data
    int maxEnemiesPerSpawnerEasy = 1;
    int maxEnemiesPerSpawnerMedium = 2;
    int maxEnemiesPerSpawnerHard = 3;

    float spawnIntervalEasy = 10.0f;
    float spawnIntervalMedium = 7.0f;
    float spawnIntervalHard = 5.0f;

    #endregion

    #region Properties

    public int MaxEnemiesPerSpawnerEasy
    {
        get { return maxEnemiesPerSpawnerEasy; }
    }

    public int MaxEnemiesPerSpawnerMedium
    {
        get { return maxEnemiesPerSpawnerMedium; }
    }

    public int MaxEnemiesPerSpawnerHard
    {
        get { return maxEnemiesPerSpawnerHard; }
    }

    public float SpawnIntervalEasy
    {
        get { return spawnIntervalEasy; }
    }

    public float SpawnIntervalMedium
    {
        get { return spawnIntervalMedium; }
    }

    public float SpawnIntervalHard
    {
        get { return spawnIntervalHard; }
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor
    /// Reads configuration data from a file. If the file
    /// read fails, the object contains default values for
    /// the configuration data
    /// </summary>
    public ConfigurationData()
    {
        // read and save configuration data from file
        StreamReader input = null;
        try
        {
            // create stream reader object
            input = File.OpenText(Path.Combine(
                Application.streamingAssetsPath, ConfigurationDataFileName));

            // read in names and values
            string names = input.ReadLine();
            string values = input.ReadLine();

            // set configuration data fields
            SetConfigurationDataFields(values);
        }
        catch (Exception e)
        {
        }
        finally
        {
            // always close input file
            if (input != null)
            {
                input.Close();
            }
        }
    }

    #endregion

    /// <summary>
    /// Sets the configuration data fields from the provided
    /// csv string
    /// </summary>
    /// <param name="csvValues">csv string of values</param>
    void SetConfigurationDataFields(string csvValues)
    {
        // the code below assumes we know the order in which the
        // values appear in the string. We could do something more
        // complicated with the names and values, but that's not
        // necessary here
        string[] values = csvValues.Split(',');
        maxEnemiesPerSpawnerEasy = int.Parse(values[0]);
        maxEnemiesPerSpawnerMedium = int.Parse(values[1]);
        maxEnemiesPerSpawnerHard = int.Parse(values[2]);
        spawnIntervalEasy = float.Parse(values[3]);
        spawnIntervalMedium = float.Parse(values[4]);
        spawnIntervalHard = float.Parse(values[5]);
    }
}
