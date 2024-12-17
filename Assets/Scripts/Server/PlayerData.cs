using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public bool isAlive;
    public ColorEnum color;

    public PlayerData()
    {
        isAlive = true;
        color = ColorEnum.Undefined;
    }

    public void UpdateField(string field, object value)
    {
        switch (field.ToLower())
        {
            case "isalive":
                isAlive = (bool)value;
                break;
            case "color":
                color = (ColorEnum)value;
                break;
            default:
                Debug.LogWarning($"Field '{field}' not found in PlayerData.");
                break;
        }
    }
}
