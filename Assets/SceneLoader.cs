using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] CanvasGroup Fade;
    static SceneLoader Instance;
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private static bool loading;
    public static void Load(string scene) => Instance.StartCoroutine(Instance.LoadRoutine(scene));
    public IEnumerator LoadRoutine(string sceneName)
    {
        if (loading) yield break;
        loading = true;
        var curScene = SceneManager.GetActiveScene();
        var t = 0f;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / .5f;
            t = Mathf.Clamp01(t);
            Fade.alpha = t;
            yield return null;
        }
        var load = SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => load.isDone);
        while (t > 0)
        {
            t -= Time.unscaledDeltaTime / .5f;
            t = Mathf.Clamp01(t);
            Fade.alpha = t;
            yield return null;
        }
        Fade.alpha = 0;
        loading = false;
    }
}
