using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField] AudioClipPlayer player;

    public void Press()
    {
        player.gameObject.SetActive(true);
        player.transform.SetParent(null);
        DontDestroyOnLoad(player);
        SceneLoader.Load("Gameplay");
    }
}
