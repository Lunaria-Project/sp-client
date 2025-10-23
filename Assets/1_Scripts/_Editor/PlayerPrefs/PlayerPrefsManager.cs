using System;
using System.Collections.Generic;
using UnityEngine;

public enum PrefKey
{
    ColliderVisualize,
}

public static class PlayerPrefsManager
{
    private const string Prefix = "SP.";

    private static readonly Dictionary<PrefKey, string> KeyMap = new()
    {
        { PrefKey.ColliderVisualize, "ColliderVisualize" },
    };

    private static readonly Dictionary<PrefKey, int> DefaultMap = new()
    {
        { PrefKey.ColliderVisualize, 0 },
    };

    // -------- int --------
    public static void SetInt(PrefKey key, int value)
    {
        if (!TryGetKey(key, out var stringKey)) return;
        PlayerPrefs.SetInt(stringKey, value);
        PlayerPrefs.Save();
    }

    public static int GetInt(PrefKey key)
    {
        var defaultValue = GetDefault(key);
        if (!TryGetKey(key, out var stringKey)) return defaultValue;
        return PlayerPrefs.GetInt(stringKey, defaultValue);
    }

    // -------- bool --------
    public static void SetBool(PrefKey key, bool value)
    {
        SetInt(key, value ? 1 : 0);
    }

    public static bool GetBool(PrefKey key)
    {
        return GetInt(key) != 0;
    }

    // -------- enum --------
    public static void SetEnum<TEnum>(PrefKey key, TEnum value) where TEnum : struct, Enum
    {
        var intValue = Convert.ToInt32(value);
        SetInt(key, intValue);
    }

    public static TEnum GetEnum<TEnum>(PrefKey key) where TEnum : struct, Enum
    {
        return (TEnum)Enum.ToObject(typeof(TEnum), GetInt(key));
    }

    // -------- common --------
    public static bool Has(PrefKey key)
    {
        if (!TryGetKey(key, out var stringKey)) return false;
        return PlayerPrefs.HasKey(stringKey);
    }

    public static void Delete(PrefKey key)
    {
        if (!TryGetKey(key, out var stringKey)) return;
        PlayerPrefs.DeleteKey(stringKey);
        PlayerPrefs.Save();
    }

    // -------- private --------
    private static bool TryGetKey(PrefKey key, out string stringKey)
    {
        if (!KeyMap.TryGetValue(key, out var suffix))
        {
            LogManager.LogError($"PlayerPrefs key not found: {key}");
            stringKey = string.Empty;
            return false;
        }

        stringKey = $"{Prefix}{suffix}";
        return true;
    }

    private static int GetDefault(PrefKey key)
    {
        return DefaultMap.TryGetValue(key, out var value) ? value : 0;
    }
}