using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Attach this to the MyRewardPanel GameObject.
/// Handles switching between the Rewards tab and Claimed tab,
/// and highlighting whichever tab button is active.
/// Rewards tab is shown by default.
/// </summary>
public class RewardTabManager : MonoBehaviour
{
    [Header("Tab Buttons")]
    public Button rewardsButton;
    public Button claimedButton;

    [Header("Tab Panels")]
    public GameObject rewardsPanel;
    public GameObject claimedPanel;

    // ── Tab colours ───────────────────────────────────────────────────────────
    [Header("Tab Colors")]
    public Color tabActiveColor = new Color(0.53f, 0.20f, 0.98f, 1f);   // matches "Rewards" purple in your screenshot
    public Color tabInactiveColor = new Color(0.14f, 0.10f, 0.26f, 1f); // matches "Claimed" dark state
    public Color tabActiveText = Color.white;
    public Color tabInactiveText = new Color(0.68f, 0.64f, 0.85f, 1f);

    private bool onRewardsTab = true;

    void Start()
    {
        // Wire up tab button listeners
        rewardsButton.onClick.AddListener(() => SwitchTab(true));
        claimedButton.onClick.AddListener(() => SwitchTab(false));

        // Always default to Rewards tab
        SwitchTab(true);
    }

    // Called whenever MyRewardPanel is re-opened, so it always resets to Rewards
    void OnEnable()
    {
        SwitchTab(true);
    }

    // ── TAB SWITCHING ─────────────────────────────────────────────────────────

    void SwitchTab(bool showRewards)
    {
        onRewardsTab = showRewards;

        rewardsPanel.SetActive(showRewards);
        claimedPanel.SetActive(!showRewards);

        SetTabStyle(rewardsButton, showRewards);
        SetTabStyle(claimedButton, !showRewards);
    }

    void SetTabStyle(Button btn, bool active)
    {
        var img = btn.GetComponent<Image>();
        if (img != null) img.color = active ? tabActiveColor : tabInactiveColor;

        var label = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null) label.color = active ? tabActiveText : tabInactiveText;
    }
}
