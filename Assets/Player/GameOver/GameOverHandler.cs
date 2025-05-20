using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] HealthPool PlayerHealthPool;
    [SerializeField] Light2D SceneLight;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] WaveSpawner spawner;
    [SerializeField] TMP_Text text;
    [SerializeField] MusicPlayer musicPlayer;
    [SerializeField] AudioClipPlayer GameOverScreenPlayer;

    [SerializeField] LeaderboardInputField nameInput;

    async void Awake()
    {
        PlayerHealthPool.OnDepleted += () => StartCoroutine(GameOverAnimationRoutine());
        spawner.OnWaveCompleted += ScoreIncrease;

        nameInput.SetTextWithoutNotify(await Authentication.GetPlayerName());
        nameInput.onSubmit.AddListener(async (name) =>
        {
            await Authentication.ChangePlayerName(name);
        });
    }

    int score = 0;
    private void ScoreIncrease(int wave)
    {
        score = wave;
        text.text = wave.ToString();
    }
    bool UnlockButtons = false;
    public void Restart()
    {
        if (UnlockButtons) SceneLoader.Load("Gameplay");
    }
    public void ToMenu() { if (UnlockButtons) SceneLoader.Load("MainMenu"); }

    IEnumerator GameOverAnimationRoutine()
    {
        var t = 0f;
        SceneLight.transform.position = PlayerHealthPool.transform.position;
        var startInnerRadius = SceneLight.pointLightInnerRadius;
        musicPlayer.FadeOut();
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / 2f;
            t = Mathf.Clamp01(t);
            if (t >= .3)
            {
                var tLight = (t - .3f) / .7f;
                tLight = Mathf.SmoothStep(0, 1, tLight);
                SceneLight.pointLightInnerRadius = startInnerRadius * (1 - tLight) + 2 * tLight;
                SceneLight.pointLightOuterRadius = SceneLight.pointLightInnerRadius + (1 - tLight) * 5 + 1;
            }
            Time.timeScale = 1 - t;
            yield return null;
        }
        while (t > 0)
        {
            t -= Time.unscaledDeltaTime / .3f;
            t = Mathf.Clamp01(t);
            canvasGroup.alpha = 1 - t;
            yield return null;
        }
        yield return null;
        UnlockButtons = true;
        GameOverScreenPlayer.gameObject.SetActive(true);
    }

    async void OnDestroy()
    {
        Time.timeScale = 1f;
        await LeaderboardManager.SetCurrentPlayerScore(score);
    }
}
