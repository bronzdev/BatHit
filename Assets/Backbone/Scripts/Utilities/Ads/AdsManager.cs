using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;
using System;
using UnityEngine.Networking;

public class AdsManager : Singleton<AdsManager>, IUnityAdsListener
{
    public static Action<AdStatus> OnAdStatus;
    public static Action ShowRewardedAd;
    public static Action<string, bool> InitAdManager;
    [SerializeField] bool testMode = false;

    protected override void Awake()
    {
        base.Awake();
        InitAdManager += InitSetupAds;
        ShowRewardedAd += ShowRewardedVideo;
    }

    private void InitSetupAds(string gameId, bool hasBanner)
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);
        if (hasBanner)
        {
            StartCoroutine(ShowBannerWhenReady());
        }
    }

    private void OnDestroy()
    {
        InitAdManager -= InitSetupAds;
        ShowRewardedAd -= ShowRewardedVideo;
    }

    private IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady("banner"))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show("banner");
    }

    private void ShowRewardedVideo()
    {
        Advertisement.Show("rewardedVideo");
    }


    #region Interface IUnityAdsListener
    public void OnUnityAdsDidFinish(string surfacingId, ShowResult showResult)
    {
        switch (showResult)
        {
            case ShowResult.Failed://The ad did not finish due to an error.
                OnAdStatus?.Invoke(AdStatus.Failed);
                break;
            case ShowResult.Skipped: //Do not reward the user for skipping the ad.
                OnAdStatus?.Invoke(AdStatus.Skipped);
                break;
            case ShowResult.Finished: //Reward the user for watching the ad to completion.
                OnAdStatus?.Invoke(AdStatus.Finished);
                break;
            default:
                break;
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        OnAdStatus?.Invoke(AdStatus.Error);
    }

    public void OnUnityAdsDidStart(string surfacingId)
    {
        OnAdStatus?.Invoke(AdStatus.Started);
    }

    public void OnUnityAdsReady(string placementId)
    {
        OnAdStatus?.Invoke(AdStatus.IsReady);
    }
    #endregion


    #region Other Utilities
    public void CheckInternet(Action<bool> OnInternetConnectionAvailable)
    {
        StartCoroutine(CheckInternetConnection(OnInternetConnectionAvailable));
    }

    private IEnumerator CheckInternetConnection(Action<bool> OnInternetConnectionAvailable)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://www.google.com/"))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.responseCode == 200)
            {
                OnInternetConnectionAvailable?.Invoke(true);
            }
            else
            {
                OnInternetConnectionAvailable?.Invoke(false);
            }
        }
    }
    #endregion
}

public enum AdStatus
{
    Finished,
    Skipped,
    Error,
    Failed,
    Started,
    IsReady
}