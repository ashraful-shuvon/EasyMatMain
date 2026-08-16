using UnityEngine;

/// <summary>
/// Runtime data for one CLAIM of a reward - built when you pull the player's
/// claimed rewards from Firebase. Pairs a static RewardDefinition asset with
/// the per-claim fields that differ every time (date, redeem code).
/// </summary>
[System.Serializable]
public class ClaimedRewardInfo
{
    public RewardDefinition definition;  // which reward type this claim is for
    public string claimedDate;           // "May 24, 2026" - from Firebase doc
    public string redeemCode;            // "GH25-X8KP-42LM" - from Firebase doc
}