using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UiLevelManagerCanvas : MonoBehaviour
{
    public static Action OnHideBat;
    public static Action OnShootBall;
    public static Action OnShootButtonClicked;
    public static Action<int> OnLevelCleared;
    public static Action OnBossLevelCleared;
    public static Action OnAllBallOver;
    [SerializeField] private bool isTesting;
    [SerializeField] private int testingBallCount = 10;
    [SerializeField] private Image[] levelsImage;
    [SerializeField] private Button homeButton;
    [SerializeField] private GameObject quitPanel;
    [SerializeField] private Button yesQuitButon;
    [SerializeField] private Button noQuitButon;
    [SerializeField] private Button shootButton;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI nextLevelText;
    [SerializeField] private GameObject shotsRemainingPanel;
    [SerializeField] private TextMeshProUGUI shotsRemainingText;
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private TextMeshProUGUI noBallCountdownText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject bonusLevelPanel;
    [SerializeField] private TextMeshProUGUI bonusBallText;
    private Vector3 platformSpawnPosition = new Vector3(0, -3, 0);
    private GameObject mainPanel;
    private Platform currentPlatform;
    private bool areBallsEmpty;
    private float counterDown;
    private int semiLevelCounter;
    private int ballsRemaining;
    private bool isSemiLevelCleared = true;

    private void Awake()
    {
        UiLevelClearedCanvas.OnLevelClearedContinueButtonPressed += OnLevelClearedContinueButtonPressed;
        Ground.OnBlocksDestroyed += OnBlocksDestroyed;
        Player.OnPlayerDataLoaded += OnPlayerDataLoaded;
        UiStartCanvas.OnGameStart += OnGameStart;
        quitPanel.SetActive(false);
        homeButton.onClick.AddListener(OnBackButtonClicked);
        yesQuitButon.onClick.AddListener(OnYesQuitButonClicked);
        noQuitButon.onClick.AddListener(OnNoQuitButonClicked);
        shootButton.onClick.AddListener(ShootButtonClicked);
        mainPanel = transform.GetChild(0).gameObject;
        mainPanel.SetActive(false);
        if (Player.isPlayerDataLoaded)
        {
            StartNextLevel();
        }
    }

    private void OnDestroy()
    {
        UiLevelClearedCanvas.OnLevelClearedContinueButtonPressed -= OnLevelClearedContinueButtonPressed;
        Ground.OnBlocksDestroyed -= OnBlocksDestroyed;
        Player.OnPlayerDataLoaded -= OnPlayerDataLoaded;
        UiStartCanvas.OnGameStart -= OnGameStart;
        homeButton.onClick.RemoveListener(OnBackButtonClicked);
        yesQuitButon.onClick.RemoveListener(OnYesQuitButonClicked);
        noQuitButon.onClick.RemoveListener(OnNoQuitButonClicked);
        shootButton.onClick.RemoveListener(ShootButtonClicked);
    }

    private void Update()
    {
        if (areBallsEmpty && !isSemiLevelCleared)
        {
            counterDown -= Time.deltaTime;
            if (counterDown >= 0)
            {
                noBallCountdownText.text = counterDown.ToString("F1") + "s";
            }
            else
            {
                noBallCountdownText.text = "";
            }
        }
    }

    private void OnPlayerDataLoaded()
    {
        StartNextLevel();
    }

    private void ToggleBallCountdownPanel()
    {
        countdownPanel.SetActive(areBallsEmpty);
    }

    private void ToggleBonusPanel()
    {
        if (semiLevelCounter == AppData.maxSemiLevel - 1)
        {
            bonusLevelPanel.SetActive(true);
            shotsRemainingPanel.SetActive(false);
        }
        else
        {
            bonusLevelPanel.SetActive(false);
            shotsRemainingPanel.SetActive(true);
        }
    }

    private void OnBackButtonClicked()
    {
        quitPanel.gameObject.SetActive(true);
    }

    private void OnYesQuitButonClicked()
    {
        quitPanel.gameObject.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private void OnNoQuitButonClicked()
    {
        quitPanel.gameObject.SetActive(false);
    }

    #region Score Stuff
    private void OnBlocksDestroyed(Transform obj)
    {
        SetCurrentLevelScore(1);
    }

    private void SetCurrentLevelScore(int value)
    {
        AppData.currentScore += value;
        UiScoreAdder.OnAddScore?.Invoke(value);
        scoreText.text = "Score " + AppData.currentScore;
    }
    #endregion


    #region Balls/Shots stuff
    private void ShootButtonClicked()
    {
        if (ballsRemaining == 0)
        {
            return;
        }
        SetBallRemaining(ballsRemaining - 1);
    }

    private void SetBallRemaining(int value)
    {
        if (value <= 0)
        {
            if (semiLevelCounter == AppData.maxSemiLevel - 1)
            {
                bonusBallText.text = AppData.ballIcon + "x" + 0;
                StartCoroutine(OnBonusLevelCompleted());
            }
            else
            {
                value = 0;
                counterDown = AppData.watchAdCountdown;
                areBallsEmpty = true;
                ToggleBallCountdownPanel();
                StartCoroutine(OnBallOver());
            }
        }
        if (!isSemiLevelCleared)
        {
            OnShootBall?.Invoke();
        }
        ballsRemaining = value;
        shotsRemainingText.text = AppData.ballIcon + "x" + ballsRemaining;
        shotsRemainingText.color = ballsRemaining <= 1 ? ColorConstants.RedColor : Color.white;
    }

    private IEnumerator OnBonusLevelCompleted()
    {
        yield return new WaitForSeconds(1);
        OnHideBat?.Invoke();
        yield return new WaitForSeconds(AppData.watchAdCountdown);
        OnAllBlocksCleared();
    }

    private IEnumerator OnBallOver()
    {
        shootButton.enabled = false;
        yield return new WaitForSeconds(AppData.ballsOverCooldownTime);
        if (areBallsEmpty)
        {
            mainPanel.SetActive(false);
            OnAllBallOver?.Invoke();
        }
    }

    private void OnAllBlocksCleared()
    {
        SetCurrentLevelScore(ballsRemaining * Player.save.currentLevel);
        currentPlatform.OnAllBlocksCleared -= OnAllBlocksCleared;
        if (currentPlatform != null)
        {
            Destroy(currentPlatform.gameObject);
        }
        isSemiLevelCleared = true;
        StartCoroutine(LoadNextSemiLevelAfterDelay());
    }
    #endregion


    private void OnLevelClearedContinueButtonPressed()
    {
        mainPanel.SetActive(true);
        StartNextLevel();
    }

    private void StartNextLevel()
    {
        AppData.currentScore = 0;
        semiLevelCounter = 0;
        SetCurrentLevelScore(0);
        InitNewLevel();
    }

    private void InitNewLevel()
    {
        currentLevelText.text = Player.save.currentLevel.ToString();
        int nextLevel = Player.save.currentLevel + 1;
        nextLevelText.text = nextLevel.ToString();
        for (int i = 0; i < levelsImage.Length; i++)
        {
            levelsImage[i].color = ColorConstants.UiLockedLevel;
        }
        levelsImage[0].color = ColorConstants.UiCurrentLevel;
        StartSemiLevel();
    }

    private void StartSemiLevel()
    {
        ToggleBonusPanel();
        areBallsEmpty = false;
        ToggleBallCountdownPanel();
        noBallCountdownText.text = "";
        if (isTesting)
        {
            SetBallRemaining(testingBallCount);
        }
        else
        {
            int randomSemiLevel = UnityEngine.Random.Range(0, 5);
            string path = AppData.platformLevelPath + "" + semiLevelCounter + "/" + randomSemiLevel;
            currentPlatform = Instantiate(Resources.Load<Platform>(path) as Platform, platformSpawnPosition, Quaternion.identity);
            currentPlatform.transform.DOMoveY(0, 0.5f);
            SetBallRemaining(currentPlatform.shots);
        }
        isSemiLevelCleared = false;
        if (currentPlatform == null)
        {
            currentPlatform = GameObject.FindObjectOfType<Platform>();
        }
        currentPlatform.OnAllBlocksCleared += OnAllBlocksCleared;
        shootButton.enabled = true;
    }

    private void OnSemiLevelCleared(int semiLevelCounter)
    {
        if (semiLevelCounter == AppData.maxSemiLevel - 1)
        {
            OnBossLevelCleared?.Invoke();
        }
        levelsImage[semiLevelCounter - 1].color = ColorConstants.UiUnlockedLevel;
        levelsImage[semiLevelCounter].color = ColorConstants.UiCurrentLevel;
        shootButton.enabled = false;
    }

    private IEnumerator LoadNextSemiLevelAfterDelay()
    {
        shootButton.enabled = false;
        yield return new WaitForEndOfFrame();
        semiLevelCounter++;
        if (semiLevelCounter >= AppData.maxSemiLevel)
        {
            Player.save.currentLevel++;
            OnLevelCleared?.Invoke(AppData.currentScore);
            OnHideBat?.Invoke();
            mainPanel.SetActive(false);
            Player.SaveGameUserData();
        }
        else
        {
            OnSemiLevelCleared(semiLevelCounter);
            StartSemiLevel();
        }
    }

    private void OnGameStart()
    {
        mainPanel.SetActive(true);
    }
}