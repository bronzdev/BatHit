//using Google.Play.Review;
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class InAppReviewManager : MonoBehaviour
{
    //private ReviewManager _reviewManager;
    //private PlayReviewInfo _playReviewInfo;

    //private void Start()
    //{
    //    StartCoroutine(ShowReviewRoutine());
    //    UiStartPanel.OnReviewAppButtonPressed += ShowReview;
    //}

    //private void OnDestroy()
    //{
    //    UiStartPanel.OnReviewAppButtonPressed -= ShowReview;
    //}

    //public void ShowReview()
    //{
    //    StartCoroutine(ShowReviewRoutine());
    //}

    //private IEnumerator ShowReviewRoutine()
    //{
    //    _reviewManager = new ReviewManager();
    //    var requestFlowOperation = _reviewManager.RequestReviewFlow();
    //    yield return requestFlowOperation;
    //    if (requestFlowOperation.Error != ReviewErrorCode.NoError)
    //    {
    //        Hud.SetHudText?.Invoke("requestFlowOperation error" + requestFlowOperation.Error.ToString());
    //        yield break;
    //    }
    //    _playReviewInfo = requestFlowOperation.GetResult();
    //    if (_playReviewInfo == null)
    //    {
    //        Hud.SetHudText?.Invoke("_playReviewInfo error ");
    //    }
    //    var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
    //    yield return launchFlowOperation;
    //    _playReviewInfo = null; // Reset the object
    //    if (launchFlowOperation.Error != ReviewErrorCode.NoError)
    //    {
    //        Hud.SetHudText?.Invoke("launchFlowOperation error" + requestFlowOperation.Error.ToString());
    //        // Log error. For example, using requestFlowOperation.Error.ToString().
    //        yield break;
    //    }
    //}
}
