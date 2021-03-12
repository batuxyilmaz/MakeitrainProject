using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    
    [Header("DEBUG")]
    public int currentLevel;
    public int levelCount;
    public int levelToBeLoaded;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentLevel = PlayerPrefs.GetInt("Level");
        levelCount = GameManager.instance.levels.Length;

        LoadLevel(currentLevel);
    }

    public void NextLevel()
    {
        currentLevel = PlayerPrefs.GetInt("Level");
        currentLevel++;
        PlayerPrefs.SetInt("Level", currentLevel);
    }

    public void RestartLevel()
    {
        LoadLevel(currentLevel);
    }

    public void LoadLevel(int level)
    {
        //   ControlLevelNumber(level);
        GameObject[] previousLevels = GameObject.FindGameObjectsWithTag("Level");
        foreach(GameObject go in previousLevels)
        {
            Destroy(go);
        }
        DefaultValues();
    }

    public int ControlLevelNumber(int level)
    {
        if (level >= levelCount)
        {
            levelToBeLoaded = Random.Range(0, levelCount);
        }
        else
        {
            levelToBeLoaded = level;
        }
        return levelToBeLoaded;
    }

    private void DefaultValues()
    {
        GameManager.instance.DefaultValues();
    }
}
