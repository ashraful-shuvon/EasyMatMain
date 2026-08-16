using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fetches the player's claimed rewards and spawns a list row per claim.
///
/// ── DURING UI DESIGN ──
/// Tick "Use Mock Data" and fill in the "Mock Claims" list by hand in the
/// Inspector (just drag in RewardDefinition assets + type a date/code).
/// This drives the exact same RewardListItem / RewardDetailsPanel flow the
/// real backend will use later - no PlayFab connection needed yet.
///
/// ── LATER (BACKEND WIRED UP) ──
/// Untick "Use Mock Data". LoadClaimedRewards() will fetch from PlayFab
/// UserData instead. Nothing else in the reward system needs to change.
/// </summary>
public class RewardManager : MonoBehaviour
{
    [Header("── REWARD LIST UI ──")]
    public Transform listContainer;
    public GameObject rewardRowPrefab; // has a RewardListItem component on it

    [Header("── REWARD LIBRARY ──")]
    public List<RewardDefinition> rewardDefinitions; // assign all Reward Definition assets here

    [Header("── UI DESIGN / TESTING ──")]
    [Tooltip("ON while designing UI - uses Mock Claims below instead of PlayFab.")]
    public bool useMockData = true;
    public List<ClaimedRewardInfo> mockClaims; // fill these in by hand for now

    private const string CLAIMED_REWARDS_KEY = "claimedRewards";

    [Serializable]
    private class ClaimedRewardRecord
    {
        public string rewardId;
        public string claimedDate;
        public string redeemCode;
    }

    [Serializable]
    private class ClaimedRewardsWrapper
    {
        public List<ClaimedRewardRecord> rewards;
    }

    private List<GameObject> spawnedRows = new List<GameObject>();

    void OnEnable()
    {
        LoadClaimedRewards();
    }

    public void LoadClaimedRewards()
    {
        foreach (var row in spawnedRows)
            Destroy(row);
        spawnedRows.Clear();

        if (useMockData)
        {
            foreach (var claim in mockClaims)
                SpawnRow(claim);
            return;
        }

        // ── Real PlayFab fetch (used once backend is wired up) ──
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.LogError("❌ Not logged in — cannot load claimed rewards.");
            return;
        }

        PlayFabClientAPI.GetUserData(new GetUserDataRequest
        {
            Keys = new List<string> { CLAIMED_REWARDS_KEY }
        },
        result =>
        {
            if (result.Data == null || !result.Data.ContainsKey(CLAIMED_REWARDS_KEY))
            {
                Debug.Log("No claimed rewards yet.");
                return;
            }

            string json = result.Data[CLAIMED_REWARDS_KEY].Value;
            ClaimedRewardsWrapper wrapper = JsonUtility.FromJson<ClaimedRewardsWrapper>(json);

            if (wrapper?.rewards == null) return;

            foreach (var record in wrapper.rewards)
                SpawnRowFromRecord(record);
        },
        error =>
        {
            Debug.LogError("❌ Failed to load claimed rewards: " + error.GenerateErrorReport());
        });
    }

    private void SpawnRowFromRecord(ClaimedRewardRecord record)
    {
        RewardDefinition definition = rewardDefinitions.Find(d => d.rewardId == record.rewardId);

        if (definition == null)
        {
            Debug.LogWarning("⚠️ No RewardDefinition found for rewardId: " + record.rewardId);
            return;
        }

        SpawnRow(new ClaimedRewardInfo
        {
            definition = definition,
            claimedDate = record.claimedDate,
            redeemCode = record.redeemCode
        });
    }

    private void SpawnRow(ClaimedRewardInfo claim)
    {
        if (claim?.definition == null) return;

        GameObject row = Instantiate(rewardRowPrefab, listContainer);
        row.GetComponent<RewardListItem>().Populate(claim);
        spawnedRows.Add(row);
    }
}