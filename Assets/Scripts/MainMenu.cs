using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [SerializeField] public string gameSceneName;

    public void PlayGame()
    {
        ScreenFader.Instance.FadeToScene(gameSceneName);
    }
    public void Quit()
    {
        StartCoroutine(QuitRoutine());
    }

    IEnumerator QuitRoutine()
    {
        yield return new WaitForSecondsRealtime(1f);
        Application.Quit();
    }
}