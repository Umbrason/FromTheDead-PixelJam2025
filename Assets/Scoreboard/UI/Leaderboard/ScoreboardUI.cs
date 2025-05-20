using System.Collections.Generic;
using System.Linq;
using Unity.Services.Authentication;
using UnityEngine;

public class ScoreboardUI : MonoBehaviour
{
    [Min(4)]
    [SerializeField] private int maxEntriesCount = 5;
    [SerializeField] private ScoreboardSingleUI scoreboardSingleUITemplate;
    [SerializeField] private RectTransform topScores;
    [SerializeField] private RectTransform playerScores;

    private void ShowScores(List<Unity.Services.Leaderboards.Models.LeaderboardEntry> entries, Transform container)
    {
        foreach (var entry in entries)
        {
            var singleUI = Instantiate(scoreboardSingleUITemplate, container);
            singleUI.SetInfo(entry.Rank + 1, entry.PlayerName[..^5], (int)entry.Score);
        }
    }


    private async void Start()
    {
        var bestScores = await LeaderboardManager.GetBestScores(maxEntriesCount, 0);
        ShowScores(bestScores, topScores);
        if (!bestScores.Any(score => score.PlayerId == AuthenticationService.Instance.PlayerId))
        ShowScores(await LeaderboardManager.GetScoresAroundPlayer(maxEntriesCount), playerScores);
    }
}
