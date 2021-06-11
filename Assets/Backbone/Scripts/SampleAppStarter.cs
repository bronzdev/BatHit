using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SampleAppStarter : MonoBehaviour
{
    [SerializeField] private Button showAd;
    [SerializeField] private TextMeshProUGUI adStatusText;
    private string gameIdAndroid = "4090539";
    private string gameIdIos = "4090538";

    private void Start()
    {
        showAd.onClick.AddListener(ShowRewardVideo);
#if UNITY_IOS
        AdsManager.InitAdManager?.Invoke(gameIdIos, true);
#endif
#if UNITY_ANDROID
        AdsManager.InitAdManager?.Invoke(gameIdAndroid, true);
#endif
        AdsManager.OnAdStatus += OnAdStatus;
    }

    private void OnDestroy()
    {
        showAd.onClick.RemoveListener(ShowRewardVideo);
        AdsManager.OnAdStatus -= OnAdStatus;
    }

    private void ShowRewardVideo()
    {
        AdsManager.ShowRewardedAd?.Invoke();
    }

    private void OnAdStatus(AdStatus adStatus)
    {
        print(adStatus.ToString());
        if (adStatus != AdStatus.IsReady)
        {
            adStatusText.text = adStatus.ToString();
        }
        switch (adStatus)
        {
            case AdStatus.Finished:
                break;
            case AdStatus.Skipped:
                break;
            case AdStatus.Error:
                break;
            case AdStatus.Failed:
                break;
            case AdStatus.Started:
                break;
            case AdStatus.IsReady:
                showAd.interactable = true;
                break;
            default:
                break;
        }
    }
}