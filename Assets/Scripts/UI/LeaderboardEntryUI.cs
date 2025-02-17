using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class LeaderboardEntryUI : MonoBehaviour
{
    //[SerializeField] private Color playerEntryColor = new Color(255f, 255f, 0f);

    [SerializeField] private TextMeshProUGUI rank, username, score;

    public void LoadFromLeaderboardEntry(LeaderboardEntry entry)
    {
        // ranks are indexes
        rank.text = (entry.Rank + 1).ToString();

        // playernames include a name and then a number string for differentiation (name#000)
        username.text = entry.PlayerName.Substring(0, entry.PlayerName.LastIndexOf("#"));

        score.text = Math.Floor(entry.Score).ToString();
    }

    public void SetAsPlayerEntry()
    {
        SetTextColor(SaveManager.save.playerColor);
    }

    public void SetTextColor(Color c)
    {
        rank.color = c;
        username.color = c;
        score.color = c;
    }
}
