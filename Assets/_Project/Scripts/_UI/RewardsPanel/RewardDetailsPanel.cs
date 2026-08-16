using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Attach this to the single "Reward Details" panel GameObject.
/// Call RewardDetailsPanel.Instance.ShowDetails(claimedReward) from any reward
/// list item's "View Details" button to populate this same panel with that
/// specific claim's info.
/// </summary>
public class RewardDetailsPanel : MonoBehaviour
{
    public static RewardDetailsPanel Instance;

    [Header("Header")]
    public TextMeshProUGUI categoryLabelText; // small text under "Reward Details" title, e.g. "Galaxy Egg"

    [Header("Reward Card")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI claimedDateText;
    public TextMeshProUGUI keysText;

    [Header("Reward Value")]
    public TextMeshProUGUI dollarValueText;

    [Header("Redeem Code")]
    public TextMeshProUGUI redeemCodeText;
    public Button copyCodeButton;
    public TextMeshProUGUI copyButtonLabel;

    private string currentRedeemCode;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        if (copyCodeButton != null)
            copyCodeButton.onClick.AddListener(OnCopyCode);
    }

    // Call this from a reward list item's "View Details" button
    public void ShowDetails(ClaimedRewardInfo claim)
    {
        if (claim == null || claim.definition == null) return;

        RewardDefinition def = claim.definition;

        categoryLabelText.text = def.rewardName;
        nameText.text = def.rewardName;
        claimedDateText.text = "Claimed on " + claim.claimedDate;
        keysText.text = def.keysCost + " keys";
        dollarValueText.text = "$" + def.dollarValue;
        redeemCodeText.text = claim.redeemCode;

        if (iconImage != null)
            iconImage.sprite = def.icon;

        currentRedeemCode = claim.redeemCode;

        // Open the panel through your existing MenuManager
        if (MenuManager.instance != null)
            MenuManager.instance.ShowPanel(gameObject);
    }

    private void OnCopyCode()
    {
        GUIUtility.systemCopyBuffer = currentRedeemCode;

        if (copyButtonLabel != null)
            StartCoroutine(FlashCopyLabel());
    }

    private System.Collections.IEnumerator FlashCopyLabel()
    {
        string original = copyButtonLabel.text;
        copyButtonLabel.text = "Copied!";
        yield return new WaitForSeconds(1.5f);
        copyButtonLabel.text = original;
    }
}