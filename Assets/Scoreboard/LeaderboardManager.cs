using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Exceptions;

using LeaderboardEntry = Unity.Services.Leaderboards.Models.LeaderboardEntry;
public static class LeaderboardManager
{
#if UNITY_EDITOR
    private const string LEADERBOARD_ID = "EditorLeaderboard";
#else
    private const string LEADERBOARD_ID = "MainScoreboard";
#endif

    public static async Task SetCurrentPlayerScore(int score)
    {
        try
        {
            await Authentication.Initialize();
            var playerEntry = await LeaderboardsService.Instance
            .AddPlayerScoreAsync(LEADERBOARD_ID, score);
        }
        catch (LeaderboardsException e)
        {
            Debug.LogError("Exception while setting new score: \n" + e);
        }
    }

    public static async Task<List<LeaderboardEntry>> GetScoresAroundPlayer(int rangeLimit)
    {
        try
        {
            await Authentication.Initialize();
            var scores = await LeaderboardsService.Instance.GetPlayerRangeAsync(
                                LEADERBOARD_ID,
                                new GetPlayerRangeOptions { RangeLimit = rangeLimit }
                                );
            return scores.Results;
        }
        catch (LeaderboardsException e)
        {
            Debug.LogError("Error while getting scores around player: \n" + e);
            return new List<LeaderboardEntry>();
        }
    }

    public static async Task<List<LeaderboardEntry>> GetBestScores(int rangeLimit, int offset)
    {
        try
        {
            await Authentication.Initialize();
            var scores = await LeaderboardsService.Instance.GetScoresAsync(
                                LEADERBOARD_ID,
                                new GetScoresOptions { Limit = rangeLimit, Offset = offset }
                                );
            return scores.Results;
        }
        catch (LeaderboardsException e)
        {
            Debug.LogError("Error while getting best scores: \n" + e);
            return new List<LeaderboardEntry>();
        }
    }
}
