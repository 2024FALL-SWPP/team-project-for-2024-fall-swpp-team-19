using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public bool isAlive;
    public ColorEnum color;
    public ColorEnum target;
    public bool isPlayingMinigame;
    public bool knowTarget;
    public int completedMinigames;
    public int minigamesForClue;

    public PlayerData()
    {
        Reset();
    }

    public void UpdateField(string field, object value)
    {
        if (value == null)
        {
            Debug.LogWarning($"Cannot update field '{field}' with a null value.");
            return;
        }

        switch (field.ToLower())
        {
            case "isalive":
                isAlive = (bool)value;
                break;
            case "color":
                color = (ColorEnum)value;
                break;
            case "target":
                target = (ColorEnum)value;
                break;
            case "isplayingminigame":
                isPlayingMinigame = (bool)value;
                break;
            case "knowtarget":
                knowTarget = (bool)value;
                break;
            case "completedminigames":
                completedMinigames = Convert.ToInt32(value);
                break;
            case "minigamesforclue":
                minigamesForClue = Convert.ToInt32(value);
                break;
            default:
                Debug.LogWarning($"Field '{field}' not found in PlayerData.");
                break;
        }
    }

    public object GetField(string field)
    {
        switch (field.ToLower())
        {
            case "isalive":
                return isAlive;
            case "color":
                return color;
            case "target":
                return target;
            case "isplayingminigame":
                return isPlayingMinigame;
            case "knowtarget":
                return knowTarget;
            case "completedminigames":
                return completedMinigames;
            case "minigamesforclue":
                return minigamesForClue;
            default:
                Debug.LogWarning($"Field '{field}' not found in PlayerData.");
                return null;
        }
    }

    public void Reset()
    {
        isAlive = true;
        color = ColorEnum.Undefined;
        target = ColorEnum.Undefined;
        isPlayingMinigame = false;
        knowTarget = false;
        completedMinigames = 0;
        minigamesForClue = 0;
    }

    public override string ToString()
    {
        return $"PlayerData [isAlive: {isAlive}, color: {color}, target: {target}, isPlayingMinigame: {isPlayingMinigame}, " +
               $"knowTarget: {knowTarget}, completedMinigames: {completedMinigames}, minigamesForClue: {minigamesForClue}]";
    }
}
