using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach this to the MyRewardPanel GameObject.
/// Wire RewardsBtn's OnClick() -> SwitchTab(true)
/// Wire ClaimedBtn's OnClick() -> SwitchTab(false)
/// Rewards tab is always shown by default.
/// </summary>
public class RewardTabManager : MonoBehaviour
{
    [Header("Tab Panels")]
    public GameObject rewardsPanel;
    public GameObject claimedPanel;

    [Header("Tab Button Images")]
    public Image rewardsBtnImage;
    public Image claimedBtnImage;

    [Header("Tab Alpha")]
    [Range(0f, 1f)] public float activeAlpha = 1f;
    [Range(0f, 1f)] public float inactiveAlpha = 0.5f;

    private void Start()
    {
        SwitchTab(true);
    }

    // Always reset to Rewards whenever this panel is opened
    private void OnEnable()
    {
        SwitchTab(true);
    }

    // Wire this on RewardsBtn OnClick() with param true,
    // and on ClaimedBtn OnClick() with param false.
    public void SwitchTab(bool showRewards)
    {
        rewardsPanel.SetActive(showRewards);
        claimedPanel.SetActive(!showRewards);

        SetTabAlpha(rewardsBtnImage, showRewards);
        SetTabAlpha(claimedBtnImage, !showRewards);
    }

    private void SetTabAlpha(Image img, bool active)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = active ? activeAlpha : inactiveAlpha;
        img.color = c;
    }
}