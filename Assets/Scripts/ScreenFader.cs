using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [Header("Fade Settings")]
    public Image fadePanel;
    public float fadeDuration = 1f;

    void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject); // ลบตัวเก่าทิ้ง

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    public void FadeToScene(int sceneIndex)
    {
        StartCoroutine(FadeOutAndLoad(sceneIndex));
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        yield return StartCoroutine(Fade(0f, 1f));
        SceneManager.LoadScene(sceneName);
        yield return StartCoroutine(Fade(1f, 0f));
    }

    IEnumerator FadeOutAndLoad(int sceneIndex)
    {
        yield return StartCoroutine(Fade(0f, 1f));
        SceneManager.LoadScene(sceneIndex);
        yield return StartCoroutine(Fade(1f, 0f));
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadePanel == null) yield break;

        fadePanel.gameObject.SetActive(true);
        float elapsed = 0f;
        Color color = fadePanel.color;
        color.a = startAlpha;
        fadePanel.color = color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadePanel.color = color;

        if (endAlpha == 0f)
            fadePanel.gameObject.SetActive(false);
    }
}