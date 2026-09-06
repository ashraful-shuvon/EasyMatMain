using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanelAnimator : MonoBehaviour
{
    [Header("Main Object")]
    public RectTransform avatar;

    [SerializeField] private float bobAmplitude = 22f;
    [SerializeField] private float bobPeriod = 1.6f;

    [Header("Orbit")]
    public RectTransform orbitContainer;
    public RectTransform[] eggs;

    [SerializeField] private float orbitRadius = 550f;
    [SerializeField] private float orbitPeriod = 6f;
    [SerializeField] private bool clockwise = true;

    [Header("Loading Text")]
    public Text loadingText;

    [SerializeField] private string loadingWord = "Loading";
    [SerializeField] private float dotInterval = 0.35f;
    [SerializeField] private int maxDots = 3;

    private Vector2 avatarRestPosition;

    private float timer;
    private float dotTimer;
    private int dots;

    private Tween orbitTween;

    private void OnEnable()
    {
        timer = 0f;
        dotTimer = 0f;
        dots = 0;

        if (avatar != null)
            avatarRestPosition = avatar.anchoredPosition;

        // Center orbit around avatar
        if (orbitContainer != null && avatar != null)
        {
            orbitContainer.anchoredPosition = avatarRestPosition;
            orbitContainer.localRotation = Quaternion.identity;
        }

        PlaceEggs();

        if (loadingText != null)
            loadingText.text = loadingWord;

        StartOrbit();
    }

    private void OnDisable()
    {
        orbitTween?.Kill();
    }

    private void StartOrbit()
    {
        if (orbitContainer == null || orbitPeriod <= 0f)
            return;

        orbitTween?.Kill();

        float rotation = clockwise ? -360f : 360f;

        orbitTween = orbitContainer
            .DOLocalRotate(
                new Vector3(0f, 0f, rotation),
                orbitPeriod,
                RotateMode.LocalAxisAdd
            )
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental)
            .SetUpdate(true);
    }

    private void PlaceEggs()
    {
        if (eggs == null || eggs.Length == 0)
            return;

        int count = eggs.Length;

        for (int i = 0; i < count; i++)
        {
            if (eggs[i] == null)
                continue;

            float angle = (360f * i / count) * Mathf.Deg2Rad;

            eggs[i].anchoredPosition = new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * orbitRadius;

            eggs[i].localRotation = Quaternion.identity;
        }
    }

    private void Update()
    {
        float deltaTime = Time.unscaledDeltaTime;

        timer += deltaTime;
        dotTimer += deltaTime;

        // Avatar bob
        if (avatar != null)
        {
            float angle = timer * Mathf.PI * 2f / bobPeriod;

            float yOffset = Mathf.Sin(angle) * bobAmplitude;

            avatar.anchoredPosition =
                avatarRestPosition + new Vector2(0f, yOffset);
        }

        // Loading dots
        if (loadingText != null && dotTimer >= dotInterval)
        {
            dotTimer -= dotInterval;

            dots++;

            if (dots > maxDots)
                dots = 0;

            loadingText.text =
                loadingWord + new string('.', dots);
        }
    }

    private void LateUpdate()
    {
        // Counter-rotate eggs so they always remain upright.
        if (eggs == null)
            return;

        foreach (RectTransform egg in eggs)
        {
            if (egg == null)
                continue;

            egg.rotation = Quaternion.identity;
        }
    }
}
