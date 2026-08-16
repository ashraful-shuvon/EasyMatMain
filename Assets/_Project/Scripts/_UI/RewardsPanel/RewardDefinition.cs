using UnityEngine;

/// <summary>
/// Static, designer-authored data for one reward TYPE (e.g. "Galaxy Egg").
/// Create one asset per reward via: Assets > Create > Rewards > Reward Definition.
/// Same for every player - icon, name, keys cost, dollar value never change per-claim.
/// </summary>
[CreateAssetMenu(fileName = "New Reward", menuName = "Rewards/Reward Definition")]
public class RewardDefinition : ScriptableObject
{
    [Header("Identity")]
    public string rewardId;        // must match the ID used in your Firebase data
    public string rewardName;      // "Galaxy Egg"
    public Sprite icon;

    [Header("Value")]
    public int keysCost;           // 25
    public int dollarValue;        // 25
}