using System.Collections;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// MainMenu splash sequence. Always runs on app open:
///   1. Title fades / scales in while the avatar flies up from below the
///      screen, spinning, and settles at anchoredPosition.y == avatarTargetY.
///   2. SplashPanel hides, LoadingPanel shows for at least loadingMinSeconds.
///   3. FirebaseAuthManager is told to run its startup flow, which decides
///      between HomePanel (already logged in) and the Login / Register flow.
///
/// Put this on the SplashPanel object and wire the references in the inspector.
/// </summary>
public class SplashScreenAnimator : MonoBehaviour
{
    [Header("Splash contents")]
    public RectTransform title;
    public RectTransform avatar;

    [Header("Avatar motion")]
    [Tooltip("anchoredPosition.y the avatar starts at (off screen, bottom).")]
    public float avatarStartY = -1421f;
    [Tooltip("anchoredPosition.y the avatar ends at.")]
    public float avatarTargetY = -650f;
    [Tooltip("Full spins the avatar makes on its way up.")]
    public float avatarSpins = 1f;
    public float avatarDuration = 1.1f;
    public Ease avatarEase = Ease.OutBack;

    [Header("Title motion")]
    public float titleDuration = 0.5f;
    public Ease titleEase = Ease.OutBack;

    [Header("Flow")]
    [Tooltip("Panel shown while the auth check runs. Build it yourself and drag it here.")]
    public GameObject loadingPanel;
    [Tooltip("Minimum seconds the loading panel stays up, even if auth resolves faster.")]
    public float loadingMinSeconds = 2f;
    [Tooltip("Safety cap: hide the loading panel after this long even if auth never reports back.")]
    public float loadingMaxSeconds = 15f;
    [Tooltip("Seconds to hold the finished splash before switching to loading.")]
    public float holdBeforeLoading = 0.4f;
    [Tooltip("Auth manager that owns Login / Register / Home. Falls back to FirebaseAuthManager.Instance.")]
    public FirebaseAuthManager authManager;

    private Sequence _seq;

    private void OnEnable()
    {
        Play();
    }

    private void OnDisable()
    {
        _seq?.Kill();
    }

    public void Play()
    {
        _seq?.Kill();

        CanvasGroup titleGroup = null;
        if (title != null)
        {
            titleGroup = title.GetComponent<CanvasGroup>();
            if (titleGroup == null) titleGroup = title.gameObject.AddComponent<CanvasGroup>();
            titleGroup.alpha = 0f;
            title.localScale = Vector3.one * 0.4f;
        }

        if (avatar != null)
        {
            Vector2 p = avatar.anchoredPosition;
            p.y = avatarStartY;
            avatar.anchoredPosition = p;
            avatar.localRotation = Quaternion.Euler(0f, 0f, 360f * avatarSpins);
        }

        if (loadingPanel != null)
            loadingPanel.SetActive(false);

        _seq = DOTween.Sequence();

        if (title != null)
        {
            _seq.Append(title.DOScale(1f, titleDuration).SetEase(titleEase));
            _seq.Join(titleGroup.DOFade(1f, titleDuration));
        }

        if (avatar != null)
        {
            float at = _seq.Duration() > 0f ? Mathf.Max(0f, _seq.Duration() - avatarDuration * 0.5f) : 0f;
            _seq.Insert(at, avatar.DOAnchorPosY(avatarTargetY, avatarDuration).SetEase(avatarEase));
            _seq.Insert(at, avatar.DOLocalRotate(Vector3.zero, avatarDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic));
        }

        _seq.AppendInterval(holdBeforeLoading);
        _seq.OnComplete(() => StartCoroutine(LoadingThenAuth()));
    }

    private IEnumerator LoadingThenAuth()
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        // Splash is done — hide it visually but keep the object active so this
        // coroutine keeps running (coroutines stop on inactive GameObjects).
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;

        // Start the auth / backend check now — it runs behind the loading screen.
        FirebaseAuthManager auth = authManager != null ? authManager : FirebaseAuthManager.Instance;
        if (auth != null)
        {
            FirebaseAuthManager.StartupResolved = false;
            auth.RunStartupFlow();
        }
        else
        {
            Debug.LogWarning("SplashScreenAnimator: no FirebaseAuthManager to hand off to.");
            FirebaseAuthManager.StartupResolved = true;
        }

        // Hold the loading screen until: the min time has passed AND the backend
        // check has resolved (logged in -> home is already swapped underneath, or
        // not logged in -> login panel is showing underneath). Bail out after the
        // safety cap so a stalled request can never freeze the loading screen.
        float elapsed = 0f;
        float cap = Mathf.Max(loadingMinSeconds, loadingMaxSeconds);
        while (elapsed < cap)
        {
            bool minDone = elapsed >= loadingMinSeconds;
            if (minDone && FirebaseAuthManager.StartupResolved)
                break;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (loadingPanel != null)
            loadingPanel.SetActive(false);

        gameObject.SetActive(false);
    }
}
