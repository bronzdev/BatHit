using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using DG.Tweening;

public class UiGameOverCanvas : MonoBehaviour
{
    public static Action OnRestartLevel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private Button watchAdButton;
    [SerializeField] private Button noThanksButton;
    [Space(10)]
    [SerializeField] private GameObject buttonsPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button shareButton;
    [SerializeField] private Button rateUsButton;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI highScore;
    private Image watchAdButtonImage;

    private void Awake()
    {
        UiLevelManagerCanvas.OnAllBallOver += GameOver;
        restartButton.onClick.AddListener(RestartLevelButtonClicked);
        watchAdButton.onClick.AddListener(WatchAdButtonClicked);
        noThanksButton.onClick.AddListener(NoThanksButtonClicked);
        shareButton.onClick.AddListener(ShareButtonClicked);
        rateUsButton.onClick.AddListener(OnReviewAppButtonPressed);
        watchAdButtonImage = watchAdButton.GetComponent<Image>();
        mainPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        restartButton.onClick.RemoveListener(RestartLevelButtonClicked);
        watchAdButton.onClick.RemoveListener(WatchAdButtonClicked);
        noThanksButton.onClick.RemoveListener(NoThanksButtonClicked);
        shareButton.onClick.RemoveListener(ShareButtonClicked);
        rateUsButton.onClick.RemoveListener(OnReviewAppButtonPressed);
        UiLevelManagerCanvas.OnAllBallOver -= GameOver;
    }

    private void NoThanksButtonClicked()
    {
        StopCoroutine(StartCountdown());
        watchAdButton.gameObject.SetActive(false);
        buttonsPanel.SetActive(true);
    }

    private void WatchAdButtonClicked()
    {
        GameAdManager.OnWatchAd?.Invoke(AdRewardType.FreeGems, "");
        AnalyticsManager.ButtonPressed(GameButtons.FreeGemsByAd);
    }

    private void OnReviewAppButtonPressed()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.bronz.bathit");
        AnalyticsManager.ButtonPressed(GameButtons.RateUs);
    }

    private void ShareButtonClicked()
    {
        ShareManager.OnShareAction?.Invoke();
        AnalyticsManager.ButtonPressed(GameButtons.Share);
        AnalyticsManager.SocialShare(UnityEngine.Analytics.ShareType.Image, UnityEngine.Analytics.SocialNetwork.None);
    }

    private void GameOver()
    {
        mainPanel.SetActive(true);
        score.text = "Score: " + AppData.currentScore.ToString();
        highScore.text = "High Score: " + Player.GetHighScore().ToString();
        //buttonsPanel.SetActive(false);
        //noThanksButton.gameObject.SetActive(false);
        //watchAdButton.gameObject.SetActive(true);
        //mainPanel.SetActive(true);
        AnalyticsManager.GameOverCurrentScore();
        AnalyticsManager.ScreenVisit(GameScreens.GameOver);
        //StartCoroutine(StartCountdown());
        //watchAdButtonImage.DOFillAmount(1, AppData.watchAdCountdown);
    }

    private IEnumerator StartCountdown()
    {

        yield return new WaitForSeconds(AppData.watchAdCountdown / 2);
        noThanksButton.gameObject.SetActive(true);
        yield return new WaitForSeconds(AppData.watchAdCountdown / 2);
        watchAdButton.gameObject.SetActive(false);
        buttonsPanel.SetActive(true);
    }

    private void RestartLevelButtonClicked()
    {
        mainPanel.SetActive(false);
        OnRestartLevel?.Invoke();
        AnalyticsManager.ButtonPressed(GameButtons.Restart);
        print("RestartLevelButtonClicked");
        SceneManager.LoadScene(0);
    }
}