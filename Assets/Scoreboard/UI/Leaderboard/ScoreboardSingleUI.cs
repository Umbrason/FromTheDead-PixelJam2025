using TMPro;
using UnityEngine;

public class ScoreboardSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void SetInfo(string playerName, int score)
    {
        playerNameText.text = playerName;
        scoreText.text = $"<mspace=1em>{score}</mspace>";
    }
}
