using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerPref : MonoBehaviour
{
    public static void SetInts(string key, List<int> collection)
    {
        PlayerPrefs.SetInt(key + ".Count", collection.Count);

        for(int i = 0; i < collection.Count; i++)
        {
            PlayerPrefs.SetInt(key + "[" + i.ToString() + "]", collection[i]);
        }
    }

    public static List<int> GetInts(string key)
    {
        int count = PlayerPrefs.GetInt(key + ".Count");
        List<int> array = new List<int>();

        for(int i = 0; i < count; i++)
        {
            array.Add(PlayerPrefs.GetInt(key + "[" + i.ToString() + "]"));
        }

        return array;
    }

    public static void SetFloats(string key, List<float> collection)
    {
        PlayerPrefs.SetInt(key + ".Count", collection.Count);

        for (int i = 0; i < collection.Count; i++)
        {
            PlayerPrefs.SetFloat(key + "[" + i.ToString() + "]", collection[i]);
        }
    }

    public static List<float> GetFloats(string key)
    {
        int count = PlayerPrefs.GetInt(key + ".Count");
        List<float> array = new List<float>();

        for (int i = 0; i < count; i++)
        {
            array.Add(PlayerPrefs.GetFloat(key + "[" + i.ToString() + "]"));
        }

        return array;
    }

    public static void SetStrings(string key, List<string> collection)
    {
        PlayerPrefs.SetInt(key + ".Count", collection.Count);

        for (int i = 0; i < collection.Count; i++)
        {
            PlayerPrefs.SetString(key + "[" + i.ToString() + "]", collection[i]);
        }
    }

    public static List<string> GetStrings(string key)
    {
        int count = PlayerPrefs.GetInt(key + ".Count");
        List<string> array = new List<string>();

        for (int i = 0; i < count; i++)
        {
            array.Add(PlayerPrefs.GetString(key + "[" + i.ToString() + "]"));
        }

        return array;
    }
}
