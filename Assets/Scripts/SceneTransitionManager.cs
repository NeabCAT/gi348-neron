using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }
    public Image fadeImage;
    public float fadeTime = 0.5f;
    public float holdBlackTime = 0.3f;
    private bool isTransitioning = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        // จอดำตั้งแต่แรกเลย
        SetAlpha(1f);
    }

    void Start()
    {
        // ซีนแรกไม่ผ่าน TransitionRoutine ต้อง Snap เองตอนเริ่ม
        StartCoroutine(SnapOnFirstLoad());
    }

    public void GoToScene(string sceneName)
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionRoutine(sceneName));
    }

    IEnumerator TransitionRoutine(string sceneName)
    {
        isTransitioning = true;

        yield return StartCoroutine(Fade(0f, 1f));

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        load.allowSceneActivation = false;

        while (load.progress < 0.9f)
            yield return null;

        yield return new WaitForSeconds(holdBlackTime);

        load.allowSceneActivation = true;

        yield return null;
        yield return null;
        yield return new WaitForEndOfFrame();

        if (!TrySnapToCurrentZone())
            SnapCamera();

        yield return new WaitForEndOfFrame();

        yield return StartCoroutine(Fade(1f, 0f));

        isTransitioning = false;
    }

    bool TrySnapToCurrentZone()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return false;

        CameraZone[] zones = FindObjectsByType<CameraZone>(FindObjectsSortMode.None);
        foreach (CameraZone zone in zones)
        {
            Collider2D col = zone.GetComponent<Collider2D>();
            if (col != null && col.bounds.Contains(player.transform.position))
            {
                CameraFollow cam = FindFirstObjectByType<CameraFollow>();
                if (cam != null)
                    cam.SnapToZone(zone.targetOrthographicSize, zone.offset);
                return true;
            }
        }
        return false;
    }

    void SnapCamera()
    {
        CameraFollow cam = FindFirstObjectByType<CameraFollow>();
        if (cam != null) cam.SnapToTarget();
    }

    IEnumerator Fade(float from, float to)
    {
        if (fadeImage == null) yield break;
        float elapsed = 0f;
        Color c = fadeImage.color;
        c.a = from;
        fadeImage.color = c;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / fadeTime);
            fadeImage.color = c;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Player player = FindFirstObjectByType<Player>();
        if (player == null) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        player.isGrappling = false;
        player.isClimbing = false;
        player.canClimb = false;
    }

    IEnumerator SnapOnFirstLoad()
    {
        yield return null;
        yield return null;
        yield return new WaitForEndOfFrame();

        if (!TrySnapToCurrentZone())
            SnapCamera();

        // Snap เสร็จแล้วค่อย fade เปิด
        yield return StartCoroutine(Fade(1f, 0f));
    }
    void SetAlpha(float alpha)
    {
        if (fadeImage == null) return;
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }


    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}