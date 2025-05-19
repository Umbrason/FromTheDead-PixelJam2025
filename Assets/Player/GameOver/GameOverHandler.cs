using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] HealthPool PlayerHealthPool;
    [SerializeField] Light2D SceneLight;
    void Awake()
    {
        PlayerHealthPool.OnDepleted += () => StartCoroutine(GameOverAnimationRoutine());
    }
    IEnumerator GameOverAnimationRoutine()
    {
        var t = 0f;
        SceneLight.transform.position = PlayerHealthPool.transform.position;
        var startInnerRadius = SceneLight.pointLightInnerRadius;
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
        yield return null;
    }
}
