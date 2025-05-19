using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardUI : MonoBehaviour
{
    [SerializeField] private Button playerScoresButton;
    [SerializeField] private Button topScoresButton;
    [Min(4)]
    [SerializeField] private int maxEntriesCount = 5;
    [SerializeField] private ScoreboardSingleUI scoreboardSingleUITemplate;
    [SerializeField] private RectTransform content;

    [SerializeField] private LeaderboardInputField nameInput;

    private List<ScoreboardSingleUI> singleUIs = new();

    private void ShowScores(List<Unity.Services.Leaderboards.Models.LeaderboardEntry> entries)
    {
        currentEntries = entries;
        while (singleUIs.Count > 0)
        {
            Destroy(singleUIs[^1].gameObject);
            singleUIs.RemoveAt(singleUIs.Count - 1);
        }
        foreach (var entry in entries)
        {
            var singleUI = Instantiate(scoreboardSingleUITemplate, content.transform);
            singleUI.SetInfo(entry.PlayerName[..^5], (int)entry.Score);
            singleUIs.Add(singleUI);
        }
    }

    List<Unity.Services.Leaderboards.Models.LeaderboardEntry> currentEntries;
    private async void Start()
    {
        ShowScores(await LeaderboardManager.GetScoresAroundPlayer(maxEntriesCount));
        playerScoresButton.onClick.AddListener(async () => ShowScores(await LeaderboardManager.GetScoresAroundPlayer(maxEntriesCount)));
        topScoresButton.onClick.AddListener(async () => ShowScores(await LeaderboardManager.GetBestScores(maxEntriesCount, 0)));
        nameInput.SetTextWithoutNotify(await Authentication.GetPlayerName());
        nameInput.onSubmit.AddListener(async (name) =>
        {
            await Authentication.ChangePlayerName(name);
            for (int i = 0; i < currentEntries.Count; i++)
            {
                var entry = currentEntries[i];
                if (entry.PlayerId == AuthenticationService.Instance.PlayerId)
                    currentEntries[i] = new(entry.PlayerId, name+"#1234", entry.Rank, entry.Score);
            }
            ShowScores(currentEntries);
        });
    }

    private void OnDestroy()
    {
        playerScoresButton.onClick.RemoveAllListeners();
        topScoresButton.onClick.RemoveAllListeners();
        nameInput.onSubmit.RemoveAllListeners();
    }
}
