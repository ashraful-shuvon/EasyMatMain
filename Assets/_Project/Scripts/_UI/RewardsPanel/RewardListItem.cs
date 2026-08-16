using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Attach to the reward row prefab (the "Existing Plan" style cards in the
/// Rewards list). Populated by RewardManager per claimed reward.
/// </summary>
public class RewardListItem : MonoBehaviour
{
    [Header("Row UI")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI claimedDateText;
    public Button viewDetailsButton;

    private ClaimedRewardInfo claim;

    private void Awake()
    {
        if (viewDetailsButton != null)
            viewDetailsButton.onClick.AddListener(OnViewDetailsClicked);
    }

    public void Populate(ClaimedRewardInfo claimedReward)
    {
        claim = claimedReward;

        if (iconImage != null)
            iconImage.sprite = claim.definition.icon;

        if (nameText != null)
            nameText.text = claim.definition.rewardName;

        if (claimedDateText != null)
            claimedDateText.text = "Claimed on " + claim.claimedDate;
    }

    private void OnViewDetailsClicked()
    {
        RewardDetailsPanel.Instance.ShowDetails(claim);
    }
}