using Mirror;
using System;

[Serializable]
public class PlayerData
{
    public string playerName;
    public int score;

    public PlayerData()
    {
        playerName = "";
        score = 0;
    }

    public void UpdateField(string field, object value)
    {
        switch (field.ToLower())
        {
            case "playername":
                playerName = value as string;
                break;
            case "score":
                score = Convert.ToInt32(value);
                break;
        }
    }
}
