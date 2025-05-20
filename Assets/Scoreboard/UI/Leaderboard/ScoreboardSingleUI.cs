using TMPro;
using UnityEngine;

public class ScoreboardSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TMP_FontAsset[] Fonts;

    void OnEnable()
    {
        playerNameText.font = scoreText.font = Fonts[Random.Range(0, Fonts.Length)];
    }

    public void SetInfo(int spot, string playerName, int score)
    {
        playerNameText.text = $"{spot}. {playerName}";
        scoreText.text = $"{score}";
    }
}
